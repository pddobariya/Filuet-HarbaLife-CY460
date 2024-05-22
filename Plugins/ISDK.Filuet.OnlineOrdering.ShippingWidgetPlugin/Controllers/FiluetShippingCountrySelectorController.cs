using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Controllers
{
    public class FiluetShippingCountrySelectorController : BasePluginController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IFiluetShippingCartService _filuetShippingCartService;
        private readonly IShippingWidgetService _shippingWidgetService;
        private readonly IDistributorService _distributorService;
        private readonly ICrmDataProviderAdapter _crmDataProvider;
        private readonly IGeoIpHelper _geoIpHelper;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FiluetShippingCountrySelectorController(
            IWorkContext workContext,
            IFiluetShippingCartService filuetShippingCartService,
            IShippingWidgetService shippingWidgetService,
            IDistributorService distributorService,
            ICrmDataProviderAdapter crmDataProvider,
            IGeoIpHelper geoIpHelper,
            ILogger logger)
        {
            _workContext = workContext;
            _filuetShippingCartService = filuetShippingCartService;
            _shippingWidgetService = shippingWidgetService;
            _distributorService = distributorService;
            _crmDataProvider = crmDataProvider;
            _geoIpHelper = geoIpHelper;
            _logger = logger;
        }

        #endregion

        #region Methods

        #region Update


        public async Task<JsonResult> Update([FromBody] ShippingComputationOptionModel model)
        {
            await _shippingWidgetService.SetShippingComputationOptionAsync(model);
            var skus = await _filuetShippingCartService.IsCartValid();

            Customer customer = await _workContext.GetCurrentCustomerAsync();
            await _geoIpHelper.SaveGeoCodedCountryAsync(customer, model.CountryCode);

            // Update customer role
            var distributorFullProfile = await _crmDataProvider.GetDistributorFullProfileAsync(customer);

            if (distributorFullProfile != null)
                await _distributorService.SetDistributorRegionalRole(customer, distributorFullProfile.Discount, model.CountryCode);
            else
                await _logger.ErrorAsync(string.Format("DistributorFullProfile is null for customer {0}",customer.Id),null, customer);

            return Json(new
            {
                success = !skus.Any(),
                skus = string.Join(", ", skus)
            });
        }


        #endregion

        #region DefaultCountry

        public async Task<JsonResult> DefaultCountry(string clientTimeZone)
        {
            var model = (await _shippingWidgetService.GetShippingComputationOptionsAsync()).FirstOrDefault(c => c.IsSelected == true);

            return Json(new
            {
                model.CountryCode,
                CountryName = model.Name
            });
        }

        #endregion

        #region PopupConfirm

        public async Task<JsonResult> PopupConfirm([FromBody] ShippingCountryPopupConfirmModel model)
        {
            Customer customer = await _workContext.GetCurrentCustomerAsync();

            if (model.IsConfirmed)
            {
                await _geoIpHelper.SaveGeoCodedCountryAsync(customer, model.Country);

                //Update customer role
                var distributorFullProfile = await _crmDataProvider.GetDistributorFullProfileAsync(customer);
                await _distributorService.SetDistributorRegionalRole(customer, distributorFullProfile?.Discount, model.Country);
            }
            else
            {
                await _geoIpHelper.ResetGeoCodedCountryAsync(customer);
            }

            return Json(new
            {
                Success = true
            });
        }

        #endregion

        #endregion

    }
}
