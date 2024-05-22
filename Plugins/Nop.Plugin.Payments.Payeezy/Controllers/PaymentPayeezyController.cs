using HBL.Baltic.OnlineOrdering.Payments.Payeezy;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Payeezy.Dto;
using Nop.Plugin.Payments.Payeezy.Models;
using Nop.Plugin.Payments.Payeezy.Services;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IShoppingCartService = Nop.Services.Orders.IShoppingCartService;

namespace Nop.Plugin.Payments.Payeezy.Controllers
{

    [JetBrains.Annotations.AspMvcSuppressViewError]
    public class PaymentPayeezyController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILogger _logger;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly ILanguageService _languageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITokenService _tokenService;
        private readonly IAddressService _addressService;
        private readonly IStoreContext _storeContext;
        private readonly IPluginService _pluginService;
        private readonly IFiluetOrderService _filuetOrderService;
        private readonly INotificationService _notificationService;
        private const string _mobileOriginOrderNotesKey = "Payments.Payeezy.PaymentMobile";
        private const string _mobileCurrentLanguageSessionKey = "Payments.Payeezy.CurrentLanguageMobile";
        private const string _previousLanguageSessionKey = "Payments.Payeezy.PreviousLanguage";
        private const string PATH_TO_VIEW = @"~/Plugins/Payments.Payeezy/Views/PaymentPayeezy/";

        #endregion

        #region Ctor

        public PaymentPayeezyController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService, 
            ILogger logger, 
            PaymentSettings paymentSettings,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IWebHelper webHelper, 
            ILanguageService languageService,
            IHttpContextAccessor httpContextAccessor,
            IFusionIntegrationService fusionIntegrationService,
            IGenericAttributeService genericAttributeService,
            IAuthenticationService authenticationService,
            IShoppingCartService shoppingCartService, 
            ITokenService tokenService,
            IFiluetOrderService filuetOrderService,
            IAddressService addressService,
            IStoreContext storeContext,
            IPluginService pluginService,
            INotificationService notificationService)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _paymentService = paymentService;
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _logger = logger;
            _paymentSettings = paymentSettings;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _customerService = customerService;
            _languageService = languageService;
            _httpContextAccessor = httpContextAccessor;
            _fusionIntegrationService = fusionIntegrationService;
            _genericAttributeService = genericAttributeService;
            _authenticationService = authenticationService;
            _shoppingCartService = shoppingCartService;
            _tokenService = tokenService;
            _addressService = addressService;
            _storeContext = storeContext;
            _pluginService = pluginService;
            _filuetOrderService = filuetOrderService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            PayeezyPaymentSettings payeezyPaymentSettings =await _settingService.LoadSettingAsync<PayeezyPaymentSettings>(storeScope);

            ConfigurationModel model = new ConfigurationModel();
            model.IsSandbox = payeezyPaymentSettings.IsSandbox;
            model.APISandboxEndpoint = payeezyPaymentSettings.APISandboxEndpoint;
            model.APIProductionEndpoint = payeezyPaymentSettings.APIProductionEndpoint;
            model.SandboxClientCertificateThumbprint = payeezyPaymentSettings.SandboxClientCertificateThumbprint;
            model.ProductionClientCertificateThumbprint = payeezyPaymentSettings.ProductionClientCertificateThumbprint;
            model.SandboxPaymentRedirectUrl = payeezyPaymentSettings.SandboxPaymentRedirectUrl;
            model.ProductionPaymentRedirectUrl = payeezyPaymentSettings.ProductionPaymentRedirectUrl;

            model.APISandboxReturnOkAction = payeezyPaymentSettings.APISandboxReturnOkAction;
            model.APIProductionReturnOkAction = payeezyPaymentSettings.APIProductionReturnOkAction;
            model.APISandboxReturnFailAction = payeezyPaymentSettings.APISandboxReturnFailAction;
            model.APIProductionReturnFailAction = payeezyPaymentSettings.APIProductionReturnFailAction;
            model.APISandboxMobilePaymentAction = payeezyPaymentSettings.APISandboxMobilePaymentAction;
            model.APIProductionMobilePaymentAction = payeezyPaymentSettings.APIProductionMobilePaymentAction;
            model.PluginControllerName = payeezyPaymentSettings.PluginControllerName;
            model.MobilePaymentQuerystring = payeezyPaymentSettings.MobilePaymentQuerystring;


            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.IsSandbox_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.IsSandbox, storeScope);
                model.APISandboxEndpoint_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APISandboxEndpoint, storeScope);
                model.APIProductionEndpoint_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APIProductionEndpoint, storeScope);
                model.SandboxClientCertificateThumbprint_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.SandboxClientCertificateThumbprint, storeScope);
                model.ProductionClientCertificateThumbprint_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.ProductionClientCertificateThumbprint, storeScope);
                model.SandboxPaymentRedirectUrl_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.SandboxPaymentRedirectUrl, storeScope);
                model.ProductionPaymentRedirectUrl_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.ProductionPaymentRedirectUrl, storeScope);

                model.APISandboxReturnOkAction_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APISandboxReturnOkAction, storeScope);
                model.APIProductionReturnOkAction_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APIProductionReturnOkAction, storeScope);
                model.APISandboxReturnFailAction_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APISandboxReturnFailAction, storeScope);
                model.APIProductionReturnFailAction_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APIProductionReturnFailAction, storeScope);
                model.APISandboxMobilePaymentAction_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APISandboxMobilePaymentAction, storeScope);
                model.APIProductionMobilePaymentAction_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.APIProductionMobilePaymentAction, storeScope);

                model.PluginControllerName_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.PluginControllerName, storeScope);
                model.MobilePaymentQuerystring_OverrideForStore =await _settingService.SettingExistsAsync(payeezyPaymentSettings, x => x.MobilePaymentQuerystring, storeScope);
            }
            return View(GetView("Configure"), model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<ActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return (ActionResult)await Configure();
            }

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            PayeezyPaymentSettings payeezyPaymentSettings = await _settingService.LoadSettingAsync<PayeezyPaymentSettings>(storeScope);

            //save settings
            payeezyPaymentSettings.IsSandbox = model.IsSandbox;
            payeezyPaymentSettings.APIProductionEndpoint = model.APIProductionEndpoint;
            payeezyPaymentSettings.APISandboxEndpoint = model.APISandboxEndpoint;
            payeezyPaymentSettings.SandboxClientCertificateThumbprint = model.SandboxClientCertificateThumbprint;
            payeezyPaymentSettings.ProductionClientCertificateThumbprint = model.ProductionClientCertificateThumbprint;
            payeezyPaymentSettings.SandboxPaymentRedirectUrl = model.SandboxPaymentRedirectUrl;
            payeezyPaymentSettings.ProductionPaymentRedirectUrl = model.ProductionPaymentRedirectUrl;

            payeezyPaymentSettings.APISandboxReturnOkAction = model.APISandboxReturnOkAction;
            payeezyPaymentSettings.APIProductionReturnOkAction = model.APIProductionReturnOkAction;
            payeezyPaymentSettings.APISandboxReturnFailAction = model.APISandboxReturnFailAction;
            payeezyPaymentSettings.APIProductionReturnFailAction = model.APIProductionReturnFailAction;
            payeezyPaymentSettings.APISandboxMobilePaymentAction = model.APISandboxMobilePaymentAction;
            payeezyPaymentSettings.APIProductionMobilePaymentAction = model.APIProductionMobilePaymentAction;

            payeezyPaymentSettings.PluginControllerName = model.PluginControllerName;
            payeezyPaymentSettings.MobilePaymentQuerystring = model.MobilePaymentQuerystring;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.IsSandbox_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.IsSandbox, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.IsSandbox, storeScope);
            }

            if (model.APISandboxEndpoint_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APISandboxEndpoint, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APISandboxEndpoint, storeScope);
            }

            if (model.APIProductionEndpoint_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APIProductionEndpoint, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APIProductionEndpoint, storeScope);
            }

            if (model.SandboxClientCertificateThumbprint_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.SandboxClientCertificateThumbprint, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.SandboxClientCertificateThumbprint, storeScope);
            }

            if (model.ProductionClientCertificateThumbprint_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.ProductionClientCertificateThumbprint, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.ProductionClientCertificateThumbprint, storeScope);
            }

            if (model.SandboxPaymentRedirectUrl_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.SandboxPaymentRedirectUrl, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.SandboxPaymentRedirectUrl, storeScope);
            }

            if (model.ProductionPaymentRedirectUrl_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.ProductionPaymentRedirectUrl, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.ProductionPaymentRedirectUrl, storeScope);
            }


            if (model.APISandboxReturnOkAction_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APISandboxReturnOkAction, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APISandboxReturnOkAction, storeScope);
            }

            if (model.APISandboxReturnFailAction_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APISandboxReturnFailAction, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APISandboxReturnFailAction, storeScope);
            }

            if (model.APISandboxMobilePaymentAction_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APISandboxMobilePaymentAction, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APISandboxMobilePaymentAction, storeScope);
            }

            if (model.APISandboxReturnOkAction_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APISandboxReturnOkAction, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APISandboxReturnOkAction, storeScope);
            }

            if (model.APIProductionReturnOkAction_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APIProductionReturnOkAction, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APIProductionReturnOkAction, storeScope);
            }

            if (model.APIProductionReturnFailAction_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.APIProductionReturnFailAction, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.APIProductionReturnFailAction, storeScope);
            }


            if (model.PluginControllerName_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.PluginControllerName, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.PluginControllerName, storeScope);
            }

            if (model.MobilePaymentQuerystring_OverrideForStore || storeScope == 0)
            {
                await _settingService.SaveSettingAsync(payeezyPaymentSettings, x => x.MobilePaymentQuerystring, storeScope, false);
            }
            else if (storeScope > 0)
            {
                await _settingService.DeleteSettingAsync(payeezyPaymentSettings, x => x.MobilePaymentQuerystring, storeScope);
            }

            //now clear settings cache
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return (ActionResult)await Configure();
        }

        public async Task<ActionResult> PaymentMobile(string t, string l)
        {
            try
            {
                await _tokenService.AuthenticateCustomerByTokenAsync(t);

                SaveCurrentLanguage(_mobileCurrentLanguageSessionKey, l);
                SetCurrentMobileLanguageAsWorkingLanguage();
                PayeezyPaymentSettings payeezyPaymentSettings =await GetSetting();

                PayeezyOrderProcessingService.IsOverrideOrderTotal = true;
                ProcessPaymentRequest request = new ProcessPaymentRequest();
                request.CustomerId = (await _workContext.GetCurrentCustomerAsync()).Id;
                request.PaymentMethodSystemName = "Payments.Payeezy";
                int storeId = EngineContext.Current.Resolve<IStoreContext>().GetCurrentStore().Id;
                request.StoreId = storeId;

                PayeezyOrderProcessingService orderProcService = EngineContext.Current.Resolve<IOrderProcessingService>() as PayeezyOrderProcessingService;
                PlaceOrderResult orderResult =await orderProcService.PlaceOrderAsync(request);
                PostProcessPaymentRequest postProcRequest = new PostProcessPaymentRequest();
                postProcRequest.Order = orderResult.PlacedOrder;
                if (postProcRequest.Order != null)
                {
                    await _orderService.InsertOrderNoteAsync(new OrderNote()
                    {
                        Note = "[DEBUG]Payment origin: mobile app", //for humans
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = postProcRequest.Order.Id
                    });
                    //TODO: change on custom attributes
                    await _orderService.InsertOrderNoteAsync(new OrderNote()
                    {
                        Note = _mobileOriginOrderNotesKey, //for checking output views
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = postProcRequest.Order.Id
                    });
                    var descriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<PayeezyPaymentProcessor>("Payments.Payeezy");
                    var processor = descriptor.Instance<PayeezyPaymentProcessor>();
                    string url = await processor.GetPaymentGatewayRedirectUrl(postProcRequest);
                    return Redirect(url);
                }
                else
                {

                    await _logger.ErrorAsync("Order has not been placed successfully during performing payment in mobile app");


                    string reason =
                        (await _localizationService.GetLocaleStringResourceByNameAsync(
                            "Plugins.Payments.Payeezy.Fields.OrderNotFoundError",
                            (await _workContext.GetWorkingLanguageAsync()).Id)).ResourceValue;
                    return RedirectToAction("PaymentError", new { s = "m", reason = reason });
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("Payeezy PaymentMobile Unknown error", ex);

                string reason =
                    (await _localizationService.GetLocaleStringResourceByNameAsync(
                        "Plugins.Payments.Payeezy.Fields.OrderNotFoundError",
                        (await _workContext.GetWorkingLanguageAsync()).Id)).ResourceValue;
                return RedirectToAction("PaymentError", new { s = "m", reason = reason });
            }
        }

        public async Task<ActionResult> PaymentComplete(string trans_id, string s)
        {
            try
            {
                await _logger.InformationAsync("Payeezy PaymentComplete Start.");
                if (s == "m")
                {
                    ViewBag.ResetWorkingLanguageAction = new Action(ResetWorkingLanguage);
                    return View(GetView("PaymentMobileSuccess"));
                }
                return await ProcessPaymentResult(trans_id);
            }
            catch (Exception ex)
            {
                PaymentErrorModel m = new PaymentErrorModel(false, null);
                try
                {
                    await _logger.ErrorAsync("Payeezy PaymentComplete Unknown error ", ex);
                    return View(GetView("PaymentError"), m);
                }
                finally
                {
                }
            }
        }


        public async Task<ActionResult> TestReverseTransaction(string trans_id, decimal amount)
        {
            var descriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<PayeezyPaymentProcessor>("Payments.Payeezy");
            var processor = descriptor.Instance<PayeezyPaymentProcessor>();
            PayeezyReversalAPIResponse response =await processor.ReverseTransaction(trans_id, amount);
            return Json(new
            {
                result = response.Result,
                result_code = response.ResultCode

            });
        }

        public async Task<ActionResult> PaymentError(string trans_id, string s)
        {
            try
            {
                if (s == "m")
                {
                    PaymentErrorModel m = new PaymentErrorModel(false, null);
                    return View(GetView("PaymentError"), m);
                }
                return await ProcessPaymentResult(trans_id);
            }
            catch (Exception ex)
            {
                PaymentErrorModel m = new PaymentErrorModel(false, null);
                try
                {
                    await _logger.ErrorAsync("Payeezy PaymentError Unknown error", ex);
                    return View(GetView("PaymentError"), m);
                }
                finally
                {
                }
            }
        }

        private async Task<ActionResult> ProcessPaymentResult(string transactionId)
        {
            Order order = null;
            string errorRedirect = string.Empty;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                var descriptor = await _pluginService.GetPluginDescriptorBySystemNameAsync<PayeezyPaymentProcessor>("Payments.Payeezy");
                var processor = descriptor.Instance<PayeezyPaymentProcessor>();

                if (processor == null || await processor.HidePaymentMethodAsync(null))
                {
                    throw new NopException("Payeezy module cannot be loaded");
                }

                order = await _filuetOrderService.GetOrderByAuthorizationTransactionIdAndPaymentMethod(transactionId, processor.PluginDescriptor.SystemName);
                if (order == null)
                {
                    var paymentErrorModel = new PaymentErrorModel(false, "/");
                    ViewBag.ResetWorkingLanguageAction = new Action(ResetWorkingLanguage);

                    return View("PaymentError", paymentErrorModel);
                }

                string clientIp = HttpContext.Connection.RemoteIpAddress.ToString();
                var transaction =await processor.ValidatePaymentTransaction(transactionId, clientIp);

                bool isOriginMobile = (await _orderService.GetOrderNotesByOrderIdAsync(order.Id)).Any(x => x.Note == _mobileOriginOrderNotesKey);
                bool isFailed = transaction == null || transaction.IsFailed || transaction.Result != PayeezyTransactionResults.OK;

                string orderNotes = "PAYEEZY RESPONSE: " + (transaction == null ? PayeezyTransactionResults.Default.GetResultMessage() : transaction.ToString());
                await _orderService.InsertOrderNoteAsync(new OrderNote()
                {
                    Note = orderNotes,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id
                });

                await _orderService.UpdateOrderAsync(order);

                if (isFailed)
                {
                    //failed payment
                    string methodPath = "Plugins.Payments.Payeezy.ProcessPaymentResult";
                    if (transaction == null)
                        await _logger.ErrorAsync(string.Format("{0}:ValidatePaymentTransaction returns null", methodPath));
                    else
                        await _logger.ErrorAsync(string.Format("{0}: Error message: {1}", methodPath, transaction.ToString()));

                    if (isOriginMobile)
                    {
                        string reasonResName = "Plugins.Payments.Payeezy.Fields.DefaultPaymentErrorNoRedirect";
                        string reason = (await _localizationService.GetLocaleStringResourceByNameAsync(reasonResName,
                            (await _workContext.GetWorkingLanguageAsync()).Id)).ResourceValue;
                        return RedirectToAction("PaymentError", new { s = "m", reason = reason });
                    }

                    PaymentErrorModel m = new PaymentErrorModel(true, string.Format("/orderdetails/{0}", order.Id));
                    ViewBag.ResetWorkingLanguageAction = new Action(ResetWorkingLanguage);
                    return View("PaymentError", m);
                }



                if (_orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    order.AuthorizationTransactionResult = transaction.Result;
                    order.AuthorizationTransactionCode = transaction.ResultCode;
                    await _orderService.UpdateOrderAsync(order);
                    await _orderProcessingService.MarkOrderAsPaidAsync(order);

                    if (isOriginMobile)
                    {
                        return RedirectToAction("PaymentComplete", new { s = "m" });
                    }

                    return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }

                await _logger.InformationAsync($"Payeezy ProcessPaymentResult successfully finished. Processing time: {stopwatch.ElapsedMilliseconds} ms.");

                return RedirectToRoute("ShoppingCart");
            }
            catch (Exception ex)
            {
                PaymentErrorModel errorModel = new PaymentErrorModel(order != null, string.IsNullOrWhiteSpace(errorRedirect) ? "/" : errorRedirect);
                try
                {
                    stopwatch.Stop();
                    await _logger.ErrorAsync($"Payeezy ProcessPaymentResult Unknown error. Processing time: {stopwatch.ElapsedMilliseconds} ms.", ex);
                }
                finally
                {

                }
                return View("PaymentError", errorModel);
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        private void SaveCurrentLanguage(string key, string l)
        {
            HttpContext.Session.Set(key, l);
        }

        private string GetSavedLanguage(string key, string defaultValue = "en")
        {
            object l = HttpContext.Session.GetString(key);
            return l != null ? Convert.ToString(l) : defaultValue;
        }

        private async Task<PayeezyPaymentSettings> GetSetting()
        {
            int storeScope =await _storeContext.GetActiveStoreScopeConfigurationAsync();
            PayeezyPaymentSettings payeezyPaymentSettings = (await _settingService.LoadSettingAsync<PayeezyPaymentSettings>(storeScope));

            return payeezyPaymentSettings;
        }

        private async void SetCurrentMobileLanguageAsWorkingLanguage()
        {
            string iso6391 = GetSavedLanguage(_mobileCurrentLanguageSessionKey);
            Language prevLang = await _workContext.GetWorkingLanguageAsync();
            SaveCurrentLanguage(_previousLanguageSessionKey, prevLang.LanguageCulture.CultureToIso6391());
            SetCurrentLanguage(iso6391);
        }

        public async void ResetWorkingLanguage()
        {
            try
            {
                string iso6391 = GetSavedLanguage(_previousLanguageSessionKey, "");
                if (!string.IsNullOrWhiteSpace(iso6391))
                {
                    SetCurrentLanguage(iso6391);
                }
            }
            catch (Exception ex)
            {
                try
                {
                   await _logger.ErrorAsync("Payeezy ResetWorkingLanguage Unknown error", ex);
                }
                finally
                {

                }
            }
        }

        private async void SetCurrentLanguage(string iso6391)
        {
            try
            {
                int langId =await iso6391.Iso6391ToNopLanguageIdAsync();
                SetCurrentLanguage(langId);
            }
            catch (Exception ex)
            {
                try
                {
                    await _logger.ErrorAsync("Payeezy SetCurrentLanguage(string iso6391) Unknown error", ex);
                }
                finally
                {

                }
            }
        }

        private async void SetCurrentLanguage(int langId)
        {
            try
            {
                Language language = await _languageService.GetLanguageByIdAsync(langId);
                if (language != null && language.Published)
                {
                    await _workContext.SetWorkingLanguageAsync(language);
                }
            }
            catch (Exception ex)
            {
                try
                {
                   await _logger.ErrorAsync("Payeezy SetCurrentLanguage(int langId) Unknown error", ex);
                }
                finally
                {

                }
            }
        }

        private async Task<ActionResult> ProcessFailedPaymentAsync(PayeezyTransactionResultAPIResponse transaction, Order order)
        {
            string methodPath = "[PaymentPayeezyController].[ProcessFailedPaymentAsync]";
            string orderNotes;
            if (transaction == null)
            {
                orderNotes = PayeezyTransactionResults.Default.GetResultMessage();
                await _logger.ErrorAsync(string.Format("{0}:ValidatePaymentTransaction returns null", methodPath));
            }
            else
            {
                orderNotes = transaction.ToString();
                await _logger.ErrorAsync(string.Format("{0}: Error message: {1}", methodPath, orderNotes));
            }

            await _orderService.InsertOrderNoteAsync(new OrderNote()
            {
                Note = orderNotes,
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = order.Id
            });

            order.AuthorizationTransactionResult = transaction.Result;
            order.AuthorizationTransactionCode = transaction.ResultCode;
            await _orderService.UpdateOrderAsync(order);

            return RedirectToRoute("ShoppingCart");
        }

        private string GetView(string viewName)
            => $"{PATH_TO_VIEW}{viewName}.cshtml";

        #endregion
    }
}