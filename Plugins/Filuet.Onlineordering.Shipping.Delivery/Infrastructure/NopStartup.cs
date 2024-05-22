using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Factories;
using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Services;
using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Validators;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using Filuet.Onlineordering.Shipping.Delivery.ViewEngine;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Filuet.Onlineordering.Shipping.Delivery.Infrastructure
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
                options.ViewLocationExpanders.Add(new DeliveryViewLocationExpander());
            });
            services.AddScoped<IDeliveryPriceService, DeliveryPriceService>();
            services.AddScoped<IDeliveryOperatorService,DeliveryOperatorService>();
            services.AddScoped<IDeliveryCityService,DeliveryCityService>();
            services.AddScoped<IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService, DeliveryOperator_DeliveryType_DeliveryCity_DependenciesService>();
            services.AddScoped<IAutoPostOfficeService, AutoPostOfficeService>();
            services.AddScoped<IPhoneFormatter,PhoneFormatter>();
            services.AddScoped<ISalesCenterService,SalesCenterService>();
            services.AddScoped<ICountryDeliveryCustomizingService, CountryDeliveryCustomizingService>();
            services.AddScoped<IDeliveryTypeService,DeliveryTypeService>();
            services.AddScoped<ISalesCenterModelFactory, SalesCenterModelFactory>();
            services.AddScoped<IDeliveryTypeModelFactory, DeliveryTypeModelFactory>();
            services.AddScoped<ICountryDeliveryCustomizingService, CountryDeliveryCustomizingService>();
            services.AddScoped<ISalesCentersService, SalesCentersService>();

            services.AddTransient<IValidator<SalesCenterDtoModel>, SalesCenterValidator>();
            services.AddTransient<IValidator<DeliveryOperatorDtoModel>, DeliveryOperatorValidator>();
            services.AddTransient<IValidator<DeliveryOperatorsCityModel>, DeliveryOperatorsCityValidator>();
            services.AddTransient<IValidator<AutoPostOfficeDtoModel>, AutoPostOfficeValidator>();
        }

        #endregion
    }
}
