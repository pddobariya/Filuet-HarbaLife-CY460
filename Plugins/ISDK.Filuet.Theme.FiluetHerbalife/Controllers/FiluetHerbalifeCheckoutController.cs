using Filuet.Onlineordering.Shipping.Delivery;
using Filuet.Onlineordering.Shipping.Delivery.Controllers;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Factories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
using Nop.Core.Http.Extensions;
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
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IPhoneFormatter = Filuet.Onlineordering.Shipping.Delivery.Services.IPhoneFormatter;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Controllers
{

    public class FiluetHerbalifeCheckoutController : CheckoutDeliveryController
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IFiluetShippingCartService _filuetShippingCartService;
        private readonly ISettingService _settingService;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly ICategoryService _categoryService;
        private readonly ICustomCheckoutModelFactory _customCheckoutModelFactory;

        #endregion

        #region Ctor

        public FiluetHerbalifeCheckoutController(
            AddressSettings addressSettings,
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
            IFiluetShippingService filuetShippingService,
            ICustomCheckoutModelFactory customCheckoutModelFactory) : base(addressSettings, captchaSettings, customerSettings, addressAttributeParser, addressModelFactory, addressService, checkoutModelFactory, countryService, customerService, genericAttributeService, localizationService, logger, orderProcessingService, orderService, paymentPluginManager, paymentService, productService, shippingService, shoppingCartService, storeContext, taxService, webHelper, workContext, orderSettings, paymentSettings, rewardPointsSettings, shippingSettings, taxSettings, filuetShippingCartService, dualMonthsService, categoryService, httpContextAccessor, pluginService, deliveryPriceService, filuetCustomerService, settingService, filuetShippingService)
        {
            _addressSettings = addressSettings;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _filuetShippingCartService = filuetShippingCartService;
            _dualMonthsService = dualMonthsService;
            _categoryService = categoryService;
            _settingService = settingService;
            _filuetShippingService = filuetShippingService;
            _customCheckoutModelFactory = customCheckoutModelFactory;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> OnePageCheckout()
        {
            try
            {
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();

                IResidentChecker residentChecker = null;
                var filuetFusionShippingComputationOptionId = 0;
                var filuetFusionShippingComputationOptionCustomerData =
                   await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(currentCustomer.Id);
                await _logger.InformationAsync("Checkout filuetFusionShippingComputationOptionCustomerData: " + JsonConvert.SerializeObject(filuetFusionShippingComputationOptionCustomerData));

                if (filuetFusionShippingComputationOptionCustomerData == null)
                {
                    var option = await _filuetShippingService.GetShippingComputationOptionCustomerDataListByCustomerIdAsync(currentCustomer.Id);
                    await _logger.InformationAsync("Checkout GetShippingComputationOptionByCustomerId: " + JsonConvert.SerializeObject(option));
                    filuetFusionShippingComputationOptionId = option?.FirstOrDefault().Id ?? (await _filuetShippingService.GetAllShippingComputationOptionsAsync()).First().Id;
                }
                else
                {
                    filuetFusionShippingComputationOptionId = filuetFusionShippingComputationOptionCustomerData
                        .FiluetFusionShippingComputationOptionId;
                }
                var deliveryPluginSettings = await _settingService.LoadSettingAsync<DeliveryPluginSettings>();
                await _logger.InformationAsync("Checkout deliveryPluginSettings: " + JsonConvert.SerializeObject(deliveryPluginSettings));
                IPhoneFormatter phoneFormatter = null;
                var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                using (var serviceScope = serviceScopeFactory.CreateScope())
                {
                    residentChecker = serviceScope.ServiceProvider.GetService<IResidentChecker>();
                    phoneFormatter = serviceScope.ServiceProvider.GetService<IPhoneFormatter>();
                }
                var countryCode = (await _filuetShippingService
                           .GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionId))?.CountryCode;
                await _logger.InformationAsync("Checkout countryCode: " + JsonConvert.SerializeObject(countryCode));
                var phonePrefix = await phoneFormatter?.FormatPrefixAsync(deliveryPluginSettings.PhonePrefix, countryCode) ??
                                  deliveryPluginSettings.PhonePrefix;
                await _logger.InformationAsync("Checkout phonePrefix: " + JsonConvert.SerializeObject(phonePrefix));

                var customerRoles = await _customerService.GetCustomerRolesAsync(currentCustomer);
                await _logger.InformationAsync("Checkout customerRoles: " + JsonConvert.SerializeObject(customerRoles));
                if (residentChecker.NeedToCheckIfAuthenticatedCustomerIsResident && customerRoles.Any(cr =>
            cr.SystemName == CommonConstants.IsNotResidentCustomerRole))
                {
                    return new RedirectToRouteResult("ShoppingCart");
                }
                await _logger.InformationAsync("Checkout residentChecker: ОК");
                //validation
                if (_orderSettings.CheckoutDisabled)
                    return new RedirectToRouteResult("ShoppingCart");

                var cart = await _shoppingCartService.GetShoppingCartAsync(currentCustomer,
                    ShoppingCartType.ShoppingCart, store.Id);
                await _logger.InformationAsync("Checkout cart: " + JsonConvert.SerializeObject(cart));
                if (!cart.Any())
                    return new RedirectToRouteResult("ShoppingCart");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    return new RedirectToRouteResult("Checkout");

                var checkoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(
                    currentCustomer,
                    NopCustomerDefaults.CheckoutAttributes, store.Id);
                await _logger.InformationAsync("Checkout checkoutAttributes: " + JsonConvert.SerializeObject(checkoutAttributes));
                var scWarnings =
                    await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributes, true);
                await _logger.InformationAsync("Checkout scWarnings: " + JsonConvert.SerializeObject(scWarnings));
                if (scWarnings.Any())
                    return new RedirectToRouteResult("ShoppingCart");

                var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(store.Id);
                await _logger.InformationAsync("Checkout settings: " + JsonConvert.SerializeObject(settings));
                if ((await _filuetShippingCartService.IsCartValid(currentCustomer, cart.ToList(), new(settings.MonthVpLimit, settings.OneOrderVpLimit))).Any() || !await _dualMonthsService.IsMonthSelectedAsync(currentCustomer))
                {
                    return new RedirectToRouteResult("ShoppingCart");
                }
                await _logger.InformationAsync("Checkout valid: OK");
                if (await _customerService.IsGuestAsync(currentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return new ChallengeResult();
                await _logger.InformationAsync("Checkout IsGuestAsync: OK");
                var model = await _customCheckoutModelFactory.PrepareOnePageCheckoutModelAsync(cart);
                await _logger.InformationAsync("Checkout model1: " + JsonConvert.SerializeObject(model));
                model.PhonePrefix = phonePrefix;
                model.PhoneMask = deliveryPluginSettings.PhoneMask;
                model.Phones = await GetCustomerPhones(currentCustomer);
                await _logger.InformationAsync("Checkout model: " + JsonConvert.SerializeObject(model));
                //Check whether payment workflow is required
                //we ignore reward points during cart total calculation
                var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, false);
                if (isPaymentWorkflowRequired)
                {
                    //filter by country
                    var filterByCountryId = 0;
                    var billingAddress = currentCustomer.BillingAddressId.HasValue
                        ? await _addressService.GetAddressByIdAsync(currentCustomer.BillingAddressId.Value)
                        : null;
                    if (_addressSettings.CountryEnabled &&
                        billingAddress != null &&
                        billingAddress.CountryId != null)
                    {
                        filterByCountryId = billingAddress.CountryId.Value;
                    }

                    //payment is required
                    var paymentMethodModel =
                        await _checkoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);
                    model.PaymentMethods = paymentMethodModel.PaymentMethods;
                }

                if (await ApfApfShoppingCartHelper.ShoppingCartContainsApfItemAsync(currentCustomer))
                {
                    var cart2 = await _shoppingCartService.GetShoppingCartAsync(currentCustomer, ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);
                    await base.OpcLoadStepAfterShippingMethod(cart2);
                    ViewBag.IsDeliverySkipped = true;
                    return View("OpcPayment", model);
                }

                var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(cart.Select(x => x.ProductId).FirstOrDefault());
                if (await productCategories
                    .AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x,CategoryAttributeNames.CategoryType) ==
                              CategoryTypeEnum.Ticket))
                {
                    var cart2 = await _shoppingCartService.GetShoppingCartAsync(currentCustomer,
                        ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id);
                    await base.OpcLoadStepAfterShippingMethod(cart2);
                    ViewBag.IsDeliverySkipped = true;
                    return View("OpcPayment", model);
                }
                return View(model);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Checkout ERROR: ", e);
                throw;
            }
        }

        private async Task<List<string>> GetCustomerPhones(Customer currentCustomer)
        {
            var phones = new List<string>();
            var phoneAttribute = currentCustomer.Phone;
            if (phoneAttribute != null)
            {
                phones.Add(phoneAttribute);
            }

            phoneAttribute = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CoreGenericAttributes.PhoneAttribute);
            if (phoneAttribute != null)
            {
                phones.AddRange(JsonConvert.DeserializeObject<List<string>>(phoneAttribute));
            }

            return phones.Where(phone => !string.IsNullOrWhiteSpace(phone))
                .Select(phone => phone.Replace(" ", string.Empty)
                    .Replace("-", string.Empty)
                    .Replace("+", string.Empty)
                    .Replace("(", string.Empty)
                    .Replace(")", string.Empty))
                .ToList();
        }

        public async Task<IActionResult> OpcSaveShippingDelivery(IFormCollection form)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var cart = (await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync()))
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .ToList();
            var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            if ((await _filuetShippingCartService.IsCartValid(currentCustomer, cart, (settings.MonthVpLimit, settings.OneOrderVpLimit))).Any())
            {
                return new RedirectToRouteResult("CheckoutOnePage");
            }

            int deliveryType = Convert.ToInt32(form["deliveryType"]);
            string receiverName = form["receiverName"];
            string receiverPhone = form["receiverPhone"];
            string receiverEmail = form["receiverEmail"];
            string deliveryAddress = form["deliveryAddress"];
            string deliveryOperator = form["deliveryOperator"];
            int operatorPriceId = !string.IsNullOrEmpty(form["operatorPriceId"]) ? Convert.ToInt32(form["operatorPriceId"]) : 0;
            string receiverPostCode = form["receiverPostCode"];
            string comment = form["comment"];

            receiverPhone = receiverPhone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");

            await _genericAttributeService.SaveAttributeAsync(currentCustomer, FiluetThemePluginDefaults.SelectedDeliveryOperatorId, deliveryOperator);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.SelectedPaymentProvider, form["paymentProvider"]);

            IActionResult baseResult = await base.OpcSaveShippingDeliveryMethod(deliveryType, receiverName, receiverPhone,
                receiverEmail, deliveryAddress, operatorPriceId, receiverPostCode, comment);

            return await OpcSavePaymentInfo(form);
        }

        public override async Task<IActionResult> OpcSavePaymentInfo(IFormCollection form)
        {
            var inn = form["inn"];
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            if (!string.IsNullOrEmpty(inn))
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, CoreGenericAttributes.CustomerInnAttribute, inn);

            var receiverPhone = form["receiverPhone"];
            if (!string.IsNullOrEmpty(receiverPhone))
                currentCustomer.Phone = receiverPhone;
                await _customerService.UpdateCustomerAsync(currentCustomer);


            bool? isShipInvoiceWithOrder = !string.IsNullOrEmpty(form["IsShipInvoiceWithOrder"]) ? (bool?)Convert.ToBoolean(form["IsShipInvoiceWithOrder"]) : null;
            if (isShipInvoiceWithOrder.HasValue)
            {
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.IsShipInvoiceWithOrder, isShipInvoiceWithOrder.Value);
            }

            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuestAsync(currentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                //await ValidateDeliveryAsync();

                var paymentMethodSystemName = form["paymentmethod"];
                if (!string.IsNullOrEmpty(paymentMethodSystemName))
                    await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentMethodSystemName, (await _storeContext.GetCurrentStoreAsync()).Id);
                else
                    paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,
                                        NopCustomerDefaults.SelectedPaymentMethodAttribute, (await _storeContext.GetCurrentStoreAsync()).Id);

                var paymentMethod = await _paymentPluginManager
                                        .LoadPluginBySystemNameAsync(paymentMethodSystemName, currentCustomer, (await _storeContext.GetCurrentStoreAsync()).Id)
                                    ?? throw new Exception("Payment method is not selected");

                var warnings = ModelState.Values.FirstOrDefault(ms => ms.ValidationState == ModelValidationState.Invalid)?.Errors.Select(me => me.ErrorMessage).ToArray() ?? Array.Empty<string>();//.ErrorCount;//await paymentMethod.ValidatePaymentFormAsync(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = await paymentMethod.GetPaymentInfoAsync(form);
                    //set previous order GUID (if exists)
                    _paymentService.GenerateOrderGuid(paymentInfo);

                    //session save
                    HttpContext.Session.Set("OrderPaymentInfo", paymentInfo);

                    return Json(new
                    {
                        redirect = Url.RouteUrl("OpcConfirmOrderInfo")
                    });
                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);
                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        html = await RenderPartialViewToStringAsync("OpcPaymentInfo", paymenInfoModel)
                    }
                });
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc, currentCustomer);
                return Json(new { error = 1, message = exc.Message });
            }
        }

        public virtual async Task<IActionResult> OpcConfirmOrderInfo()
        {
            try
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();

                var cart = await _shoppingCartService.GetShoppingCartAsync(currentCustomer, ShoppingCartType.ShoppingCart, store.Id);
                if (!cart.Any())
                {
                    return RedirectToRoute("ShoppingCart");
                }

                if (await ApfApfShoppingCartHelper.ShoppingCartContainsApfItemAsync(currentCustomer))
                {
                    ViewBag.IsDeliverySkipped = true;
                }

                var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(cart.Select(x => x.ProductId).FirstOrDefault());
                if (await productCategories
                    .AnyAwaitAsync(async x =>await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x,CategoryAttributeNames.CategoryType) ==
                              CategoryTypeEnum.Ticket)) 
                   
                {
                    ViewBag.IsDeliverySkipped = true;
                }

                return View("OpcConfirmOrder");
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
                return new RedirectToRouteResult("ShoppingCart");
            }
        }

        #endregion
    }
}
