using HBL.Cyprus.Onlineordering.Payments.Saferpay.Services;
using HBL.Cyprus.Onlineordering.Payments.Saferpay.ViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;


namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Infrastructure
{
    public class NopStartup : INopStartup
    {
        #region Methods

        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SaferpayViewLocationExpander());
            });

            services.AddScoped<ISaferpayService, SaferpayService>();
        }

        #endregion
    }
}
