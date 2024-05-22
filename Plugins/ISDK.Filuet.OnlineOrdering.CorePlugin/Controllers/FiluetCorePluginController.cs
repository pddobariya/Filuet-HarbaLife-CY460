using ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
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

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    public class FiluetCorePluginController : BasePluginController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService; 

        #endregion

        #region Ctor
        public FiluetCorePluginController(
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }
        #endregion

        #region Methods

        [JetBrains.Annotations.AspMvcSuppressViewError]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
            {
                return AccessDeniedView();
            }

            var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            var model = PluginMapper.Mapper.Map<FiluetCorePluginSettings, ConfigurationModel>(settings);
            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Configure.cshtml", model);
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
            var settings = PluginMapper.Mapper.Map<ConfigurationModel, FiluetCorePluginSettings>(model);
            await _settingService.SaveSettingAsync(settings, await _storeContext.GetActiveStoreScopeConfigurationAsync());

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}
