using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Configuration;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Orders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.ShoppingCart;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Orders;
using IShippingModelFactory = ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Factory.IShippingModelFactory;
using ShippingModelFactory = ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Factory.ShippingModelFactory;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => int.MaxValue;

        public void Configure(IApplicationBuilder application)
        {
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //Ragister Service
            services.AddScoped<IAuthorizationProxyService, AuthorizationProxyService>();
            services.AddScoped<IShoppingCartProxyService, ShoppingCartProxyService>();
            services.AddScoped<IStockBalanceProxyService, StockBalanceProxyService>();
            services.AddScoped<IDistributorBehaviorService, DistributorBehaviorService>();
            services.AddScoped<ISettingsLoader, SettingsLoader>();
            services.AddScoped<ICategoryChecker, CategoryChecker>();
            services.AddScoped<ICategoryChecker, CyCategoryChecker>();
            services.AddScoped<IDistributorService, DistributorService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICustomOrderService, CustomOrderService>();
            services.AddScoped<IDualMonthsService, DualMonthsService>();
            services.AddScoped<IFusionIntegrationService, FusionIntegrationService>();
            services.AddScoped<IFusionPresets, FusionPresets>();
            services.AddScoped<IRestApiClient, RestApiClient>();
            services.AddScoped<IFiluetShippingService, FiluetShippingService>();
            services.AddScoped<IShippingWidgetService, ShippingWidgetService>();
            services.AddScoped<IShoppingCartService, FiluetShoppingCartService>();
            services.AddScoped<IDefaultShippingSettingsService, DefaultShippingSettingsService>();
            services.AddScoped<IDistributorBehaviorService, DistributorBehaviorService>();
            services.AddScoped<IHerbalifeEnvironment, HerbalifeEnvironment>();
            services.AddScoped<IPromotionWoker, PromotionWoker>();
            services.AddScoped<ISettingsLoader, SettingsLoader>();
            services.AddScoped<ISpRoleChecker, SpRoleChecker>();
            services.AddScoped<IOrderProcessingService, FusionOrderProcessingService>();
            services.AddScoped<FiluetConfig>();
            services.AddScoped<IWmsDataProvider, WmsDataProvider>();
            services.AddScoped<FusionValidationService>();
            services.AddScoped<IFiluetOrderService, FiluetOrderService>();
            services.AddScoped<IGeoIpHelper, GeoIpHelper>();
            services.AddScoped<IFiluetShippingCartService, FiluetShippingCartService>();
            services.AddScoped<ICustomProductService, CustomProductService>();
            services.AddScoped<IGeoIpHelper,GeoIpHelper>();

            //Register Factory
            services.AddScoped<IShippingModelFactory, ShippingModelFactory>();
        }
    }
}
