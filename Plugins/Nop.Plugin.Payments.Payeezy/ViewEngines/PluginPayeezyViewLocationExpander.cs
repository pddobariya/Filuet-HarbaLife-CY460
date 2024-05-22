using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Payments.Payeezy.ViewEngines
{
    public class PluginPayeezyViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                     $"~/Plugins/Payments.Payeezy/Views/{{0}}.cshtml",
                     $"~/Plugins/Payments.Payeezy/Views/{{1}}/{{0}}.cshtml",
                     $"~/Plugins/Payments.Payeezy/Views/Shared/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                           $"~/Plugins/Payments.Payeezy/Views/{{1}}/{{0}}.cshtml",
                          $"~/Plugins/Payments.Payeezy/Views/Shared/{{0}}.cshtml",
                      }.Concat(viewLocations);
            }
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
           
        }
    }
}
