using Nop.Services.Cms;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions
{
    public class CommonExtendedFunctionsPlugin : BasePlugin, IWidgetPlugin
    {
        #region Methods

        public bool HideInWidgetList => false;
  
        public override async Task InstallAsync()
        {
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
           
            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return null;
        }

        #endregion
    }
}
