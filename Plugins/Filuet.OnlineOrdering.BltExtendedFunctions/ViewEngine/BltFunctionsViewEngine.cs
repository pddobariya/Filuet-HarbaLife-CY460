using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Filuet.OnlineOrdering.BltExtendedFunctions.ViewEngine
{
    public class BltFunctionsViewEngine : IViewLocationExpander
    {
        #region Methods
        private const string THEME_KEY = "nop.themename";
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            context.Values.TryGetValue(THEME_KEY, out string theme);

            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                     $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Views/{{0}}.cshtml",
                     $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Views/{{1}}/{{0}}.cshtml",
                     $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Views/Shared/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                        $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                        $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Filuet.OnlineOrdering.BltExtendedFunctions/Views/Shared/{{0}}.cshtml",
                      }.Concat(viewLocations);
            }
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }

        #endregion
    }
}
