using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class PluginViewLocationExpander : IViewLocationExpander
    {
        private const string ThemeKey = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.ControllerName == nameof(FiluetCustomerController).Replace("Controller", string.Empty) && context.ViewName == "UserLogin")
            {
                viewLocations = new[]
                {
                    "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/FiluetCustomer/UserLogin.cshtml",
                    "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/{0}.cshtml"
                }.Concat(viewLocations);
            }

            viewLocations = new[] {
                "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/{1}/{0}.cshtml",
                "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/{0}.cshtml"
            }.Concat(viewLocations);

            if (context.AreaName == "Admin" && context.ControllerName == "Customer" && context.ViewName == "List")
            {
                viewLocations = new[]
                    {
                        "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Customer/List.cshtml"
                    }
                    .Concat(viewLocations);
            }
            if (context.AreaName == "Admin" && context.ControllerName == "Product" && context.ViewName == "_CreateOrUpdate.Info")
            {
                viewLocations = new[]
                    {
                        "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Areas/Admin/Views/Product/_CreateOrUpdate.Info.cshtml"
                    }
                    .Concat(viewLocations);
            }
            if (context.ViewName == "Components/NopAjaxCart/NopAjaxCart")
            {
                viewLocations = new[]
                    {
                        "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Components/NopAjaxCart/NopAjaxCart.cshtml"
                    }
                    .Concat(viewLocations);
            }
            if (context.ControllerName == "FiluetCorePlugin" && context.ViewName == "Configure")
            {
                viewLocations = new[]
                    {
                        "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Configure.cshtml"
                    }
                    .Concat(viewLocations);
            }

            if (context.ControllerName == "FiluetOrder" && context.ViewName == "Details")
            {
                viewLocations = new[]
                    {
                        $"~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Order/Details.cshtml"
                    }
                    .Concat(viewLocations);
            }

            if (context.ControllerName == "FiluetShoppingCart")
            {
                viewLocations = new[]
                    {
                        "~/Views/ShoppingCart/{0}.cshtml",
                    }.Concat(viewLocations);
            }

            if (context.ControllerName == "FiluetCheckout")
            {
                viewLocations = new[]
                    {
                        $"~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/{{1}}/{{0}}.cshtml",
                        "~/Views/Checkout/{0}.cshtml",
                    }.Concat(viewLocations);
            }

            if (context.Values.TryGetValue(ThemeKey, out string theme))
            {
                viewLocations = new[] {
                        $"~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                    }
                    .Concat(viewLocations);
            }
            if (context.ControllerName == "QuickViewCatalog")
            {
                viewLocations = new[]
                {
                    "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Product/{0}.cshtml",
                    "~/Areas/Admin/Views/Product/{0}.cshtml"
                }.Concat(viewLocations);
            }
            if (context.ControllerName == "FiluetAdminProduct")
            {
                viewLocations = new[]
                {
                    "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Areas/Admin/Views/Product/{0}.cshtml",
                    "~/Areas/Admin/Views/Product/{0}.cshtml"
                }.Concat(viewLocations);
            }
           
            return viewLocations;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // nothing
        }
    }
}
