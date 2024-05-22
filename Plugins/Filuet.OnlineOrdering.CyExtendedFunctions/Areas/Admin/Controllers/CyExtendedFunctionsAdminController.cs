using Filuet.OnlineOrdering.CyExtendedFunctions.Areas.Admin.Models;
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

namespace Filuet.OnlineOrdering.CyExtendedFunctions.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class CyExtendedFunctionsAdminController : BasePluginController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor
        public CyExtendedFunctionsAdminController(IStoreContext storeContext,
            ISettingService settingService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Method
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            {
                return AccessDeniedView();
            }
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var cyExtendedFunctionsSettings = await _settingService.LoadSettingAsync<CyExtendedFunctionsSettings>(storeScope);
            var model = new ConfigurationModel
            {
                AddSKU3798ForSKU5451 = cyExtendedFunctionsSettings.AddSKU3798ForSKU5451,
                ActiveStoreScopeConfiguration = storeScope,
            };
            if (storeScope > 0)
            {
                model.AddSKU3798ForSKU5451_OverrideForStore = await _settingService.SettingExistsAsync(cyExtendedFunctionsSettings, x => x.AddSKU3798ForSKU5451, storeScope);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Configure();
            }
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var cyExtendedFunctionsSettings = await _settingService.LoadSettingAsync<CyExtendedFunctionsSettings>(storeScope);
            cyExtendedFunctionsSettings.AddSKU3798ForSKU5451 = model.AddSKU3798ForSKU5451;
            await _settingService.SaveSettingOverridablePerStoreAsync(cyExtendedFunctionsSettings, x => x.AddSKU3798ForSKU5451, model.AddSKU3798ForSKU5451_OverrideForStore, storeScope, false);
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }

        #endregion

    }
}
