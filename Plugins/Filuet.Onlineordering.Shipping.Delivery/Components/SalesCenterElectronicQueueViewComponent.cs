using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Components
{
    public class SalesCenterElectronicQueueViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor
        public SalesCenterElectronicQueueViewComponent(
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var deliveryPluginSettings = _settingService.LoadSetting<DeliveryPluginSettings>( await _storeContext.GetActiveStoreScopeConfigurationAsync());
            if (!deliveryPluginSettings.ShowInvitation)
                return Content("");
            var model = deliveryPluginSettings.SalesCenterElectronicQueueInvitation;
            return View("~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Views/Default.cshtml",model);
        }
        #endregion
    }
}
