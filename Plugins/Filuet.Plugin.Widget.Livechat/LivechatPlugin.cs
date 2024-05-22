using Filuet.Plugin.Widget.Livechat.Components;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filuet.Plugin.Widget.Livechat
{
    public class LivechatPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        public const string WidgetsLivechat = "widgetslivechat";

        #endregion

        #region Ctor

        public LivechatPlugin(ILocalizationService localizationService,
            IWebHelper webHelper,
            ISettingService settingService)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HeadHtmlTag });
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsLivechat/Configure";
        }

        public override async Task InstallAsync()

        {
            var settings = new LivechatSettings
            {
                TrackingScript = @"<!-- Please append livechat script here!!! --> ",
            };
            await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.Livechat.TrackingScript"] = "Tracking code",
                ["Plugins.Widgets.Livechat.Configure"] = "Configure",
            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<LivechatSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.Livechat");

            await base.UninstallAsync();
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
                return typeof(WidgetsLivechatViewComponent);
        }
        public bool HideInWidgetList => false;

        #endregion
    }
}