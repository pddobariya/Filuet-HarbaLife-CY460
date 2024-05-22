using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.ViewEngine
{
    public class PriceFilterViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            //context.Values.TryGetValue(THEME_KEY, out var theme);

            if (context.AreaName == "Admin")
                viewLocations = new[] {
                        $"~/Plugins/Catalogue.PriceFilter/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Catalogue.PriceFilter/Views/Shared/{{0}}.cshtml"
                    }.Concat(viewLocations);
            else
                viewLocations = new[] {
                        $"~/Plugins/Catalogue.PriceFilter/Views//{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Catalogue.PriceFilter/Views/Shared/{{0}}.cshtml"
                    }.Concat(viewLocations);

            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
         
        }
    }
}
