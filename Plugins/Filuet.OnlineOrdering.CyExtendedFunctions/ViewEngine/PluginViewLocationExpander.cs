using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Filuet.OnlineOrdering.CyExtendedFunctions.ViewEngine
{
    public class PluginViewLocationExpander : IViewLocationExpander
    {
        #region Methods
        private const string THEME_KEY = "nop.themename";
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            context.Values.TryGetValue(THEME_KEY, out string theme);

            if (context.AreaName == "Admin")
            {
                viewLocations = new[]{
                     $"~/Plugins/Filuet.OnlineOrdering.CyExtendedFunctions/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[]{
                        $"~/Plugins/Filuet.OnlineOrdering.CyExtendedFunctions/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Filuet.OnlineOrdering.CyExtendedFunctions/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                        $"~/Plugins/Filuet.OnlineOrdering.CyExtendedFunctions/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Filuet.OnlineOrdering.CyExtendedFunctions/Views/Shared/{{0}}.cshtml",
                }.Concat(viewLocations);

            }
            return viewLocations;
        }

        public async void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;

            context.Values[THEME_KEY] = await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();
        }
        #endregion
    }
}
