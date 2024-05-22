using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Helpers;
using ISDK.Filuet.Theme.FiluetHerbalife.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Factories;
using System.IO;
using IBlogModelFactory = Nop.Web.Areas.Admin.Factories.IBlogModelFactory;
using IProductModelFactory = Nop.Web.Areas.Admin.Factories.IProductModelFactory;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure
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

            #region Register Factories admin

            // Factories admin
            services.AddScoped<IBlogModelFactory, FiluetBlogModelFactory>();
            services.AddScoped<ICategoryModelFactory, FiluetCategoryModelFactory>();
            services.AddScoped<IProductModelFactory, Areas.Admin.Factories.FiluetProductModelFactory>();

            #endregion

            #region Register Fectories front-side

            //Fectories front side
            services.AddScoped<IFiluetCartModelFactory, FiluetCartModelFactory>();
            services.AddScoped<IFiluetCatalogModelFactory, FiluetCatalogModelFactory>();
            services.AddScoped<ICatalogModelFactory, FiluetCatalogModelFactory>();
            services.AddScoped<INewsBlockModelFactory, NewsBlockModelFactory>();
            services.AddScoped<Nop.Web.Factories.ICommonModelFactory, FiluetHerbalifeCommonModelFactory>();
            services.AddScoped<ICustomCheckoutModelFactory, CustomCheckoutModelFactory>();
            services.AddScoped<Nop.Web.Factories.IProductModelFactory, Factories.FiluetProductModelFactory>();

            #endregion

            #region Service

            // Services
            services.AddScoped<ICartValidatorService, CartValidatorService>();

            #endregion

            #region Theme

            //themes support
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new PluginViewLocationExpander());
            });

            #endregion

            services.AddScoped<FaqDataInitHelper>();
            services.AddScoped<HtmlConvertHelpers>();

        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => int.MaxValue;

        #endregion
    }
}
