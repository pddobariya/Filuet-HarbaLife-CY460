using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Controllers
{
    public class ShippingWidgetPluginController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ShippingWidgetPluginController(
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {        
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        #region Configure

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
            {
                return AccessDeniedView();
            }

            var deliverySettings = await LoadSettingsAsync();

            var model = new ConfigurationModel
            {
                OmnivaUrl = deliverySettings.OmnivaUrl,
                DpdFtpLogin = deliverySettings.DpdFtpLogin,
                DpdFtpPwd = deliverySettings.DpdFtpPwd,
                DpdFtpUrl = deliverySettings.DpdFtpUrl
            };
            return View(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Configure();
            }

            //save settings
            var deliverySettings = await LoadSettingsAsync();
            deliverySettings.DpdFtpLogin = model.DpdFtpLogin;
            deliverySettings.DpdFtpPwd = model.DpdFtpPwd;
            deliverySettings.DpdFtpUrl = model.DpdFtpUrl;
            deliverySettings.OmnivaUrl = model.OmnivaUrl;
           await _settingService.SaveSettingAsync(deliverySettings);

            //now clear settings cache
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }

        #endregion

        #region LoadSettingsAsync

        private async Task<ShippingWidgetPluginSettings> LoadSettingsAsync()
        {
            var deliverySettings =
                await _settingService.LoadSettingAsync<ShippingWidgetPluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            
            return deliverySettings;
        }

        #endregion

        #endregion
    }
}
