using Filuet.OnlineOrdering.CommonExtendedFunctions.ExportImport;
using Filuet.OnlineOrdering.CommonExtendedFunctions.ViewEngine;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.ExportImport;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        #region Methods

        public int Order => 1002;

        public void Configure(IApplicationBuilder application)
        {
            
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICustomProductService, Filuet.OnlineOrdering.CommonExtendedFunctions.Services.CustomProductService>();
            services.AddScoped<IExportManager, FiluetExportManager>();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CommonExtendedViewEngine());
            });
        }

        #endregion
    }
}
