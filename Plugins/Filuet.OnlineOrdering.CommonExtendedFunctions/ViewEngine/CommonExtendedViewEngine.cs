using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions.ViewEngine
{
    public class CommonExtendedViewEngine : IViewLocationExpander
    {
        #region Methods

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.ControllerName == "FiluetCustomer" && context.ViewName == "UserLogin")
            {
                viewLocations = new[]
                {
                    "~/Plugins/Filuet.OnlineOrdering.CommonExtendedFunctions/Views/{0}.cshtml",
                }.Concat(viewLocations);
            }

            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                     $"~/Plugins/Filuet.OnlineOrdering.CommonExtendedFunctions/Views/{{0}}.cshtml",
                     $"~/Plugins/Filuet.OnlineOrdering.CommonExtendedFunctions/Views/{{1}}/{{0}}.cshtml",
                     $"~/Plugins/Filuet.OnlineOrdering.CommonExtendedFunctions/Views/Shared/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                           $"~/Plugins/Filuet.OnlineOrdering.CommonExtendedFunctions/Views/{{1}}/{{0}}.cshtml",
                          $"~/Plugins/Filuet.OnlineOrdering.CommonExtendedFunctions/Views/Shared/{{0}}.cshtml",
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
