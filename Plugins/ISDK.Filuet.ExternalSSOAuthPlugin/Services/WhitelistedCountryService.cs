using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts;
using Nop.Core.Domain.Directory;
using Nop.Services.Configuration;
using Nop.Services.Directory;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Services
{
    internal class WhitelistedCountryService : IWhitelistedCountryService
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly SSOAuthPluginSettings _sSOAuthPluginSettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor
        public WhitelistedCountryService(
            ICountryService countryService,
            SSOAuthPluginSettings sSOAuthPluginSettings,
            ISettingService settingService)
        {
            _countryService = countryService;
            _sSOAuthPluginSettings = sSOAuthPluginSettings;
            _settingService = settingService;
        }
        #endregion

        #region Methods

        public async Task<IEnumerable<Country>> GetWhitelistedCountries()
        {
            var countriesStr = _sSOAuthPluginSettings.WhitelistedCountries;

            var countryCodes = countriesStr
                .TrimStart(',')
                .TrimEnd(',')
                .ToLower().Split(',');

            var countriesList =(await _countryService.GetAllCountriesAsync())
                .Where(c => countryCodes.Contains(c.TwoLetterIsoCode.ToLower()));

            return countriesList;
        }

        public async void SaveWhitelistedCountries(string[] countryCodes)
        {
            var result = String.Empty;

            var countriesList = (await _countryService.GetAllCountriesAsync())
               .Where(c => countryCodes.Contains(c.TwoLetterIsoCode.ToLower()));

            foreach (var country in countriesList)
            {
                result += $"{country.ThreeLetterIsoCode.ToLower()},";
            }

            _sSOAuthPluginSettings.WhitelistedCountries = result.TrimEnd(',');

           await _settingService.SaveSettingAsync(_sSOAuthPluginSettings);
        }

        #endregion
    }

}
