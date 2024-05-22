using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Checkout;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
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
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Controllers
{
    public class FiluetCheckoutController : CheckoutController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IFiluetShippingCartService _filuetShippingCartService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly ILogger _logger;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreContext _storeContext;
        private readonly OrderSettings _orderSettings;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentService _paymentService;
        private readonly AddressSettings _addressSettings;
        private readonly IAddressService _addressService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IPluginService _pluginService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly CaptchaSettings _captchaSettings;

        #endregion

        #region Ctor

        public FiluetCheckoutController(
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
            IPluginService pluginService) 
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
                  taxSettings)
        {
            _addressSettings = addressSettings;
            _addressService = addressService;
            _checkoutModelFactory = checkoutModelFactory;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _paymentSettings = paymentSettings;
            _filuetShippingCartService = filuetShippingCartService;
            _dualMonthsService = dualMonthsService;
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
            _pluginService = pluginService;
            _captchaSettings = captchaSettings; 
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> OnePageCheckout()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            if ((await _filuetShippingCartService.IsCartValid()).Any() || !await _dualMonthsService.IsMonthSelectedAsync(currentCustomer))
            {
                return RedirectToRoute("ShoppingCart");
            }

            IResidentChecker residentChecker = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                residentChecker = serviceScope.ServiceProvider.GetService<IResidentChecker>();
            }

            var customerRoles = await _customerService.GetCustomerRolesAsync(currentCustomer);

            if (residentChecker?.NeedToCheckIfAuthenticatedCustomerIsResident == true && customerRoles.Any(cr =>
                    cr.SystemName == CommonConstants.IsNotResidentCustomerRole))
            {
                return RedirectToRoute("ShoppingCart");
            }

            return await base.OnePageCheckout();
        }

        public override async Task<IActionResult> Completed(int? orderId)
        {
            if (!orderId.HasValue)
            {
                return RedirectToRoute("HomePage");
            }

            try
            {
                await _logger.InformationAsync($"[FiluetCheckoutController].[Completed] OrderId={orderId}");

                Order order = await _orderService.GetOrderByIdAsync(orderId.Value);
                if (order == null || order.Deleted)
                {
                    await _logger.InformationAsync("[FiluetCheckoutController].[Completed] order is null or not found");
                    return RedirectToRoute("HomePage");
                }

                await _orderProcessingService.CheckOrderStatusAsync(order);
                ViewBag.HideDetails = order.PaymentMethodSystemName == "Payments.UzOffline";
                Customer customer = await _workContext.GetCurrentCustomerAsync();
                if (customer == null || customer.Id != order.CustomerId)
                {
                    await _logger.InformationAsync("[FiluetCheckoutController].[Completed] customer is null or different");
                    return RedirectToRoute("HomePage");
                }

                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                    CoreGenericAttributes.ApfMessageAcceptedAttribute, false);
                string fusionOrderNumber = await order.GetFusionOrderNumberAsync();
                IActionResult baseResult = await base.Completed(orderId);

                //TODO: find a better way of extending models. Maybe via model factory
                CheckoutCompletedModel baseModel = (CheckoutCompletedModel)((ViewResult)baseResult).Model;
                string baseSerialized = JsonConvert.SerializeObject(baseModel);
                FiluetCheckoutCompleteModel model =
                    JsonConvert.DeserializeObject<FiluetCheckoutCompleteModel>(baseSerialized);
                model.FusionOrderId = fusionOrderNumber;
                var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
                model.ApfCompleted =
                    orderItems.Any(oi => oi.ProductId == ApfApfShoppingCartHelper.GetApfProductAsync(customer).Id);
                if (model.ApfCompleted)
                {
                    await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                        CoreGenericAttributes.MainConditionsAcceptedAttribute, false);
                    await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                        CoreGenericAttributes.ApfMessageAcceptedAttribute, true);
                }

                return View(model);
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync($"[FiluetCheckoutController].[Completed] Exception. OrderId={orderId}", exc);

                return RedirectToRoute("HomePage");
            }
        }

        public override async Task<IActionResult> Confirm()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart =
                await _shoppingCartService.GetShoppingCartAsync(currentCustomer,
                    storeId: _storeContext.GetCurrentStore().Id);
            //validation
            var cart = shoppingCart
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .ToList();

            if (!cart.Any())
            {
                return RedirectToRoute("ShoppingCart");
            }

            if (_orderSettings.OnePageCheckoutEnabled)
            {
                return RedirectToRoute("CheckoutOnePage");
            }

            if (await _customerService.IsGuestAsync(currentCustomer) && !_orderSettings.AnonymousCheckoutAllowed)
            {
                return new UnauthorizedResult();
            }

            bool isDebtor = await currentCustomer.IsDebtorAsync();

            //if customer is debtor check the item is APF
            if (isDebtor)
            {
                var productCategoriesByProductId =
                    await _categoryService.GetProductCategoriesByProductIdAsync(cart[0].ProductId);
                bool anyMaintenanceCategoryType = await productCategoriesByProductId
                    .AnyAwaitAsync(async x => ((await _genericAttributeService.GetAttributesForEntityAsync(x.CategoryId, nameof(Category)))
                                  .FirstOrDefault(attr => attr.Key == CategoryAttributeNames.CategoryType)
                                  ?.Value) ==
                              nameof(CategoryTypeEnum.Maintenance));
                if (cart.Count != 1 || !anyMaintenanceCategoryType)
                {
                    throw new Exception(await _localizationService.GetResourceAsync("Customer.ApfProductIsRequired"));
                }
            }

            //model
            var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);

            try
            {
                var processPaymentRequest =
                    _httpContextAccessor.HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                    {
                        return RedirectToRoute("CheckoutPaymentInfo");
                    }

                    processPaymentRequest = new ProcessPaymentRequest();
                }

                //prevent 2 orders being placed within an X seconds time frame
                if (!await IsMinimumOrderPlacementIntervalValidAsync(currentCustomer))
                {
                    throw new Exception(
                        await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));
                }

                //place order
                processPaymentRequest.StoreId = _storeContext.GetCurrentStore().Id;
                processPaymentRequest.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;

                processPaymentRequest.PaymentMethodSystemName 
                    =await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.SelectedPaymentMethodAttribute, _storeContext.GetCurrentStore().Id);
                var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    _httpContextAccessor.HttpContext.Session.Remove("OrderPaymentInfo");
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };

                    await _logger.InformationAsync($"PostProcessPayment of paymentMethod is being invoked");
                    await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);
                    await _logger.InformationAsync($"PostProcessPayment of paymentMethod is invoked");

                    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                    {
                        // UNDONE probably is necessary to remove the debtor flag here
                        //redirection or POST has been done in PostProcessPayment
                        return Content("Redirected");
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = placeOrderResult.PlacedOrder.Id });
                }

                foreach (var error in placeOrderResult.Errors)
                {
                    model.Warnings.Add(error);
                }
            }
            catch (Exception exc)
            {
                await _logger.WarningAsync(exc.Message, exc);
                model.Warnings.Add(exc.Message);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        public override async Task<IActionResult> OpcConfirmOrder(bool captchaValid)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            try
            {
                if (await customer.GetCantBuyFlagAsync())
                {
                    return Json(new
                    {
                        error = 1,
                        message = _localizationService.GetResourceAsync(
                            "HBL.Baltic.OnlineOrdering.ShoppingPlugin.Resources.CantBuy")
                    });
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"FiluetShoppingCartService.AddToCartAsync CustomerId = {customer?.Id}", ex);
            }

            #region Custom Changes

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await _logger.InformationAsync($"[OpcConfirmOrder] Start. ");
               
                var isCaptchaSettingEnabled = await _customerService.IsGuestAsync(customer) &&
                    _captchaSettings.Enabled && _captchaSettings.ShowOnCheckoutPageForGuests;

                var confirmOrderModel = new CheckoutConfirmModel()
                {
                    DisplayCaptcha = isCaptchaSettingEnabled
                };

                //captcha validation for guest customers
                if (!isCaptchaSettingEnabled || (isCaptchaSettingEnabled && captchaValid))
                {
                    //validation
                    if (_orderSettings.CheckoutDisabled)
                        throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                    var store = await _storeContext.GetCurrentStoreAsync();
                    var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);

                    if (!cart.Any())
                        throw new Exception("Your cart is empty");

                    if (!_orderSettings.OnePageCheckoutEnabled)
                        throw new Exception("One page checkout is disabled");

                    if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                        throw new Exception("Anonymous checkout is not allowed");

                    //prevent 2 orders being placed within an X seconds time frame
                    if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                        throw new Exception(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

                    //place order
                    var processPaymentRequest = HttpContext.Session.Get<ProcessPaymentRequest>("OrderPaymentInfo");
                    if (processPaymentRequest == null)
                    {
                        //Check whether payment workflow is required
                        if (await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart))
                        {
                            throw new Exception("Payment information is not entered");
                        }

                        processPaymentRequest = new ProcessPaymentRequest();
                    }
                    _paymentService.GenerateOrderGuid(processPaymentRequest);
                    processPaymentRequest.StoreId = store.Id;
                    processPaymentRequest.CustomerId = customer.Id;
                    processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                        NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                    HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", processPaymentRequest);
                    var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
                    if (placeOrderResult.Success)
                    {
                        HttpContext.Session.Set<ProcessPaymentRequest>("OrderPaymentInfo", null);
                        var postProcessPaymentRequest = new PostProcessPaymentRequest
                        {
                            Order = placeOrderResult.PlacedOrder
                        };

                        var paymentMethod = await _paymentPluginManager
                            .LoadPluginBySystemNameAsync(placeOrderResult.PlacedOrder.PaymentMethodSystemName, customer, store.Id);
                        if (paymentMethod == null)
                            //payment method could be null if order total is 0
                            //success
                            return Json(new { success = 1 });

                        if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                        {
                            //Redirection will not work because it's AJAX request.
                            //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                            stopwatch.Stop();
                            await _logger.InformationAsync($"[OpcConfirmOrder] PlaceOrderAsync() successfully finished. Processing time: {stopwatch.ElapsedMilliseconds} ms. PaymentMethodType = Redirection. ");

                            //redirect
                            return Json(new
                            {
                                redirect = $"{_webHelper.GetStoreLocation()}checkout/OpcCompleteRedirectionPayment"
                            });
                        }

                        await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                        stopwatch.Stop();
                        await _logger.InformationAsync($"[OpcConfirmOrder] PlaceOrderAsync() -> PostProcessPaymentAsync() successfully finished. Processing time: {stopwatch.ElapsedMilliseconds} ms.");
                        
                        //success
                        return Json(new { success = 1 });
                    }

                    //error
                    foreach (var error in placeOrderResult.Errors)
                        confirmOrderModel.Warnings.Add(error);
                }
                else
                {
                    confirmOrderModel.Warnings.Add(await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
                }

                return Json(new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order"
                });
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync($"{exc.Message}. Processing time: {stopwatch.ElapsedMilliseconds} ms.", exc, await _workContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }

            #endregion
        }

        public override async Task<IActionResult> OpcCompleteRedirectionPayment()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await _logger.InformationAsync("[OpcCompleteRedirectionPayment] Start.");

                //validation
                if (!_orderSettings.OnePageCheckoutEnabled)
                    return RedirectToRoute("Homepage");

                var customer = await _workContext.GetCurrentCustomerAsync();
                if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                    return Challenge();

                //get the order
                var store = await _storeContext.GetCurrentStoreAsync();
                var order = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1)).FirstOrDefault();
                if (order == null)
                    return RedirectToRoute("Homepage");

                var paymentMethod = await _paymentPluginManager
                    .LoadPluginBySystemNameAsync(order.PaymentMethodSystemName, customer, store.Id);
                if (paymentMethod == null)
                    return RedirectToRoute("Homepage");
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    return RedirectToRoute("Homepage");

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                    return RedirectToRoute("Homepage");

                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };

                await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                stopwatch.Stop();
                await _logger.InformationAsync($"[OpcCompleteRedirectionPayment] PostProcessPaymentAsync() successfully finished. Processing time: {stopwatch.ElapsedMilliseconds} ms.");

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    return Content(await _localizationService.GetResourceAsync("Checkout.RedirectMessage"));
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception exc)
            {
                stopwatch.Stop();
                await _logger.ErrorAsync($"{exc.Message}. Processing time: {stopwatch.ElapsedMilliseconds} ms.", exc, await _workContext.GetCurrentCustomerAsync());
                return Content(exc.Message);
            }
        }

        public override async Task<IActionResult> OpcSavePaymentInfo(IFormCollection form)
        {
            var inn = form["inn"];
            if (!string.IsNullOrEmpty(inn))
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                    CoreGenericAttributes.CustomerInnAttribute, inn);
            bool? isShipInvoiceWithOrder = !string.IsNullOrEmpty(form["IsShipInvoiceWithOrder"])
                ? (bool?)Convert.ToBoolean(form["IsShipInvoiceWithOrder"])
                : null;
            if (isShipInvoiceWithOrder.HasValue)
            {
                await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                    CustomerAttributeNames.IsShipInvoiceWithOrder, isShipInvoiceWithOrder.Value);
            }

            #region Custom Changes

            try
            {
                //validation
                if (_orderSettings.CheckoutDisabled)
                    throw new Exception(await _localizationService.GetResourceAsync("Checkout.Disabled"));

                var customer = await _workContext.GetCurrentCustomerAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                var paymentMethod = await _paymentPluginManager
                    .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id)
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

                    var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order"
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
                await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }

            #endregion
        }

        protected override async Task<JsonResult> OpcLoadStepAfterShippingMethod(IList<ShoppingCartItem> cart)
        {
            try
            {
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
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

                    if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                        paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                    {
                        //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                        //so customer doesn't have to choose a payment method

                        var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                        await _genericAttributeService.SaveAttributeAsync(currentCustomer,
                            NopCustomerDefaults.SelectedPaymentMethodAttribute,
                            selectedPaymentMethodSystemName, _storeContext.GetCurrentStore().Id);

                        var pluginDescriptorBySystemName = await _pluginService
                            .GetPluginDescriptorBySystemNameAsync<IPaymentMethod>(selectedPaymentMethodSystemName,
                                storeId: _storeContext.GetCurrentStore().Id, customer: currentCustomer);
                        var paymentMethodInst = pluginDescriptorBySystemName.Instance<IPaymentMethod>();
                        if (paymentMethodInst == null ||
                            !await _paymentPluginManager.IsPluginActiveAsync(pluginDescriptorBySystemName.SystemName,
                                currentCustomer, _storeContext.GetCurrentStore()?.Id ?? 0))
                            throw new Exception("Selected payment method can't be parsed");

                        return await OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                    }

                    //customer have to choose a payment method
                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "payment-method",
                            html = await RenderPartialViewToStringAsync(
                                "~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views" +
                                "/FiluetCheckout/OpcPaymentMethods.cshtml", paymentMethodModel)
                        },
                        goto_section = "payment_method"
                    });
                }

                //payment is not required
                await _genericAttributeService.SaveAttributeAsync<string>(currentCustomer,
                    NopCustomerDefaults.SelectedPaymentMethodAttribute, null, _storeContext.GetCurrentStore().Id);
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(Nop.Core.Domain.Logging.LogLevel.Error, "[PERF DEBUG] OpcLoadStepAfterShippingMethod error.", ex.ToString());
            }

            var confirmOrderModel = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "confirm-order",
                    html = await RenderPartialViewToStringAsync("OpcConfirmOrder", confirmOrderModel)
                },
                goto_section = "confirm_order"
            });
        }

        #endregion

    }
}
