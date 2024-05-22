using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.ViewEngines
{
    public class CustomViewEngine : IViewLocationExpander
    {
        #region Methods

        private const string THEME_KEY = "nop.themename";

        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine the
        /// values that would be consumed by this instance of Microsoft.AspNetCore.Mvc.Razor.IViewLocationExpander.
        /// The calculated values are used to determine if the view location has changed since the last time it was located.
        /// </summary>
        /// <param name="context">Context</param>
        public async void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;

            context.Values[THEME_KEY] = await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();
        }

        /// <summary>
        /// Invoked by a Microsoft.AspNetCore.Mvc.Razor.RazorViewEngine to determine potential locations for a view.
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="viewLocations">View locations</param>
        /// <returns>iew locations</returns>
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            context.Values.TryGetValue(THEME_KEY, out string theme);
            if (context.AreaName == "Admin")
            {
                viewLocations = new[]
                {
                    $"~/Plugins/ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin/Views/{{1}}/{{0}}.cshtml"

                }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[]{
                        $"~/Plugins/ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);
            }

            return viewLocations;
        }

        #endregion

    }
}
