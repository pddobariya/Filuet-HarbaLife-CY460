using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Factories;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.ViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Factories;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 1;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new PriceFilterViewLocationExpander());
            });

            services.AddScoped<ICatalogModelFactory,FiluetCatalogModelFactory>();
            services.AddScoped<PriceRangeFilterService>();
            services.AddScoped<PriceFilterModelFactory>();
            services.AddScoped<FiluetCatalogModelFactory>();
        }
    }
}
