﻿using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;
using System.Collections.Generic;
using System.Linq;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.ViewEngine
{
    public class SaferpayViewLocationExpander : IViewLocationExpander
    {
        #region Methods

        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            context.Values.TryGetValue(THEME_KEY, out var theme);

            if (context.AreaName == "Admin")
                viewLocations = new[] {
                        $"~/Plugins/Payments.Saferpay/Area/Admin/Views/{{1}}/{{0}}.cshtml",
                    }.Concat(viewLocations);
            else
                viewLocations = new[] {
                        $"~/Plugins/Payments.Saferpay/Views/Shared/{{0}}.cshtml",
                        $"~/Plugins/Payments.Saferpay/Content/Themes/{theme}/Area/Admin/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Payments.Saferpay/Content/Themes/{theme}/Views/Shared/Components/{{0}}.cshtml"
                    }.Concat(viewLocations);

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