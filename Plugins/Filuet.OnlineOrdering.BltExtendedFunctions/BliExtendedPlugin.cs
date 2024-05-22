using Filuet.OnlineOrdering.BltExtendedFunctions.Components;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Constants;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.BltExtendedFunctions
{
    public class BliExtendedPlugin : BasePlugin, IWidgetPlugin
    {
        #region Methods

        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.OrderSummaryContentBefore)
            {
                return typeof(CustomOrderCardMessageViewComponent);
            }
            else if (widgetZone == FiluetPublicWidgetZones.AfterTotalInfoOrderDetails)
            {
                return typeof(AfterTotalInfoOrderDetailsMessageViewComponent);
            }
            return null;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.OrderSummaryContentBefore, FiluetPublicWidgetZones.AfterTotalInfoOrderDetails });
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        #endregion
    }
}
