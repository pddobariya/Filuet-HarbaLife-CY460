using Filuet.Onlineordering.Shipping.Delivery.Constants;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Controllers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Controllers
{
    public class CheckoutDeliveryController : FiluetCheckoutController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly IDeliveryPriceService _deliveryPriceService;
        private readonly IFiluetCustomerService _filuetCustomerService;
        private readonly ISettingService _settingService;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly IAddressService _addressService;
        private readonly IPluginService _pluginService;

        #endregion

        #region Ctor

        public CheckoutDeliveryController(AddressSettings addressSettings,
            CaptchaSettings captchaSettings, 
            CustomerSettings customerSettings, 
            IAddressAttributeParser addressAttributeParser, 
            IAddressModelFactory addressModelFactory,
            IAddressService addressService, 
            ICheckoutModelFactory checkoutModelFactory, 
            ICountryService countryService,
            ICustomerService customerService, 
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService, 
            ILogger logger, 
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService, 
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext, 
            ITaxService taxService, 
            IWebHelper webHelper, 
            IWorkContext workContext, 
            OrderSettings orderSettings, 
            PaymentSettings paymentSettings, 
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings, 
            IFiluetShippingCartService filuetShippingCartService, 
            IDualMonthsService dualMonthsService, 
            ICategoryService categoryService,
            IHttpContextAccessor httpContextAccessor, 
            IPluginService pluginService, 
            IDeliveryPriceService deliveryPriceService, 
            IFiluetCustomerService filuetCustomerService, 
            ISettingService settingService, 
            IFiluetShippingService filuetShippingService) 
            : base(addressSettings,
                  captchaSettings,
                  customerSettings, 
                  addressAttributeParser, 
                  addressModelFactory, 
                  addressService, 
                  checkoutModelFactory,
                  countryService, 
                  customerService,
                  genericAttributeService,
                  localizationService,
                  logger, 
                  orderProcessingService,
                  orderService, 
                  paymentPluginManager, 
                  paymentService, 
                  productService,
                  shippingService,
                  shoppingCartService,
                  storeContext,
                  taxService,
                  webHelper,
                  workContext,
                  orderSettings,
                  paymentSettings,
                  rewardPointsSettings,
                  shippingSettings,
                  taxSettings,
                  filuetShippingCartService,
                  dualMonthsService,
                  categoryService,
                  httpContextAccessor, 
                  pluginService)
        {
            _addressService = addressService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _pluginService = pluginService;
            _deliveryPriceService = deliveryPriceService;
            _filuetCustomerService = filuetCustomerService;
            _settingService = settingService;
            _filuetShippingService = filuetShippingService;
        }

        #endregion

        #region Methods

        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> OpcSaveShippingDeliveryMethod(int deliveryType, string receiverName, string receiverPhone, string receiverEmail, string deliveryAddress, int operatorPriceId, string receiverPostCode, string comment)
        {
            var Store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            var filuetFusionShippingComputationOptionCustomerData =await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
            var deliveryPluginSettings = _settingService.LoadSetting<DeliveryPluginSettings>();
            IPhoneFormatter phoneFormatter = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                phoneFormatter = serviceScope.ServiceProvider.GetService<IPhoneFormatter>();
            }
            var countryCode = (await _filuetShippingService
                .GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData
                    .FiluetFusionShippingComputationOptionId))?.CountryCode;

            var phonePrefix =await phoneFormatter?.FormatPrefixAsync(deliveryPluginSettings.PhonePrefix, countryCode) ?? deliveryPluginSettings.PhonePrefix;
            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingMethodSystemName, "Shipping.Delivery");
            bool saveAddresses = false;
            var billingAddressId = customer.BillingAddressId;
            var billingAddress = billingAddressId is null
                ? null
                : await _addressService.GetAddressByIdAsync(billingAddressId.Value);
            if (billingAddress == null)
            {

                billingAddress = new Address
                {
                    Address1 = customer.StreetAddress,
                    Address2 = customer.StreetAddress2,
                    City = customer.City,
                    Email = customer.Email,
                    PhoneNumber = customer.Phone,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,

                };
                await _customerService.InsertCustomerAddressAsync(customer, billingAddress);

                customer.BillingAddressId = billingAddress.Id;
                await _customerService.UpdateCustomerAsync(customer);
                saveAddresses = true;
            }

            DeliveryObject deliveryObject = null;
            var isPickupInStore = true;

            //save
            if (deliveryType == (int)ShipingMethodEnum.SelfPickup)
            {

                var scAddressSelector = int.Parse(deliveryAddress);

                if (billingAddress.Email != receiverEmail)
                {
                    saveAddresses = true;
                }
                billingAddress.Email = receiverEmail;
                receiverPhone = $"{phonePrefix}-{receiverPhone}";
                receiverPhone = await phoneFormatter?.FormatPrefixAsync(receiverPhone, phonePrefix) ?? receiverPhone;
                var saleCenter = await _deliveryPriceService.GetSalesCenterByIdAsync(scAddressSelector, language.Id);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingWareHouse, saleCenter.WarehouseCode);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DeliveryPrice, saleCenter.Price);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.City, saleCenter.City);
                var addressStr = saleCenter.Address;
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.Address, addressStr);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingFullname, receiverName);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingPhoneNumber, receiverPhone);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingFreightCode, saleCenter.FreightCode);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.ShippingNotes, $"{receiverName}\r\np:{receiverPhone}{(deliveryPluginSettings.AddAddressToComment ? (Environment.NewLine + addressStr) : string.Empty)}");

                await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null);

                 _filuetCustomerService.AddPhone(receiverPhone);
            }
            else if (deliveryType == (await _deliveryPriceService.GetDeliveryTypesAsync(language.Id)).FirstOrDefault(dt => dt.SystemType == ShipingMethodEnum.Delivery.ToString()).Id)
            {
                if (billingAddress.Email != receiverEmail)
                {
                    saveAddresses = true;
                }
                billingAddress.Email = receiverEmail;
                receiverPhone = $"{phonePrefix}-{receiverPhone}";
                receiverPhone = await phoneFormatter?.FormatPrefixAsync(receiverPhone, phonePrefix) ?? receiverPhone;
                deliveryObject = await _deliveryPriceService.GetDeliveryObjectByPriceIdAsync(operatorPriceId);
                //await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DeliveryPrice, deliveryObject.DeliveryPrise);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.City, deliveryObject.DeliveryCity);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.Address, $"{deliveryAddress}{Environment.NewLine}{receiverPostCode}");
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingFullname, receiverName);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingPhoneNumber, receiverPhone);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingWareHouse, deliveryObject.WarehouseCode);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingFreightCode, deliveryObject.FreightCode);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.ShippingNotes, $"DeliveryOperator: {deliveryObject.OperatorName}\r\np:{receiverPhone}{(deliveryPluginSettings.AddAddressToComment ? (Environment.NewLine + deliveryAddress) : string.Empty)}{(string.IsNullOrWhiteSpace(comment) ? string.Empty : "\r\n" + comment)}");

                await _genericAttributeService.SaveAttributeAsync<string>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null);

                 _filuetCustomerService.AddStreetAddress(deliveryAddress);
                 _filuetCustomerService.AddPhone(receiverPhone);
            }
            else
            {
                var pickupPointAddress = int.Parse(deliveryAddress);
                if (billingAddress.Email != receiverEmail)
                {
                    saveAddresses = true;
                }
                billingAddress.Email = receiverEmail;
                receiverPhone = $"{phonePrefix}-{receiverPhone}";
                receiverPhone = await phoneFormatter?.FormatPrefixAsync(receiverPhone, phonePrefix) ?? receiverPhone;
                deliveryObject = await _deliveryPriceService.GetDeliveryObjectByAutoPostOfficeIdAsync(pickupPointAddress, operatorPriceId);
              //  await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DeliveryPrice, deliveryObject.DeliveryPrise);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.City, deliveryObject.DeliveryCity);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.Address, deliveryObject.Address);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingFullname, receiverName);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingPhoneNumber, receiverPhone);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingWareHouse, deliveryObject.WarehouseCode);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingFreightCode, deliveryObject.FreightCode);
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.ShippingNotes, $"DeliveryOperator: {deliveryObject.OperatorName}\r\np:{receiverPhone}{(deliveryPluginSettings.AddAddressToComment ? (Environment.NewLine + deliveryObject.Address) : string.Empty)}{(string.IsNullOrWhiteSpace(comment) ? string.Empty : "\r\n" + comment)}");
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPickupPointAttribute, deliveryObject.PointId);

                 _filuetCustomerService.AddPhone(receiverPhone);
            }
            await _genericAttributeService.SaveAttributeAsync(customer, OrderAttributeNames.SelectedShippingZipCode, $"{receiverPostCode}");
          
            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.CountryOrderOfProcessing, countryCode);

            var shippingAddressId = customer.ShippingAddressId;
            var shippingAddress = shippingAddressId is null
                ? null
                : await _addressService.GetAddressByIdAsync(shippingAddressId.Value);

            if (shippingAddress == null)
            {
                customer.ShippingAddressId = billingAddressId;
                saveAddresses = true;
            }
            else
            {
                shippingAddress.Email = billingAddress.Email;
                await _addressService.UpdateAddressAsync(shippingAddress);
            }
            if (saveAddresses)
            {
                await _customerService.UpdateCustomerAsync(customer);
                await _addressService.UpdateAddressAsync(billingAddress);
            }

            try
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                    throw new Exception("Shipping is not required");


                var pluginDescriptor = (await _pluginService.GetPluginDescriptorsAsync<DeliveryPlugin>()).First();
                await _genericAttributeService.SaveAttributeAsync(customer, ShippingDetailsAttributes.ShippingDetailsCountryAttribute, countryCode, Store.Id);

                if (deliveryObject != null)
                {
                    var shippingDetailsComment = deliveryObject.Comment != null ? JsonConvert.SerializeObject(deliveryObject.Comment) : "";
                    await _genericAttributeService.SaveAttributeAsync(customer, ShippingDetailsAttributes.ShippingDetailsCommentAttribute, shippingDetailsComment, Store.Id);
                    var selectedShippingOption = new ShippingOption
                    {
                        ShippingRateComputationMethodSystemName = pluginDescriptor.SystemName,
                        Name = deliveryObject.OperatorName,
                        IsPickupInStore = isPickupInStore
                    };
                    await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, selectedShippingOption, Store.Id);
                }

                else
                {
                    var shippingDetailsCommentAttribute = await _genericAttributeService.GetCustomAttributeAsync(customer, ShippingDetailsAttributes.ShippingDetailsCommentAttribute);
                    if (shippingDetailsCommentAttribute != null)
                        await _genericAttributeService.DeleteAttributeAsync(shippingDetailsCommentAttribute);
                }

                return await base.OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, customer);
                return Json(new { error = 1, message = exc.Message }.ToString());
            }
        }

        public override async Task<IActionResult> OnePageCheckout()
        {
            var customer =await _workContext.GetCurrentCustomerAsync();
            var language =await _workContext.GetWorkingLanguageAsync();

            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.NofAddress,
                (await _deliveryPriceService.GetSalesCentersAsync(language.Id)).OrderBy(sc => sc.Id)
                    .FirstOrDefault()?.Address);
            return await base.OnePageCheckout();
        }

        #endregion

    }
}
