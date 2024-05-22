using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.viewEngine
{
    public class SsoAuthViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                        $"~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Areas/Admin/Views/{{0}}.cshtml",
                        $"~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                    $"~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Views/{{1}}/{{0}}.cshtml",
                    $"~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Views/Shared/Components/{{1}}/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            
        }
    }
}
