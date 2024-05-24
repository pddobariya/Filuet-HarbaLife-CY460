using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin
{
    /// <summary>
    /// ExtendedTotalsShoppingCartModelFactory overrides shopping cart order total for displaying actual totals in shopping cart view
    /// </summary>
    public class FiluetTotalsShoppingCartModelFactory : ShoppingCartModelFactory, IShoppingCartModelFactory
    {
        #region Fields

        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IAuthenticationService _authenticationService;
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly IDistributorService _distributorService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly CommonSettings _commonSettings;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FiluetTotalsShoppingCartModelFactory(
            AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            IAuthenticationService authenticationService,
            IFusionIntegrationService fusionIntegrationService,
            IDistributorService distributorService,
            ILogger logger)
            : base(addressSettings,
                  captchaSettings,
                  catalogSettings,
                  commonSettings,
                  customerSettings,
                  addressModelFactory,
                  checkoutAttributeFormatter,
                  checkoutAttributeParser,
                  checkoutAttributeService,
                  countryService,
                  currencyService,
                  customerService,
                  dateTimeHelper,
                  discountService,
                  downloadService,
                  genericAttributeService,
                  giftCardService,
                  httpContextAccessor,
                  localizationService,
                  orderProcessingService,
                  orderTotalCalculationService,
                  paymentPluginManager,
                  paymentService,
                  permissionService,
                  pictureService,
                  priceFormatter,
                  productAttributeFormatter,
                  productService,
                  shippingService,
                  shoppingCartService,
                  stateProvinceService,
                  staticCacheManager,
                  storeContext,
                  storeMappingService,
                  taxService,
                  urlRecordService,
                  vendorService,
                  webHelper,
                  workContext,
                  mediaSettings,
                  orderSettings,
                  rewardPointsSettings,
                  shippingSettings,
                  shoppingCartSettings,
                  taxSettings,
                  vendorSettings)
        {
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _authenticationService = authenticationService;
            _fusionIntegrationService = fusionIntegrationService;
            _distributorService = distributorService;
            _shoppingCartSettings = shoppingCartSettings;
            _catalogSettings = catalogSettings;
            _vendorSettings = vendorSettings;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _commonSettings = commonSettings;
            _customerService = customerService;
            _discountService = discountService;
            _paymentPluginManager = paymentPluginManager;
            _priceFormatter = priceFormatter;
            _logger = logger;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the shopping cart model
        /// </returns>
        protected virtual async Task<ShoppingCartModel> PrepareCustomShoppingCartModelAsync(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //simple properties
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (!cart.Any())
                return model;

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnProductDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.CheckoutAttributes, store.Id);
            var minOrderSubtotalAmountOk = await _orderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);
            if (!minOrderSubtotalAmountOk)
            {
                var minOrderSubtotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderSubtotalAmount, await _workContext.GetWorkingCurrencyAsync());
                model.MinOrderSubtotalWarning = string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderSubtotalAmount"), await _priceFormatter.FormatPriceAsync(minOrderSubtotalAmount, true, false));
            }

            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //discount and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCodes = await _customerService.ParseAppliedDiscountCouponCodesAsync(customer);
            foreach (var couponCode in discountCouponCodes)
            {
                var discount = await (await _discountService.GetAllDiscountsAsync(couponCode: couponCode))
                    .FirstOrDefaultAwaitAsync(async d => d.RequiresCouponCode && (await _discountService.ValidateDiscountAsync(d, customer)).IsValid);

                if (discount != null)
                {
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
                }
            }

            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //checkout attributes
            model.CheckoutAttributes = await PrepareCheckoutAttributeModelsAsync(cart);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = await PrepareShoppingCartItemModelAsync(cart, sci);
                model.Items.Add(cartItemModel);
            }

            //payment methods
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(customer, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (var pm in buttonPaymentMethods)
            {
                if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var viewComponent = pm.GetPublicViewComponent();
                model.ButtonPaymentMethodViewComponents.Add(viewComponent);
            }
            //hide "Checkout" button if we have only "Button" payment methods
            model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponents.Any();

            //order review data
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = await PrepareOrderReviewDataModelAsync(cart);
            }

            return model;
        }

        #endregion

        #region Methods

        public override async Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync()
        {
            MiniShoppingCartModel model = await base.PrepareMiniShoppingCartModelAsync();
            Customer currentCustomer = await _authenticationService.GetAuthenticatedCustomerAsync();

            var quantitySum = model?.Items?.Select(x => x.Quantity).Sum();
            if (model != null && model.Items.Any() && model.Items.Select(x => x.ProductId).ToList().Any())
            {
                await _logger.InsertLogAsync(LogLevel.Debug, $"MiniShoppingCartModel Quantity :{model.Items.Select(x => x.Quantity).Sum()}", customer: currentCustomer);
                await _logger.InsertLogAsync(LogLevel.Debug, $"MiniShoppingCartModel product id :{string.Join(",", model.Items.Select(x => x.ProductId).ToList())}", customer: currentCustomer);
            }

            if (currentCustomer != null)
            {
                model.SubTotal = await CalculateShoppingCartTotal(await _shoppingCartService.GetShoppingCartAsync(currentCustomer));
            }
            model.DisplayCheckoutButton = false;
            return model;
        }

        public override async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(IList<ShoppingCartItem> cart, bool isEditable)
        {
            OrderTotalsModel baseModel = await base.PrepareOrderTotalsModelAsync(cart, isEditable);
            baseModel.OrderTotal = await CalculateShoppingCartTotal(cart);
            return baseModel;
        }

        /// <summary>
        /// Prepare the shopping cart model.
        /// If user is debtor shopping cart is uneditable
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareEstimateShippingIfEnabled">Whether to prepare estimate shipping model</param>
        /// <param name="setEstimateShippingDefaultAddress">Whether to use customer default shipping address for estimating</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>Shopping cart model</returns>
        public override async Task<ShoppingCartModel> PrepareShoppingCartModelAsync(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false)
        {
            Customer customer = await _workContext.GetCurrentCustomerAsync();

            var quantitySum = cart?.Select(x => x.Quantity).Sum();
            await _logger.InsertLogAsync(LogLevel.Debug, $"Quantity sum before: {quantitySum.Value}", customer: customer);

            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            ShoppingCartModel shoppingCartModel = await PrepareCustomShoppingCartModelAsync(model, cart, isEditable, validateCheckoutAttributes,
                prepareAndDisplayOrderReviewData);

            var quantitySum1 = shoppingCartModel?.Items?.Select(x => x.Quantity).Sum();
            await _logger.InsertLogAsync(LogLevel.Debug, $"Quantity sum after: {quantitySum1.Value}", customer: customer);

            var distributorProfileOfCurrentCustomer = await _distributorService.GetDistributorDetailedProfileAsync(customer);

            //model.ConfirmIndependentPartners = !string.IsNullOrWhiteSpace(distributorProfileOfCurrentCustomer?.DsType) && distributorProfileOfCurrentCustomer.DsType.ToLower() != "PM".ToLower();
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;

            bool isDebtor = await customer.IsDebtorAsync();
            if (isDebtor)
            {
                foreach (var item in shoppingCartModel.Items)
                {
                    item.AllowItemEditing = false;
                    item.DisableRemoval = true;
                }

                shoppingCartModel.CustomProperties.Add(NopFiluetCommonDefaults.IsDebtor, true.ToString());
            }

            shoppingCartModel.IsEditable = isEditable;
            return shoppingCartModel;
        }

        private async Task<string> CalculateShoppingCartTotal(IEnumerable<ShoppingCartItem> cart)
        {
            Customer currentCustomer = await _authenticationService.GetAuthenticatedCustomerAsync();
            var cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(currentCustomer, cart);
            return await cartTotal.TotalDue.FormatPriceAsync();
        }

        #endregion
    }
}