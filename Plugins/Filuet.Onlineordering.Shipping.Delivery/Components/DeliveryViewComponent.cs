using Filuet.Onlineordering.Shipping.Delivery.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Components
{
    public class DeliveryViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IDeliveryPriceService _deliveryPriceService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDistributorService _distributorService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ICountryDeliveryCustomizingService _countryDeliveryCustomizing;
        private readonly IFiluetShippingService _filuetShippingService;

        #endregion

        #region Ctor
        public DeliveryViewComponent(IDeliveryPriceService deliveryPriceService, 
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            IDistributorService distributorService, 
            ISettingService settingService, 
            IStoreContext storeContext, 
            ICountryDeliveryCustomizingService countryDeliveryCustomizing,
            IFiluetShippingService filuetShippingService)
        {
            _deliveryPriceService = deliveryPriceService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _distributorService = distributorService;
            _settingService = settingService;
            _storeContext = storeContext;
            _countryDeliveryCustomizing = countryDeliveryCustomizing;
            _filuetShippingService = filuetShippingService;
        }

        #endregion

        #region Methods
        public async Task<IViewComponentResult> InvokeAsync()
        {
           
            var deliveryPluginSettings = _settingService.LoadSetting<DeliveryPluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            ViewBag.RequirePostCode = true;
            ViewBag.PhoneMask = deliveryPluginSettings.PhoneMask;
            return View("~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Shared/Components/Delivery/Default.cshtml");
        }
        #endregion
    }
}
