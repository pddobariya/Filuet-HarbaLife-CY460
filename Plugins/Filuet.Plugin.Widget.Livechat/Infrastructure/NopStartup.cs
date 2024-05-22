using Filuet.Plugin.Widget.Livechat.ViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;


namespace Filuet.Plugin.Widget.Livechat.Infrastructure
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
                options.ViewLocationExpanders.Add(new LivechatViewLocationExpander());
            });
        }

        #endregion
    }
}
