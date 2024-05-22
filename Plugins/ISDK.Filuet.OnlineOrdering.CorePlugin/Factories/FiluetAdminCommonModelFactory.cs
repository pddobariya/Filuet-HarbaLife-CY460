using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    public class FiluetAdminCommonModelFactory : CommonModelFactory
    {
        #region Fields

        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public FiluetAdminCommonModelFactory(
            AppSettings appSettings,
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationPluginManager authenticationPluginManager,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICurrencyService currencyService,
            ICustomerService customerService, 
            IEventPublisher eventPublisher,
            INopDataProvider dataProvider, 
            IDateTimeHelper dateTimeHelper,
            INopFileProvider fileProvider,
            IExchangeRatePluginManager exchangeRatePluginManager,
            IHttpContextAccessor httpContextAccessor,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMaintenanceService maintenanceService,
            IMeasureService measureService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            IOrderService orderService, 
            IPaymentPluginManager paymentPluginManager,
            IPickupPluginManager pickupPluginManager,
            IPluginService pluginService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            ISearchTermService searchTermService,
            IServiceCollection serviceCollection,
            IShippingPluginManager shippingPluginManager, 
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext, 
            IStoreService storeService,
            ITaxPluginManager taxPluginManager,
            IUrlHelperFactory urlHelperFactory, 
            IUrlRecordService urlRecordService, 
            IWebHelper webHelper, 
            IWidgetPluginManager widgetPluginManager,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            NopHttpClient nopHttpClient,
            ProxySettings proxySettings)
            : base(appSettings,
                  catalogSettings,
                  currencySettings, 
                  actionContextAccessor,
                  authenticationPluginManager,
                  baseAdminModelFactory,
                  currencyService,
                  customerService,
                  eventPublisher, 
                  dataProvider,
                  dateTimeHelper,
                  fileProvider, 
                  exchangeRatePluginManager,
                  httpContextAccessor,
                  languageService,
                  localizationService,
                  maintenanceService, 
                  measureService,
                  multiFactorAuthenticationPluginManager,
                  orderService,
                  paymentPluginManager, 
                  pickupPluginManager,
                  pluginService, 
                  productService, 
                  returnRequestService,
                  searchTermService,
                  serviceCollection,
                  shippingPluginManager,
                  staticCacheManager,
                  storeContext,
                  storeService,
                  taxPluginManager,
                  urlHelperFactory,
                  urlRecordService,
                  webHelper,
                  widgetPluginManager, 
                  workContext, 
                  measureSettings,
                  nopHttpClient,
                  proxySettings)
        {
            _orderService = orderService;
        }

        #endregion

        #region Methods

        public override async Task<CommonStatisticsModel> PrepareCommonStatisticsModelAsync()
        {
            var osIds = new List<int>();
            osIds.Add((int)OrderStatus.Complete);
            osIds.Add((int)OrderStatus.Processing);
            var model = await base.PrepareCommonStatisticsModelAsync();
            model.NumberOfOrders =
                (await _orderService.SearchOrdersAsync(pageIndex: 0, pageSize: 1, getOnlyTotalCount: true, osIds: osIds))
                .TotalCount;
            return model;
        }

        #endregion
    }
}
