using ISDK.Filuet.OnlineOrdering.CorePlugin.Factories;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Filters;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Fusion;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Payment;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Security;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Setting;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.ShoppingCart;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Configuration;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Factories;
using System;
using System.IO;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        #region Methods

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //register for Response.Write 
            application.Use(async (context, next) =>
            {
                // Create a backup of the original response stream
                var backup = context.Response.Body;
                try
                {
                    using (var customStream = new MemoryStream())
                    {
                        // Assign readable/writeable stream
                        context.Response.Body = customStream;

                        await next();
                        customStream.Position = 0;

                        var content = await new StreamReader(customStream).ReadToEndAsync();
                        customStream.Position = 0;
                        // Write custom content to response
                        await customStream.CopyToAsync(backup);
                    }
                }

                finally
                {
                    context.Response.Body = backup;
                }
            });
        }

        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new PluginViewLocationExpander());
            });

            services.AddScoped<IMessageTokenProvider, ExtendedMessageTokenProvider>();

            services.AddScoped<IShoppingCartModelFactory, FiluetTotalsShoppingCartModelFactory>();
            services.AddScoped<IProductModelFactory, FiluetProductModelFactory>();
            services.AddScoped<ICustomerModelFactory, FiluetCustomerModelFactory>();
            services.AddScoped<IFiluetCustomerModelFactory, FiluetCustomerModelFactory>();
            services.AddScoped<ITopicModelFactory, FiluetTopicModelFactory>();
            services.AddScoped<IFiluetCustomerService, FiluetCustomerService>();
            services.AddScoped<ICustomerRegistrationService, FiluetCustomerRegistrationService>();
            services.AddScoped<IPluginSettingsService, PluginSettingsService>();
            services.AddScoped<IFusionIntegrationService, FusionIntegrationService>();
            services.AddScoped<IFiluetPaymentService, FiluetPaymentService>();
            services.AddScoped<FusionValidationService>();
            services.AddScoped<IFilterProvider, AdminLoadOrderStatisticsActionFilter>();
            services.AddScoped<IPermissionService, FiluetPermissionService>();
            services.AddScoped<Nop.Web.Areas.Admin.Factories.IProductModelFactory, FiluetAdminProductModelFactory>();
            services.AddScoped<IProductService, FiluetProductService>();
            services.AddScoped<Nop.Web.Areas.Admin.Factories.ICustomerModelFactory, FiluetAdminCustomerModelFactory>();
            services.AddScoped<IFiluetOrderModelFactory, FiluetOrderModelFactory>();
            services.AddScoped<IFiluetOrderService, FiluetOrderService>();
            services.AddScoped<_1SServiceWrapper>();
            services.AddScoped<Nop.Web.Areas.Admin.Factories.ICommonModelFactory, FiluetAdminCommonModelFactory>();
            services.AddScoped<ILogger, FiluetLogger>();
            services.AddScoped<ICartValidator, CartValidator>();
            services.AddScoped<IOrderStatusService, OrderStatusService>();
            services.AddScoped<IStatusService, StatusService>();
            services.AddScoped<IFiluetShoppingCartService, FiluetShoppingCartService>();


            var aiOptions = new Microsoft.ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
            {
                EnableAdaptiveSampling = false
            };
            services.AddApplicationInsightsTelemetry(aiOptions);

            //add configuration parameters
            var appSettings = new FiluetConfig();
            configuration.GetSection("Filuet").Bind(appSettings);
            services.AddSingleton(appSettings);

            services.AddMvc(options =>
            {
                options.Filters.Add(new ApfCheckActionFilter());
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = CrmDataProvider.SessionName;
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.IsEssential = true;
            });

            services.AddHealthChecks()
                .AddCheck<FiluetHealthCheck>("filuet_health_check");

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;

        #endregion
    }
}
