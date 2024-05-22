using ISDK.Filuet.ExternalSSOAuthPlugin.Services;
using ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts;
using ISDK.Filuet.ExternalSSOAuthPlugin.viewEngine;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        #region Methods

        public const string UserUpdatedDate = "UserUpdatedDate";
        public int Order => 999;

        public void Configure(IApplicationBuilder application)
        {
            application.UseMiddleware<FiluetCoreMiddleware>();
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SsoAuthViewLocationExpander());
            });
            services.AddScoped<ICrmDataProvider>(provider =>
                {
                    return new CrmDataProvider(provider.GetService<SSOAuthPluginSettings>().ApiUrl,
                        provider.GetService<IHttpContextAccessor>());
                });

            services.AddScoped<IWhitelistedCountryService, WhitelistedCountryService>();
            services.AddScoped<IResidentChecker, ResidentChecker>();
            services.AddScoped<IExternalAuthenticationService, FiluetExternalAuthenticationService>();

            services.AddScoped<ICrmDataProviderAdapter, CrmDataProviderAdapter>();
            services.AddScoped<IDistributorRestrictionService, DistributorRestrictionService>();
        }

        #endregion
    }
}
