using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Directory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public class GeoIpHelper : IGeoIpHelper
    {
        #region Fields

        private Dictionary<string, string> _cityCountryMapping = new Dictionary<string, string>();
        private Dictionary<string, string> _cultureCountryMapping = new Dictionary<string, string>();
        private readonly IGeoLookupService _geoLookupService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public GeoIpHelper(
            IGeoLookupService geoLookupService,
            IGenericAttributeService genericAttributeService)
        {
            _geoLookupService = geoLookupService;
            _genericAttributeService = genericAttributeService;
            _cityCountryMapping.Add("tallin", "EE");
            _cityCountryMapping.Add("riga", "LV");
            _cityCountryMapping.Add("vilnius", "LT");
            _cultureCountryMapping.Add("lv-LV", "LV");
            _cultureCountryMapping.Add("lt-LT", "LT");
            _cultureCountryMapping.Add("et-EE", "EE");
            _cultureCountryMapping.Add("ru-RU", "RU");
        }

        #endregion

        #region Methods

        public async Task<string> GetCountryByIpAddressAsync(Customer customer, string clientTimeZone = null)
        {
            if (!string.IsNullOrWhiteSpace(clientTimeZone))
            {
                //try to infere from client timezone using known city mapping
                //expected value e.g. Europe/Riga
                string[] candidates = clientTimeZone.Contains("/") ? clientTimeZone.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim().ToLowerInvariant()).ToArray() : new string[] { clientTimeZone.Trim().ToLowerInvariant() };
                foreach (var kvp in _cityCountryMapping)
                {
                    if (candidates.Contains(kvp.Key))
                    {
                        return kvp.Value;
                    }
                }
            }
            //customer.LastIpAddress
            //try to get from generic attributes
            string lastGeoCodedIpAddress = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.LastGeoCodedIpAddress);
            string lastGeoCodedCountry = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.LastGeoCodedCountry);
            if (!string.IsNullOrWhiteSpace(lastGeoCodedCountry) && !string.IsNullOrWhiteSpace(lastGeoCodedIpAddress) && lastGeoCodedIpAddress == customer.LastIpAddress)
            {
                return lastGeoCodedCountry;
            }
            //get from geoip with standard nopCommerce (but potentially outdated) and two fallback free APIs 

            string countryCode = _geoLookupService.LookupCountryIsoCode(customer.LastIpAddress);

            if (string.IsNullOrWhiteSpace(countryCode))
            {
                countryCode = await GetIpApiCountryCodeAsync(customer.LastIpAddress);
            }
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                countryCode = await GetIpInfoCountryCodeAsync(customer.LastIpAddress);
            }

            return countryCode;
        }

        public async Task SaveGeoCodedCountryAsync(Customer customer, string countryCode)
        {
            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                if (!_cultureCountryMapping.ContainsValue(countryCode))
                {
                    countryCode = _cultureCountryMapping["lv-LV"];
                }
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.LastGeoCodedIpAddress, customer.LastIpAddress);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.LastGeoCodedCountry, countryCode);
            }
        }

        private async Task<string> GetIpApiCountryCodeAsync(string clientIp)
        {
            IpApiResponseModel ipInfo = new IpApiResponseModel();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string info = await client.GetStringAsync("http://ip-api.com/json/" + clientIp);
                    ipInfo = JsonConvert.DeserializeObject<IpApiResponseModel>(info);
                }
            }
            catch (Exception)
            {
                ipInfo.CountryCode = null;
            }

            return ipInfo.CountryCode;
        }

        private async Task<string> GetIpInfoCountryCodeAsync(string clientIp)
        {
            IpInfoResponseModel ipInfo = new IpInfoResponseModel();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string info = await client.GetStringAsync("http://ipinfo.io/" + clientIp);
                    ipInfo = JsonConvert.DeserializeObject<IpInfoResponseModel>(info);
                }
            }
            catch (Exception)
            {
                ipInfo.Country = null;
            }

            return ipInfo.Country;
        }

        public async Task<bool> IsCountryGeoCodedAsync(Customer customer)
        {
            string lastGeoCodedIpAddress =await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.LastGeoCodedIpAddress);
            string lastGeoCodedCountry =await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.LastGeoCodedCountry);
            if (!string.IsNullOrWhiteSpace(lastGeoCodedCountry) && !string.IsNullOrWhiteSpace(lastGeoCodedIpAddress) && lastGeoCodedIpAddress == customer.LastIpAddress)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsShowShippingCountryPopupAsync(Customer customer)
        {
            var isShowShippingCountryPopup =await _genericAttributeService.GetAttributeAsync<bool>(customer, CustomerAttributeNames.IsShowShippingCountryPopup);
            if (isShowShippingCountryPopup || !await IsCountryGeoCodedAsync(customer))
            {
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.IsShowShippingCountryPopup, false);
                return true;
            }
            return false;
        }

        public async Task ResetGeoCodedCountryAsync(Customer customer)
        {
            await _genericAttributeService.SaveAttributeAsync<string>(customer, CustomerAttributeNames.LastGeoCodedIpAddress, null);
            await _genericAttributeService.SaveAttributeAsync<string>(customer, CustomerAttributeNames.LastGeoCodedCountry, null);
        }

        #endregion
    }
}
