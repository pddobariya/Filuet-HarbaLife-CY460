using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Web.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Filuet.Onlineordering.Shipping.Delivery.ViewEngine
{
    class DeliveryViewLocationExpander : IViewLocationExpander
    {
        #region Methods

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] {
                        $"~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] {
                    $"~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Views/{{1}}/{{0}}.cshtml",
                    $"~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Views/Shared/Components/{{1}}/{{0}}.cshtml",
                    }.Concat(viewLocations);
            }
            return viewLocations;
        }
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;
        }

        #endregion
    }
}
