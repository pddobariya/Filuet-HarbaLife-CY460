using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core;
using Nop.Core.Domain.Common;
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

namespace Nop.Plugin.Payments.Payeezy.Services
{
    public class PayeezyOrderProcessingService : OrderProcessingService
    {
        #region Fields

        public static bool IsOverrideOrderTotal { get; set; }

        private readonly ICountryService _countryService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _loggerService;
        private readonly IStoreContext _storeContext;
        private readonly IFiluetShippingService _filuetShippingService;

        #endregion

        #region Ctor

        public PayeezyOrderProcessingService(CurrencySettings currencySettings,
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
            IStoreContext storeContext,
            IFiluetShippingService filuetShippingService)
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
            _countryService = countryService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _loggerService = logger;
            _storeContext = storeContext;
            _filuetShippingService = filuetShippingService;
        }

        #endregion

        #region Methods

        protected override async Task<PlaceOrderContainer> PreparePlaceOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest)
        {
            await _loggerService.InformationAsync("PreparePlaceOrderDetails: IsOverrideOrderTotal: " + IsOverrideOrderTotal);
            await _loggerService.InformationAsync("PreparePlaceOrderDetails: Order total: " + processPaymentRequest.OrderTotal);
            decimal orderTotal = processPaymentRequest.OrderTotal;
            PlaceOrderContainer result = await base.PreparePlaceOrderDetailsAsync(processPaymentRequest);
            if (IsOverrideOrderTotal && orderTotal != 0) //don't override on initial order placing when order total is not known during standard checkout process
            {
                result.OrderTotal = orderTotal;
                processPaymentRequest.OrderTotal = orderTotal;
            }

            var customer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(
                customer, NopCustomerDefaults.SelectedShippingOptionAttribute,
                currentStore.Id);
            result.ShippingMethodName = shippingOption?.Name;
            result.PickupInStore = shippingOption?.IsPickupInStore ?? true;

            var filuetFusionShippingComputationOptionCustomerData = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
            var countryCode = await _genericAttributeService.GetAttributeAsync<string>(
                customer, ShippingDetailsAttributes.ShippingDetailsCountryAttribute,
                currentStore.Id) ?? (await _filuetShippingService
                .GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData
                    .FiluetFusionShippingComputationOptionId))?.CountryCode;
            var countryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(
                countryCode)).Id;
            var address = new Address
            {
                ZipPostalCode = await _genericAttributeService.GetAttributeAsync<string>(
                    customer, OrderAttributeNames.SelectedShippingZipCode),
                CountryId = countryId,
                City = await _genericAttributeService.GetAttributeAsync<string>(
                    customer, CustomerAttributeNames.City),
                Address1 = await _genericAttributeService.GetAttributeAsync<string>(
                    customer, CustomerAttributeNames.Address),
                PhoneNumber = await _genericAttributeService.GetAttributeAsync<string>(
                    customer, CustomerAttributeNames.SelectedShippingPhoneNumber)
            };
            if (result.PickupInStore)
            {
                result.PickupAddress = address;
            }
            else
            {
                result.ShippingAddress = address;
            }
            return result;
        }

        protected override async Task<Order> SaveOrderDetailsAsync(ProcessPaymentRequest processPaymentRequest,
            ProcessPaymentResult processPaymentResult, PlaceOrderContainer details)
        {
            //if order total of processPaymentRequest and details differ then it was overriden by payment plugin in ProcessPayment method so we need update details with processPaymentRequest data 
            if (IsOverrideOrderTotal && processPaymentRequest.OrderTotal != details.OrderTotal && processPaymentRequest.OrderTotal != 0)
            {
                details.OrderTotal = processPaymentRequest.OrderTotal;
            }

            Order savedOrder = await base.SaveOrderDetailsAsync(processPaymentRequest, processPaymentResult, details);

            if (savedOrder != null)
            {
                //save order to Fusion

            }

            return savedOrder;
        }

        #endregion
    }
}
