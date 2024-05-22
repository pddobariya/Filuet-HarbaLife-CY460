using FluentValidation.Results;
using HBL.Baltic.OnlineOrdering.Payments.Payeezy;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.Payeezy.Components;
using Nop.Plugin.Payments.Payeezy.Models;
using Nop.Plugin.Payments.Payeezy.SchedulerTask;
using Nop.Plugin.Payments.Payeezy.Services;
using Nop.Plugin.Payments.Payeezy.Validators;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Payments.Payeezy
{
    public class PayeezyPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Properties

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugin.Payments.Payeezy.PaymentMethodDescription");
        }

        #endregion

        #region Fields

        private ISettingService _settingsService;
        private ICustomerService _customerService;
        private IOrderService _orderService;
        private IWebHelper _webHelper;
        private IWorkContext _workContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private PayeezyPaymentSettings _payeezySettings;
        private IScheduleTaskService _scheduleTaskService;
        private ILogger _logger;
        private ILocalizationService _localizationService;
        private IFusionIntegrationService _fusionIntegrationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryService _countryService;
        private readonly IAddressService _addressService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILanguageService _languageService;
        private readonly INopFileProvider _fileProvider;

        #endregion

        #region Ctor

        public PayeezyPaymentProcessor(PayeezyPaymentSettings payeezySettings,
            ISettingService settingService,
            ICustomerService customerService,
            IOrderService orderService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IHttpContextAccessor httpContextAccessor,
            IScheduleTaskService scheduleTaskService,
            ILogger logger,
            ILocalizationService localizationService,
            IFusionIntegrationService fusionIntegrationService,
            IGenericAttributeService genericAttributeService,
            ICountryService countryService,
            IAddressService addressService,
            IShoppingCartService shoppingCartService,
            ILanguageService languageService,
            INopFileProvider fileProvider)
        {
            //enable total override using Fusion
            PayeezyOrderProcessingService.IsOverrideOrderTotal = true;
            _settingsService = settingService;
            _customerService = customerService;
            _orderService = orderService;
            _webHelper = webHelper;
            _workContext = workContext;
            _httpContextAccessor = httpContextAccessor;
            _payeezySettings = payeezySettings;
            _scheduleTaskService = scheduleTaskService;
            _logger = logger;
            _localizationService = localizationService;
            _fusionIntegrationService = fusionIntegrationService;
            _genericAttributeService = genericAttributeService;
            _countryService = countryService;
            _addressService = addressService;
            _shoppingCartService = shoppingCartService;
            _languageService = languageService;
            _fileProvider = fileProvider;
        }

        #endregion

        #region Methods

        public async Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("CancelRecurringPayment method not supported");
            return await Task.FromResult(result);
        }

        public async Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            return await Task.FromResult(true);
        }

        public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            List<string> warnings = new List<string>();

            PaymentInfoValidator validator = new PaymentInfoValidator(_localizationService);
            PaymentInfoModel model = new PaymentInfoModel
            {
                IsShipInvoiceWithOrder = form["IsShipInvoiceWithOrder"].FirstOrDefault() != null ? (bool?)Convert.ToBoolean(form["IsShipInvoiceWithOrder"]) : null

            };
            ValidationResult validationResult = validator.Validate(model);
            if (!validationResult.IsValid)
            {
                warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));
            }
            else
            {
            }
            //check IsShipInvoiceWithOrder
            if (!model.IsShipInvoiceWithOrder.HasValue)
            {
                warnings.Add(await _localizationService.GetResourceAsync("Plugins.Payments.Payeezy.Fields.IsShipInvoiceWithOrder.NotSelected"));
            }
            return warnings;
        }

        public async Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            //update system email and billing address and IsShipInvoiceWithOrder
            Customer currentCustomer = await _workContext.GetCurrentCustomerAsync();
            if (currentCustomer != null)
            {
                bool? isShipInvoiceWithOrder = form["IsShipInvoiceWithOrder"].FirstOrDefault() != null ? (bool?)Convert.ToBoolean(form["IsShipInvoiceWithOrder"]) : null;
                if (isShipInvoiceWithOrder.HasValue)
                {
                    await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.IsShipInvoiceWithOrder, isShipInvoiceWithOrder.Value);
                }
                Address address = currentCustomer.BillingAddressId != null ? await _addressService.GetAddressByIdAsync(currentCustomer.BillingAddressId.Value) : null;
                await _logger.InformationAsync(string.Format("[Payeezy.GetPaymentInfoAsync] Billing address exists: {0}", address != null));

                string firstName = NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
                string lastName = NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
                string fullName = (await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CustomerAttributeNames.SelectedShippingFullname))?.Trim();
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    firstName = fullName.Split(' ')[0];
                    lastName = fullName.Split(' ').Length > 1 ? fullName.Split(' ')[1] : NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
                }
                var countryCode = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CustomerAttributeNames.CountryOrderOfProcessing) ?? await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CustomerAttributeNames.CountryOfProcessing);

                countryCode = string.IsNullOrWhiteSpace(countryCode) ? NopFiluetCommonDefaults.EmptyDisplayPlaceholder : countryCode;
               
                string address1 = string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.Address)) ? NopFiluetCommonDefaults.EmptyDisplayPlaceholder : await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.Address);
                string phone = string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.SelectedShippingPhoneNumber)) ? NopFiluetCommonDefaults.EmptyDisplayPlaceholder : await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.SelectedShippingPhoneNumber);
                string city = string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.City)) ? NopFiluetCommonDefaults.EmptyDisplayPlaceholder : await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.City);
                string zip = string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.ZipCode)) ? NopFiluetCommonDefaults.EmptyDisplayPlaceholder : await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.ZipCode);
                if (address == null)
                {
                    //create new address with blank entries
                    address = new Address()
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = currentCustomer.Email,
                        CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryCode)).Id,
                        Address1 = address1,
                        PhoneNumber = phone,
                        ZipPostalCode = zip,
                        City = city
                    };

                    await _addressService.InsertAddressAsync(address);
                    await _customerService.InsertCustomerAddressAsync(currentCustomer, address);
                    //currentCustomer.Addresses.Add(address);
                }
                else
                {
                    address.FirstName = firstName;
                    address.LastName = lastName;
                    address.CountryId = (await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryCode)).Id;
                    address.Address1 = address1;
                    address.PhoneNumber = phone;
                    address.ZipPostalCode = zip;
                    address.City = city;
                    await _addressService.UpdateAddressAsync(address);
                }
                currentCustomer.BillingAddressId = address.Id;
                await _customerService.UpdateCustomerAsync(currentCustomer);
                //}
            }
            return new ProcessPaymentRequest();
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(0m);
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentPayeezy/Configure";
        }

        public async Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return await Task.FromResult(false);
        }

        public override async Task InstallAsync()
        {
            try
            {
                //settings
                PayeezyPaymentSettings settings = new PayeezyPaymentSettings()
                {
                    IsSandbox = true,
                    APISandboxEndpoint = "https://secureshop-test.firstdata.lv:8443/ecomm/MerchantHandler",
                    APIProductionEndpoint = "",
                    SandboxClientCertificateThumbprint = "80FF28C4854E4113B3F03687CBCE45955EBA646F",
                    ProductionClientCertificateThumbprint = "",
                    SandboxPaymentRedirectUrl = "https://secureshop-test.firstdata.lv/ecomm/ClientHandler?trans_id={0}",
                    ProductionPaymentRedirectUrl = "",
                    PluginControllerName = "PaymentPayeezy",
                    APISandboxReturnOkAction = "PaymentComplete",
                    APISandboxReturnFailAction = "PaymentError",
                    APISandboxMobilePaymentAction = "PaymentMobile",
                    MobilePaymentQuerystring = "?s=m"
                };

                await _settingsService.SaveSettingAsync(settings);

                //schedule task
                ScheduleTask task = await _scheduleTaskService.GetTaskByTypeAsync(PayeezyPaymentScheduleTask.TaskType);
                if (task == null)
                {
                    await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                    {
                        Name = PayeezyPaymentScheduleTask.Name,
                        Seconds = 12 * 3600, //12h
                        Type = PayeezyPaymentScheduleTask.TaskType,
                        Enabled = true,
                        StopOnError = false
                    });
                }

                await base.InstallAsync();
            }
            catch (Exception ex)
            {
                await LogError("Install", ex);
            }
            await InstallLocaleResources();
        }

        private async Task<PayeezyManager> InitializePayeezyManager()
        {
            try
            {
                string bankUrl = _payeezySettings.IsSandbox ? _payeezySettings.APISandboxEndpoint : _payeezySettings.APIProductionEndpoint;
                string certKey = _payeezySettings.IsSandbox ? _payeezySettings.SandboxClientCertificateThumbprint : _payeezySettings.ProductionClientCertificateThumbprint;
                PayeezyManager payeezyManager = new PayeezyManager(bankUrl, null, certKey);
                return payeezyManager;
            }
            catch (Exception ex)
            {
                await LogError("InitializePayeezyManager", ex);
                return null;
            }
        }

        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {

            try
            {
                //Fusion
                Customer customer = await _workContext.GetCurrentCustomerAsync();
                ShoppingCartTotalModel cartTotal = null;
                try
                {
                    cartTotal = await _fusionIntegrationService.GetShoppingCartTotalAsync(customer);
                    var shoppingCartAsync = await _shoppingCartService.GetShoppingCartAsync(customer);
                    if (cartTotal.TotalDue == 0 && shoppingCartAsync != null && shoppingCartAsync.Any())
                    {
                        await _logger.ErrorAsync(string.Format("{0}: Error calling GetShoppingCartTotal Fusion method. Method returned TotalDue=0; TotalAmount={1}; TotalTaxAmount={2}",
                            "PayeezyPaymentProcessor.ProcessPayment", cartTotal.TotalAmount, cartTotal.TotalTaxAmount), null, customer);
                        //init stub data
                        cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);
                    }
                }
                catch (Exception exc)
                {
                    //log failed Fusion call
                    await _logger.ErrorAsync("PayeezyPaymentProcessor.ProcessPayment: Error calling GetShoppingCartTotal Fusion method.", exc, customer);
                    //init stub data
                    cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);
                }


                decimal total = cartTotal != null ? cartTotal.TotalDue : 0;

                processPaymentRequest.OrderTotal = total;
                var result = new ProcessPaymentResult();
                result.NewPaymentStatus = PaymentStatus.Pending;
                return result;
            }
            catch (Exception ex)
            {
                await LogError("ProcessPayment", ex);
                return null;
            }
        }

        public async Task<PayeezyReversalAPIResponse> ReverseTransaction(string transactionId, decimal amount)
        {
            PayeezyManager payeezyManager =await InitializePayeezyManager();
            return payeezyManager.ReverseTransaction(transactionId, amount);
        }

        //initial registration of transaction in payment gateway API
        public async Task<PayeezyStartTransactionAPIResponse> InitializePaymentTransaction(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                PayeezyManager payeezyManager =await InitializePayeezyManager();
                Customer customer = await _customerService.GetCustomerByIdAsync(postProcessPaymentRequest.Order.CustomerId);

                string clientIp = _webHelper.GetCurrentIpAddress() ?? "";
                decimal amount = postProcessPaymentRequest.Order.OrderTotal;
                string purchaseDescription = await GetPaymentMethodDescriptionAsync();

                string lang = ConvertNopCommerceToPayeezyLanguage(await _workContext.GetWorkingLanguageAsync());

                await _logger.InformationAsync(string.Format("PayeezyManager.InitializeTransaction - request: type - {0}; clientIp - {1}; amount - {2:0.00}; description - {3}; lang - {4}", PayeezyPaymentTransactionTypes.SMS, clientIp, amount, purchaseDescription, lang));

                PayeezyStartTransactionAPIResponse response = payeezyManager.InitializeTransaction(PayeezyPaymentTransactionTypes.SMS, clientIp, amount, purchaseDescription, lang);
                await _logger.InformationAsync(string.Format("PayeezyManager.InitializeTransaction - response: {0}", JsonConvert.SerializeObject(response)));
                if (response == null || response.IsFailed)
                {
                    if (response == null)
                    {
                        await _logger.ErrorAsync("Error registering transaction: transaction is null");
                    }
                    else
                    {
                        await _logger.ErrorAsync("Error registering transaction. API response: " + response.ToString());
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                await LogError("InitializePaymentTransaction", ex);
                return null;
            }
        }

        //initiates redirection to payment gateway website
        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                string url = await GetPaymentGatewayRedirectUrl(postProcessPaymentRequest);
                _httpContextAccessor.HttpContext.Response.Redirect(url);
            }
            catch (Exception ex)
            {
                await LogError("PostProcessPayment", ex);
            }
        }

        public async Task<string> GetPaymentGatewayRedirectUrl(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            PayeezyStartTransactionAPIResponse response = await InitializePaymentTransaction(postProcessPaymentRequest);
            string transactionId = response != null ? response.TransactionId : null;
            await _logger.InformationAsync("Payeezy transactionId: " + transactionId);
            postProcessPaymentRequest.Order.AuthorizationTransactionId = transactionId;
            postProcessPaymentRequest.Order.AuthorizationTransactionResult = response != null ? response.ToString() : null;
            await _logger.InformationAsync("Payeezy Order.AuthorizationTransactionId (before): " + postProcessPaymentRequest.Order.AuthorizationTransactionId);
            await _orderService.UpdateOrderAsync(postProcessPaymentRequest.Order);
            string url = _payeezySettings.IsSandbox ? _payeezySettings.SandboxPaymentRedirectUrl : _payeezySettings.ProductionPaymentRedirectUrl;
            await _logger.InformationAsync("Payeezy IsSandbox: " + _payeezySettings.IsSandbox);
            await _logger.InformationAsync("Payeezy url template: " + url);
            await _logger.InformationAsync("Payeezy Order.AuthorizationTransactionId (after): " + postProcessPaymentRequest.Order.AuthorizationTransactionId);
            url = string.Format(url, HttpUtility.UrlEncode(postProcessPaymentRequest.Order.AuthorizationTransactionId));
            await _logger.InformationAsync("Payeezy redirect to: " + url);
            return url;
        }

        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return await Task.FromResult(result);
        }


        public async Task<PayeezyTransactionResultAPIResponse> ValidatePaymentTransaction(string transactionId, string clientIp)
        {
            try
            {
                PayeezyManager payeezyManager =await InitializePayeezyManager();
                string defaultErrorMessage = PayeezyTransactionResults.GetResultMessage(PayeezyTransactionResults.Default);
                PayeezyTransactionResultAPIResponse trans = payeezyManager.GetTransactionResult(transactionId, clientIp);

                return trans;
            }
            catch (Exception ex)
            {
                await LogError("ValidatePaymentTransaction", ex);
                return null;
            }
        }


        public async Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("ProcessRecurringPayment method not supported");
            return await Task.FromResult(result);
        }

        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return await Task.FromResult(result);
        }

        #region Resourcestring
        protected virtual async Task InstallLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/Payments.Payeezy/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, string.Format("ResourceString_{0}.xml", language.UniqueSeoCode)))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }

        }
        protected virtual async Task DeleteLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/Payments.Payeezy/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, string.Format("ResourceString_{0}.xml", language.UniqueSeoCode)))
                {
                    var languageResourceNames = from name in XDocument.Load(filePath).Document.Descendants("LocaleResource")
                                                select name.Attribute("Name").Value;

                    foreach (var item in languageResourceNames)
                    {
                        await _localizationService.DeleteLocaleResourcesAsync(item);
                    }
                }
            }

        }
        #endregion
        public override async Task UninstallAsync()
        {
            try
            {
                //settings
                await _settingsService.DeleteSettingAsync<PayeezyPaymentSettings>();

                //schedule task
                ScheduleTask task = await _scheduleTaskService.GetTaskByTypeAsync(PayeezyPaymentScheduleTask.TaskType);
                if (task != null)
                {
                    await _scheduleTaskService.DeleteTaskAsync(task);
                }

                await base.UninstallAsync();
            }
            catch (Exception ex)
            {
                await LogError("Uninstall", ex);
            }
            await DeleteLocaleResources();
        }

        public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return await Task.FromResult(result);
        }

        //TODO:move to helpers and use dictionary
        private string ConvertNopCommerceToPayeezyLanguage(Language nopLang)
        {
            string lang = "en";
            if (nopLang != null)
            {
                string nopLangName = nopLang.Name.ToLower().Trim();
                string nopLangCulture = nopLang.LanguageCulture.ToLower().Trim();
                if (nopLangName == "russian" || nopLangCulture == "ru-ru")
                {
                    lang = "ru";
                }
                else if (nopLangName == "latvian" || nopLangCulture == "lv-lv")
                {
                    lang = "lv";
                }
                else if (nopLangName == "lithuanian" || nopLangCulture == "lt-lt")
                {
                    lang = "lt";
                }
                else if (nopLangName == "estonian" || nopLangCulture == "et-ee")
                {
                    lang = "et";
                }
            }
            return lang;
        }

        private async Task LogError(string method, Exception ex)
        {
            await _logger.ErrorAsync(string.Format("Plugins.Payments.Payeezy:{0}", method), ex);
        }

        public Type GetPublicViewComponent()
        {
            return typeof(PaymentPayeezyViewComponent);
        }

        #endregion
    }
}
