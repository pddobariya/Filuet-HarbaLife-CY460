using ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Routing;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CyExtendedFunctions.Controllers
{
    public class CyExtendedFunctionsController : FiluetShoppingCartController
    {
        #region Fields

        public static string Sku3798AddedAttribute = "Sku3798Added";
        public static string Sku3798IdAttribute = "Sku5451Id";

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IShoppingCartService _shoppingCartService;
        protected readonly IRepository<Product> _productRepository;
        private readonly IRepository<ShoppingCartItem> _sciRepository;

        #endregion

        #region Ctor

        public CyExtendedFunctionsController(IRepository<Product> productRepository,
            ISettingService settingService,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            INopUrlHelper nopUrlHelper,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings,
            IPriceCalculationService priceCalculationService,
            ILogger logger,
            IStaticCacheManager cacheManager,
            FusionValidationService fusionValidationService,
            IFiluetShippingCartService filuetShippingCartService,
            IOrderService orderService,
            IDualMonthsService dualMonthsService,
            FiluetCorePluginSettings filuetCorePluginSettings,
            IDistributorService distributorService,
            IRepository<ShoppingCartItem> sciRepository)
            : base(captchaSettings,
                  customerSettings,
                  checkoutAttributeParser,
                  checkoutAttributeService,
                  currencyService,
                  customerActivityService,
                  customerService,
                  discountService,
                  downloadService,
                  genericAttributeService,
                  giftCardService,
                  htmlFormatter,
                  localizationService,
                  fileProvider,
                  nopUrlHelper,
                  notificationService,
                  permissionService,
                  pictureService,
                  priceFormatter,
                  productAttributeParser,
                  productAttributeService,
                  productService,
                  shippingService,
                  shoppingCartModelFactory,
                  shoppingCartService,
                  staticCacheManager,
                  storeContext,
                  taxService,
                  urlRecordService,
                  webHelper,
                  workContext,
                  workflowMessageService,
                  mediaSettings,
                  orderSettings,
                  shoppingCartSettings,
                  shippingSettings,
                  priceCalculationService,
                  logger,
                  cacheManager,
                  fusionValidationService,
                  filuetShippingCartService,
                  orderService,
                  dualMonthsService,
                  filuetCorePluginSettings,
                  distributorService)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _logger = logger;
            _settingService = settingService;
            _productRepository = productRepository;
            _sciRepository = sciRepository;
        }

        #endregion

        #region Method

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public override async Task<IActionResult> StartCheckout(IFormCollection form)
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var cyExtendedFunctionsSettings = await _settingService.LoadSettingAsync<CyExtendedFunctionsSettings>(storeScope);
            if (cyExtendedFunctionsSettings.AddSKU3798ForSKU5451)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var customer = await _workContext.GetCurrentCustomerAsync();
                var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                var sku3798Ids = (from s in shoppingCarts
                                  join p in _productRepository.Table
                                  on s.ProductId equals p.Id
                                  where p.Sku == "3798"
                                  select s.Id).ToArray();
                var shoppingCartItemsSku5451 = (from s in shoppingCarts
                                                join p in _productRepository.Table
                                                on s.ProductId equals p.Id
                                                where p.Sku == "5451" && (! _genericAttributeService.GetAttributeAsync<bool>(s, Sku3798AddedAttribute).Result || sku3798Ids.All(id => _genericAttributeService.GetAttributeAsync<long>(s, Sku3798IdAttribute).Result != id))
                                                select s).ToArray();
                var quantityOfSku5451 =  shoppingCartItemsSku5451
                    .Aggregate(0, (quantity, sci) => sci.Quantity + quantity);
                if (quantityOfSku5451 > 0)
                {
                    var shoppingCartItemSku3798 = (from s in shoppingCarts
                                                   join p in _productRepository.Table
                                                   on s.ProductId equals p.Id
                                                   where p.Sku == "3798"
                                                   select s).FirstOrDefault();
                    if (shoppingCartItemSku3798 == null)
                    {
                        var productSku3798 = await _productService.GetProductBySkuAsync("3798");
                        if (productSku3798 == null)
                        {
                            await _logger.InsertLogAsync(LogLevel.Error, "Sku 3798 is absent");
                        }
                        else
                        {
                            DateTime now = DateTime.UtcNow;
                            shoppingCartItemSku3798 = new  ShoppingCartItem
                            {
                                ShoppingCartType = ShoppingCartType.ShoppingCart,
                                StoreId = store.Id,
                                ProductId = productSku3798.Id,
                                Quantity = quantityOfSku5451,
                                CreatedOnUtc = now,
                                UpdatedOnUtc = now,
                                CustomerId = customer.Id
                            };
                            await _sciRepository.InsertAsync(shoppingCartItemSku3798);
                            // shoppingCarts.Add(shoppingCartItemSku3798);
                            await _customerService.UpdateCustomerAsync(customer);
                            foreach (var shoppingCartItem in shoppingCartItemsSku5451)
                            {
                                await _genericAttributeService.SaveAttributeAsync(shoppingCartItem, Sku3798AddedAttribute, true);
                                await _genericAttributeService.SaveAttributeAsync(shoppingCartItem, Sku3798IdAttribute, shoppingCartItemSku3798.Id);
                            }
                        }
                    }
                    else
                    {
                        shoppingCartItemSku3798.Quantity += quantityOfSku5451;
                        await _customerService.UpdateCustomerAsync(await _workContext.GetCurrentCustomerAsync());
                        foreach (var shoppingCartItem in shoppingCartItemsSku5451)
                        {
                            await _genericAttributeService.SaveAttributeAsync(shoppingCartItem, Sku3798AddedAttribute, true);
                            await _genericAttributeService.SaveAttributeAsync(shoppingCartItem, Sku3798IdAttribute, shoppingCartItemSku3798.Id);
                        }
                    }
                }
            }
            return await base.StartCheckout(form);
        }

        #endregion
    }
}
