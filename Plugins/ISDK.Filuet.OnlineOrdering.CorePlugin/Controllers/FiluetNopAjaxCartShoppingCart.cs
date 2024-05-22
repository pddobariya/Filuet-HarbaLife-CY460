using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
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
using Nop.Web.Framework.Mvc.Routing;
using NUglify.Helpers;
using SevenSpikes.Nop.Plugins.AjaxCart.Controllers;
using SevenSpikes.Nop.Plugins.AjaxCart.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    public class FiluetNopAjaxCartShoppingCart : NopAjaxCartShoppingCartController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ICategoryChecker _categoryChecker;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FiluetNopAjaxCartShoppingCart(NopAjaxCartSettings ajaxCartSettings,
            IProductAttributeFormatter productAttributeFormatter,
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
            ILogger logger, 
            ICategoryChecker categoryChecker) 
            : base(ajaxCartSettings,
                  productAttributeFormatter, 
                  captchaSettings,
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
                  shippingSettings)
        {
            _customerService = customerService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _categoryChecker = categoryChecker;
            _logger = logger;
            _categoryChecker = categoryChecker;
        }

        #endregion

        #region Methods

        public async Task<JsonResult> FiluetAddProductFromProductDetailsPageToCartAjax(
            int productId,
            bool isAddToCartButton,
            IFormCollection form)
        {
            
            var result =await _categoryChecker.CheckProductsCompatibilityWithMessageAsync(productId);
            if (result != null)
            {
                return await Task.FromResult(result);
            }
            ControllerContext.RouteData.Values["controller"] = "NopAjaxCartShoppingCart";
            return await base.AddProductFromProductDetailsPageToCartAjax(productId, isAddToCartButton, form);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> FiluetAddProductToCartAjax(
            int productId,
            int quantity,
            bool isAddToCartButton)
        {
            var result =await _categoryChecker.CheckProductsCompatibilityWithMessageAsync(productId);
            if (result != null)
            {
                return await Task.FromResult(result);
            }
            ControllerContext.RouteData.Values["controller"] = "NopAjaxCartShoppingCart";
            return await base.AddProductToCartAjax(productId, quantity, isAddToCartButton);
        }

        public async Task<IActionResult> AddBizWorkToCart()
        {
            var replaced =await BizWorkShoppingCartHelper.ReplaceShoppingCartWithBizWorkItemAsync(await _workContext.GetCurrentCustomerAsync());
            if (!replaced)
               await _logger.ErrorAsync("BizWork product is missed.");
            return Ok();
        }

        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ClearCartAndAddProduct(
            int productId,
            int quantity,
            bool isAddToCartButton)
        {
            ClearCart();
            return await FiluetAddProductToCartAjax(productId, quantity, isAddToCartButton);
        }

        public async Task<IActionResult> ClearCartAndAddProductFromProductDetailsPage(
            int productId,
            bool isAddToCartButton,
            IFormCollection form)
        {
            ClearCart();
            return await FiluetAddProductFromProductDetailsPageToCartAjax(productId, isAddToCartButton, form);
        }

        private async void ClearCart()
        {
            var currentCustomer =await _workContext.GetCurrentCustomerAsync();
            (await _shoppingCartService.GetShoppingCartAsync(currentCustomer)).ForEach(async sci =>await _shoppingCartService.DeleteShoppingCartItemAsync(sci));
            await _customerService.ResetCheckoutDataAsync(currentCustomer,(await _storeContext.GetCurrentStoreAsync()).Id);
            await _customerService.UpdateCustomerAsync(currentCustomer);
        }

        private async Task<JsonResult> CheckBizWork(int productId)
        {
            var currentCustomer =await _workContext.GetCurrentCustomerAsync();
            var bizWorkProduct =await BizWorkShoppingCartHelper.GetBizWorkProductAsync(currentCustomer);
            if (bizWorkProduct == null)
            {
                return null;
                //throw new Exception("bizWorkProduct not found");
            }
            if (await (await _shoppingCartService.GetShoppingCartAsync(currentCustomer)).AnyAwaitAsync(async sci => (await _productService.GetProductByIdAsync(sci.ProductId)) == bizWorkProduct) && await _productService.GetProductByIdAsync(productId) != bizWorkProduct)
            {
                var model = new AddProductToCartResultModel();
                model.Status = "frombizwork";
                model.PopupTitle = "Добавить в корзину не удалось из-за следующих предупреждений";
                model.AddToCartWarnings =
                    "В вашей корзине на данный момент находится bizwork. Если вы хотите добавить в корзину выбранный товар, то подписка bizwork будет удалена из корзины. Вы хотите продолжить покупки и удалить bizwork?";
                return Json(model);
            }
            if (await(await _shoppingCartService.GetShoppingCartAsync(currentCustomer)).AnyAwaitAsync(async sci =>await _productService.GetProductByIdAsync(productId) != bizWorkProduct) && await _productService.GetProductByIdAsync(productId) == bizWorkProduct)
            {
                var model = new AddProductToCartResultModel();
                model.Status = "bizwork";
                model.PopupTitle = "Добавить в корзину не удалось из-за следующих предупреждений";
                model.AddToCartWarnings =
                    "В вашей корзине в данный момент находятся товары. При оформлении bizwork текущие товары в корзине будут удалены. Вы хотите  обнулить корзину и добавить bizwork?";
                return Json(model);
            }
            return null;
        }

        #endregion
    }
}
