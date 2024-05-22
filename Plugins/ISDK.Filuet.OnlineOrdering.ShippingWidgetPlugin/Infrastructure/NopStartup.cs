using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.ViewEngines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using System.IO;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure
{
    public class NopStartup : INopStartup
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
            //themes support
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new CustomViewEngine());
            });
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;

        #endregion
    }
}
