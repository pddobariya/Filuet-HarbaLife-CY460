using Filuet.OnlineOrdering.CyExtendedFunctions.Controllers;
using Filuet.OnlineOrdering.CyExtendedFunctions.Services;
using Filuet.OnlineOrdering.CyExtendedFunctions.ViewEngine;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.ShoppingCart;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Orders;
using Nop.Web.Controllers;

namespace Filuet.OnlineOrdering.CyExtendedFunctions.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {

        public int Order => int.MaxValue;
        #region Ctor
        public void Configure(IApplicationBuilder application)
        {

        }
        #endregion

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            #region Resigster Service

            services.AddScoped<IShoppingCartService, CyFiluetShoppingCartService>();

            #endregion

            #region Resigster ViewEnegine

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new PluginViewLocationExpander());
            });
            services.AddScoped<FiluetShoppingCartController, CyExtendedFunctionsController>();

            #endregion
        }
    }
}
