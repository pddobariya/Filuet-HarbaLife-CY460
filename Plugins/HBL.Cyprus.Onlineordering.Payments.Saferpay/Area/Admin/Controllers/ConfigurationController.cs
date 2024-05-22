using HBL.Cyprus.Onlineordering.Payments.Saferpay.Area.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Area.Admin.Controllers
{
    public class ConfigurationController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor
        public ConfigurationController(IPermissionService permissionService,
            IStoreContext storeContext,
            ISettingService settingService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _permissionService = permissionService;
            _storeContext = storeContext;
            _settingService = settingService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentSettings =await _settingService.LoadSettingAsync<SaferpayPaymentSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Bypass = paymentSettings.Bypass,
                APIUrl = paymentSettings.APIUrl,
                APIUsername = paymentSettings.APIUsername,
                APIPassword = paymentSettings.APIPassword,
                APISpecVersion = paymentSettings.APISpecVersion,
                CurrencyCode = paymentSettings.CurrencyCode,
                CustomerId = paymentSettings.CustomerId,
                TerminalId = paymentSettings.TerminalId
            };
            return View(model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();


            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentSettings =await _settingService.LoadSettingAsync<SaferpayPaymentSettings>(storeScope);

            //save settings
            paymentSettings.Bypass = model.Bypass;
            paymentSettings.APIUrl = model.APIUrl;
            paymentSettings.APIUsername = model.APIUsername;
            paymentSettings.APIPassword = model.APIPassword;
            paymentSettings.APISpecVersion = model.APISpecVersion;
            paymentSettings.TerminalId = model.TerminalId;
            paymentSettings.CustomerId = model.CustomerId;
            paymentSettings.CurrencyCode = model.CurrencyCode;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingAsync(paymentSettings, x => x.Bypass, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.APIUrl, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.APIUsername, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.APIPassword, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.APISpecVersion, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.TerminalId, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.CustomerId, storeScope, false);
            await _settingService.SaveSettingAsync(paymentSettings, x => x.CurrencyCode, storeScope, false);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
