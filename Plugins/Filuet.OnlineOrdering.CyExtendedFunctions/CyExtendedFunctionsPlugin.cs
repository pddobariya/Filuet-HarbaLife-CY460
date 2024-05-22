using Filuet.OnlineOrdering.CyExtendedFunctions.Components;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CyExtendedFunctions
{
    public class CyExtendedFunctionsPlugin : BasePlugin, IWidgetPlugin
    {
        #region Filelds
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public CyExtendedFunctionsPlugin(
            IWebHelper webHelper,
            ILocalizationService localizationService
            )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns></returns>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Filuet.OnlineOrdering.CyExtendedFunctions.AddSKU3798ForSKU5451"] = "AddSku",
                ["Filuet.OnlineOrdering.CyExtendedFunctions.Configure"] = "Configure",
                ["Filuet.OnlineOrdering.CyExtendedFunctions.AddSKU3798ForSKU5451.Hint"] = "Enable a AddSku"
            });
            await base.InstallAsync();
        }

        /// <summary>
        /// UnInstall plugin
        /// </summary>
        /// <returns></returns>
        public override async Task UninstallAsync()
        {
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Filuet.OnlineOrdering.CyExtendedFunctions");
            await base.UninstallAsync();

        }

        /// <summary>
        /// Configure page
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/CyExtendedFunctionsAdmin/Configure";
        }

        public bool HideInWidgetList => false;

        /// <summary>
        /// WidgetZone
        /// </summary>
        /// <returns></returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.OpCheckoutPaymentInfoBottom});
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(AcceptConditionsViewComponent);
        }

        #endregion
    }
}
