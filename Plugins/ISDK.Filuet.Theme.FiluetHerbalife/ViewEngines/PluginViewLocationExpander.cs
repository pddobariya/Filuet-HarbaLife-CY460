using Microsoft.AspNetCore.Mvc.Razor;
using Nop.Core.Infrastructure;
using Nop.Web.Framework;
using Nop.Web.Framework.Themes;
using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure
{
    public class PluginViewLocationExpander : IViewLocationExpander
    {
        #region Methods

        private const string THEME_KEY = "nop.themename";

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.Values.TryGetValue(THEME_KEY, out string theme))
            {
                viewLocations = new[] {
                        $"~/Plugins/Theme.FiluetHerbalife/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Theme.FiluetHerbalife/Themes/{theme}/Views/{{0}}.cshtml",
                        $"~/Plugins/Theme.FiluetHerbalife/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                        $"~/Plugins/Theme.FiluetHerbalife/Views/{{1}}/{{0}}.cshtml",
                        $"~/Plugins/Theme.FiluetHerbalife/Views/{{0}}.cshtml",
                        $"~/Plugins/Theme.FiluetHerbalife/Views/Shared/{{0}}.cshtml",
                        $"~/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"~/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                    }
                    .Concat(viewLocations);
            }

            if (context.AreaName == "Admin")
                viewLocations = new[] {
                    $"~/Plugins/Theme.FiluetHerbalife/Areas/Admin/Views/{{1}}/{{0}}.cshtml",
                    $"~/Plugins/Theme.FiluetHerbalife/Areas/Admin/Views/{{0}}.cshtml",
                    $"~/Plugins/Theme.FiluetHerbalife/Areas/Admin/Views/Shared/{{0}}.cshtml"
                }.Concat(viewLocations);

            return viewLocations;
        }

        public async void PopulateValues(ViewLocationExpanderContext context)
        {
            //no need to add the themeable view locations at all as the administration should not be themeable anyway
            if (context.AreaName?.Equals(AreaNames.Admin) ?? false)
                return;

            context.Values[THEME_KEY] = await EngineContext.Current.Resolve<IThemeContext>().GetWorkingThemeNameAsync();
        }

        #endregion

    }
}
