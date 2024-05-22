using ISDK.Filuet.ExternalSSOAuthPlugin.Areas.Admin.Models;
using ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core.Infrastructure.Mapper;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Areas.Admin.Controllers
{
    public class SSOAuthAdminController: BaseAdminController
    {
        #region Fields

        private SSOAuthPluginSettings _sSOAuthPluginSettings;
        private readonly IPermissionService _permissionService;
        private readonly IOptionsMonitorCache<SSOAuthOptions> _optionsCache;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor
        public SSOAuthAdminController(SSOAuthPluginSettings sSOAuthPluginSettings,
            IPermissionService permissionService,
            IOptionsMonitorCache<SSOAuthOptions> optionsCache,
            ISettingService settingService,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _sSOAuthPluginSettings = sSOAuthPluginSettings;
            _permissionService = permissionService;
            _optionsCache = optionsCache;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }
        #endregion

        #region Method
        
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = AutoMapperConfiguration.Mapper.Map<ConfigurationModel>(_sSOAuthPluginSettings);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            _sSOAuthPluginSettings = AutoMapperConfiguration.Mapper.Map<SSOAuthPluginSettings>(model);

            await _settingService.SaveSettingAsync(_sSOAuthPluginSettings);

            _optionsCache.TryRemove(SSOAuthHerbalifeDefaults.AuthenticationScheme);

            //now clear settings cache
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return View(model);
        }
        #endregion
    }
}
