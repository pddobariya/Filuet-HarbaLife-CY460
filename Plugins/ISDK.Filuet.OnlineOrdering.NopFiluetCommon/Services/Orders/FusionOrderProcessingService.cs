using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Orders
{
    public class FusionOrderProcessingService : OrderProcessingService
    {
        #region Fields

        public static bool IsOverrideOrderTotal => true;
        private readonly ILogger _loggerService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FusionOrderProcessingService(
            CurrencySettings currencySettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICountryService countryService, 
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService, 
            ICustomerService customerService, 
            ICustomNumberFormatter customNumberFormatter, 
            IDiscountService discountService, 
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILogger logger, 
            IOrderService orderService, 
            IOrderTotalCalculationService orderTotalCalculationService, 
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPdfService pdfService, 
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IRewardPointService rewardPointService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            ITaxService taxService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            OrderSettings orderSettings,
            PaymentSettings paymentSettings, 
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings, 
            ILogger loggerService)
            : base(currencySettings, 
                  addressService, 
                  affiliateService, 
                  checkoutAttributeFormatter,
                  countryService, 
                  currencyService,
                  customerActivityService,
                  customerService, 
                  customNumberFormatter,
                  discountService,
                  encryptionService,
                  eventPublisher, 
                  genericAttributeService, 
                  giftCardService,
                  languageService, 
                  localizationService, 
                  logger, 
                  orderService,
                  orderTotalCalculationService,
                  paymentPluginManager,
                  paymentService, 
                  pdfService, 
                  priceCalculationService,
                  priceFormatter,
                  productAttributeFormatter,
                  productAttributeParser,
                  productService,
                  returnRequestService,
                  rewardPointService,
                  shipmentService, 
                  shippingService,
                  shoppingCartService,
                  stateProvinceService,
                  storeService, 
                  taxService, 
                  vendorService,
                  webHelper, 
                  workContext,
                  workflowMessageService, 
                  localizationSettings, 
                  orderSettings, 
                  paymentSettings,
                  rewardPointsSettings,
                  shippingSettings,
                  taxSettings)
        {
            _loggerService = loggerService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        protected override async Task<PlaceOrderContainer> PreparePlaceOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest)
        {
            await _loggerService.InformationAsync("PreparePlaceOrderDetails: IsOverrideOrderTotal: " + IsOverrideOrderTotal);
            await _loggerService.InformationAsync("PreparePlaceOrderDetails: Order total: " + processPaymentRequest.OrderTotal);
            var orderTotal = processPaymentRequest.OrderTotal;
            var result = await base.PreparePlaceOrderDetailsAsync(processPaymentRequest);
            if (!IsOverrideOrderTotal || orderTotal == 0)
            {
                return result;
            }

            result.OrderTotal = orderTotal;
            processPaymentRequest.OrderTotal = orderTotal;

            return result;
        }

        protected override async Task<Order> SaveOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest, ProcessPaymentResult processPaymentResult, PlaceOrderContainer details)
        {
            //if order total of processPaymentRequest and details differ then it was overriden by payment plugin in ProcessPayment method so we need update details with processPaymentRequest data 
            if (IsOverrideOrderTotal && processPaymentRequest.OrderTotal != details.OrderTotal && processPaymentRequest.OrderTotal != 0)
            {
                details.OrderTotal = processPaymentRequest.OrderTotal;
            }

            Order savedOrder = await base.SaveOrderDetailsAsync(processPaymentRequest, processPaymentResult, details);

            if (savedOrder != null)
            {
            }

            return savedOrder;
        }

        public override async Task CheckOrderStatusAsync(Order order)
        {
            if (order.OrderStatus == OrderStatus.Processing &&
                await _genericAttributeService.GetAttributeAsync<bool>(order,OrderAttributeNames.IsFusionSubmitOrderSuccess))
                await SetOrderStatusAsync(order, OrderStatus.Complete, true);
            await base.CheckOrderStatusAsync(order);
        }

        #endregion
    }
}
