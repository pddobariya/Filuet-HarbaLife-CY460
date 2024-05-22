using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Controllers
{
    public class FiluetHerbalifePublicController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FiluetHerbalifePublicController(
            ILocalizationService localizationService,
            CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IOrderService orderService,
            IProductService productService,
            IReviewTypeService reviewTypeService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            IProductModelFactory productModelFactory,
            IShoppingCartService shoppingCartService, 
            IFusionIntegrationService fusionIntegrationService,
            ILogger logger)
        {
            _localizationService = localizationService;
            _catalogSettings = catalogSettings;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _orderService = orderService;
            _productService = productService;
            _reviewTypeService = reviewTypeService;
            _storeContext = storeContext;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _productModelFactory = productModelFactory;
            _shoppingCartService = shoppingCartService;
            _fusionIntegrationService = fusionIntegrationService;
            _logger = logger;
        }

        #endregion

        #region Utilities

        protected virtual async Task ValidateProductReviewAvailabilityAsync(Product product)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            if (await _customerService.IsGuestAsync(customer) && !_catalogSettings.AllowAnonymousUsersToReviewProduct)
                ModelState.AddModelError(string.Empty, await _localizationService.GetResourceAsync("Reviews.OnlyRegisteredUsersCanWriteReviews"));

            if (!_catalogSettings.ProductReviewPossibleOnlyAfterPurchasing)
                return;

            var hasCompletedOrders = product.ProductType == ProductType.SimpleProduct
                ? await HasCompletedOrdersAsync(product)
                : await (await _productService.GetAssociatedProductsAsync(product.Id)).AnyAwaitAsync(HasCompletedOrdersAsync);

            if (!hasCompletedOrders)
                ModelState.AddModelError("ProductReviewPossibleOnlyAfterPurchasing", await _localizationService.GetResourceAsync("Reviews.ProductReviewPossibleOnlyAfterPurchasing"));
        }

        protected virtual async ValueTask<bool> HasCompletedOrdersAsync(Product product)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            return (await _orderService.SearchOrdersAsync(customerId: customer.Id,
                productId: product.Id,
                osIds: new List<int> { (int)OrderStatus.Complete },
                pageSize: 1)).Any();
        }

        #endregion

        #region Methods


        public IActionResult GetCartSummaryBarViewComponent()
        {
            return ViewComponent("CartSummaryBar");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ProductReviewsAdd(ProductReviewsModel model)
        {
            var product = await _productService.GetProductByIdAsync(model.ProductId);

            if (product == null || product.Deleted || !product.Published || !product.AllowCustomerReviews ||
                !await _productService.CanAddReviewAsync(product.Id, (await _storeContext.GetCurrentStoreAsync()).Id))
            {
                model.AddProductReview.Result = await _localizationService.GetResourceAsync("ISDK.Filuet.Theme.FiluetHerbalife.Reviews.NotAllowedForProduct");
                return Json(model);
            }
            await ValidateProductReviewAvailabilityAsync(product);

            if(ModelState.IsValid)
            {
                //save review
                var rating = model.AddProductReview.Rating;
                if (rating < 1 || rating > 5)
                    rating = _catalogSettings.DefaultProductRatingValue;
                var isApproved = !_catalogSettings.ProductReviewsMustBeApproved;

                var productReview = new ProductReview
                {
                    ProductId = product.Id,
                    CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id,
                    Title = model.AddProductReview.Title,
                    ReviewText = model.AddProductReview.ReviewText,
                    Rating = rating,
                    HelpfulYesTotal = 0,
                    HelpfulNoTotal = 0,
                    IsApproved = isApproved,
                    CreatedOnUtc = DateTime.UtcNow,
                    StoreId = (await _storeContext.GetCurrentStoreAsync()).Id,
                };

                await _productService.InsertProductReviewAsync(productReview);

                //add product review and review type mapping                
                foreach (var additionalReview in model.AddAdditionalProductReviewList)
                {
                    var additionalProductReview = new ProductReviewReviewTypeMapping
                    {
                        ProductReviewId = productReview.Id,
                        ReviewTypeId = additionalReview.ReviewTypeId,
                        Rating = additionalReview.Rating
                    };

                    await _reviewTypeService.InsertProductReviewReviewTypeMappingsAsync(additionalProductReview);
                }

                //update product totals
                await _productService.UpdateProductReviewTotalsAsync(product);

                //notify store owner
                if (_catalogSettings.NotifyStoreOwnerAboutNewProductReviews)
                    await _workflowMessageService.SendProductReviewStoreOwnerNotificationMessageAsync(productReview, _localizationSettings.DefaultAdminLanguageId);

                //activity log
                await _customerActivityService.InsertActivityAsync("PublicStore.AddProductReview",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddProductReview"), product.Name), product);

                //raise event
                if (productReview.IsApproved)
                    await _eventPublisher.PublishAsync(new ProductReviewApprovedEvent(productReview));

                model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);
                model.AddProductReview.Title = null;
                model.AddProductReview.ReviewText = null;

                model.AddProductReview.SuccessfullyAdded = true;
                if (!isApproved)
                    model.AddProductReview.Result = await _localizationService.GetResourceAsync("Reviews.SeeAfterApproving");
                else
                    model.AddProductReview.Result = await _localizationService.GetResourceAsync("Reviews.SuccessfullyAdded");

                return Json(model);
            }

            //if we got this far, something failed, redisplay form
            model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);

            var modelError = ModelState.Values.Where(error => error.ValidationState == ModelValidationState.Invalid).FirstOrDefault();
            model.AddProductReview.Result = $"{modelError.Errors.FirstOrDefault().ErrorMessage}";

            return Json(model);
        }

        public async Task<IActionResult> GetUserCart()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var cart = (await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id))
                .ToList();
            if (cart.Count == 0)
                await _logger.WarningAsync("FiluetHerbalifePublicController/GetUserCart empty list was returned");
            return Json(cart);
        }

        public async Task<IActionResult> GetProductBySku(string sku)
        {
            var product = await _productService.GetProductBySkuAsync(sku);
            if (product == null)
                return Json(new { success = false });

            var productName = await _localizationService.GetLocalizedAsync(product, x => x.Name);

            return Json(new { success = true, productId = product.Id, productName = productName });
        }

        public async Task<IActionResult> GetOrderTotals()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);
            var orderTotal =await cartTotal.TotalDue.FormatPriceAsync();
            var orderVolumePoints =await cartTotal.VolumePoints.FormatPriceAsync(true);

            return Json(new { success = true, orderTotal = orderTotal, orderVolumePoints = orderVolumePoints });
        }

        public IActionResult GetFlyoutShoppingCart()
        {
            return ViewComponent("FlyoutShoppingCart");
        }

        public async Task<IActionResult> GetShoppingCartItems()
        {
            var shoppingCartItems = (await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.ShoppingCart,
                (await _storeContext.GetCurrentStoreAsync()).Id))
                    .Sum(item => item.Quantity);

            return Json(new { quantity = shoppingCartItems });
        }

        public async Task<IActionResult> GetWishlistItems()
        {
            var wishlistItems = (await _shoppingCartService.GetShoppingCartAsync(
                await _workContext.GetCurrentCustomerAsync(),
                ShoppingCartType.Wishlist,
                (await _storeContext.GetCurrentStoreAsync()).Id))
                    .Sum(item => item.Quantity);

            return Json(new { quantity = wishlistItems });
        }

        #endregion

    }
}