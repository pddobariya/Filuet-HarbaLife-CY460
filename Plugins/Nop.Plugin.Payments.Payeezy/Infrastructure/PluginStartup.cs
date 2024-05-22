using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Payeezy.ViewEngines;

namespace Nop.Plugin.Payments.Payeezy.Infrastructure
{
    public class PluginStartup : INopStartup
    {
        public int Order => 10;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new PluginPayeezyViewLocationExpander());
            });
            // Note:- Comment on this code to solve the warning.
            //services.AddScoped<IOrderProcessingService, PayeezyOrderProcessingService>();
        }

        public void Configure(IApplicationBuilder application)
        {
            
        }
    }
}
