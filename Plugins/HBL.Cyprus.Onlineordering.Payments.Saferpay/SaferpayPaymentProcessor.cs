using HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO;
using HBL.Cyprus.Onlineordering.Payments.Saferpay.Services;
using HBL.Uzbek.Onlineordering.Payments.Payme.Components;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Payment;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay
{
    public class SaferpayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields 

        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly ISaferpayService _saferpayService;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFiluetPaymentService _filuetPaymentService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly INopFileProvider _fileProvider;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;
        private readonly ICountryService _countryService;
        private readonly IAddressService _addressService;

        #endregion

        #region Ctor

        public SaferpayPaymentProcessor(ILocalizationService localizationService,
            IOrderService orderService,
            ISaferpayService saferpayService,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor,
            IFiluetPaymentService filuetPaymentService,
            IOrderProcessingService orderProcessingService,
            ISettingService settingService,
            IStoreContext storeContext,
            INopFileProvider fileProvider,
            ILanguageService languageService,
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            ILogger logger,
            ICustomerService customerService,
            ICountryService countryService,
            IAddressService addressService)
        {
            _localizationService = localizationService;
            _orderService = orderService;
            _saferpayService = saferpayService;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
            _filuetPaymentService = filuetPaymentService;
            _orderProcessingService = orderProcessingService;
            _settingService = settingService;
            _storeContext = storeContext;
            _fileProvider = fileProvider;
            _languageService = languageService;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
            _customerService = customerService;
            _countryService = countryService;
            _addressService = addressService;
        }

        #endregion

        #region Methods

        public bool SupportCapture => true;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public async Task<string> GetPaymentMethodDescriptionAsync()        {            return await _localizationService.GetResourceAsync("Plugins.Payment.PayByCard.PaymentMethodDescription");        }


        public async Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return await Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });

        }

        public async Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return await Task.FromResult(true);
        }

        public async Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            Order order = null;
            if (!string.IsNullOrEmpty(capturePaymentRequest.Order.CustomOrderNumber))
                order = await _orderService.GetOrderByCustomOrderNumberAsync(capturePaymentRequest.Order.CustomOrderNumber);
            else if (capturePaymentRequest.Order.OrderGuid != Guid.Empty)
                order = await _orderService.GetOrderByGuidAsync(capturePaymentRequest.Order.OrderGuid);

            try
            {
                var assertResponse = await _saferpayService.AssertAsync(order.AuthorizationTransactionId);
                if (assertResponse.Transaction.Status.Equals("AUTHORIZED", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!assertResponse.Liability.LiabilityShift || !assertResponse.Liability.ThreeDs.Authenticated) //check if 3D-Secure was completed)
                    {
                        await _saferpayService.CancelAsync(assertResponse.Transaction.Id);
                        return new CapturePaymentResult() { CaptureTransactionResult = "3DS-ERROR", Errors = new string[] { "We can't confirm that your 3D-Secure code was correct. Please try again." } };
                    }
                    await _orderProcessingService.MarkAsAuthorizedAsync(order);
                    order.CaptureTransactionId = assertResponse.Transaction.Id;

                    // capture
                    var captureResponse = await _saferpayService.CaptureAsync(order.CaptureTransactionId);
                    await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    await _orderService.UpdateOrderAsync(order);
                    return new CapturePaymentResult() { CaptureTransactionResult = "CAPTURED", NewPaymentStatus = PaymentStatus.Paid };
                }
                else if (assertResponse.Transaction.Status.Equals("CAPTURED", StringComparison.InvariantCultureIgnoreCase))
                {
                    await _orderProcessingService.MarkOrderAsPaidAsync(order);
                    return new CapturePaymentResult() { CaptureTransactionResult = "CAPTURED", NewPaymentStatus = PaymentStatus.Paid };
                }
                else if (assertResponse.Transaction.Status.Equals("CANCELED", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new CapturePaymentResult() { CaptureTransactionResult = "CANCELED", NewPaymentStatus = PaymentStatus.Pending };
                }
                return new CapturePaymentResult() { CaptureTransactionResult = "UNEXPECTED-ERROR", Errors = new string[] { "Sorry but an unexpected error occured. Please try again later." } };

            }
            catch (Exception e)
            {
                CapturePaymentResult result = new CapturePaymentResult() { CaptureTransactionResult = "ERROR", Errors = new List<string>() };
                if (e.Data != null && e.Data.Contains("Error") && e.Data["Error"] is SaferpayErrorResponse response)
                {
                    response = (SaferpayErrorResponse)e.Data["Error"];
                    if (response?.ErrorDetail != null)
                        result.Errors = response.ErrorDetail;

                    result.AddError(response?.ErrorMessage);

                    await _logger.ErrorAsync(string.Format("Error Message {0}.", response?.ErrorMessage),e, await _workContext.GetCurrentCustomerAsync());

                }
                else
                {
                    await _logger.ErrorAsync(string.Format("The order associated with GUID {0} is not available in the order table.", capturePaymentRequest.Order.OrderGuid), e, await _workContext.GetCurrentCustomerAsync());
                }

                return result;
            }
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(0m); ;
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Configuration/Configure";
        }

        public async Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return await Task.FromResult(false);
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = await _orderService.GetOrderByIdAsync(postProcessPaymentRequest.Order.Id);
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentSettings = await _settingService.LoadSettingAsync<SaferpayPaymentSettings>(storeScope);
            if (paymentSettings.Bypass)
            {
                order = await _orderService.GetOrderByIdAsync(postProcessPaymentRequest.Order.Id);
                order.AuthorizationTransactionId = "1234548765";
                await _orderService.UpdateOrderAsync(order);
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
                return;
            }

            var storeLocation = _webHelper.GetStoreLocation();
            try
            {
                var saferPayResponse = await _saferpayService.InitializeAsync(postProcessPaymentRequest.Order.OrderTotal, postProcessPaymentRequest.Order.CustomOrderNumber,
                   $"{storeLocation}PaymentSaferpay/PaySuccess?orderId={postProcessPaymentRequest.Order.Id}", $"{storeLocation}PaymentSaferpay/PayFailure?orderId={postProcessPaymentRequest.Order.Id}",
                   $"{storeLocation}PaymentSaferpay/Notify?guid={order.OrderGuid}", order.OrderGuid);
                order.AuthorizationTransactionId = saferPayResponse.Token;
                await _orderService.UpdateOrderAsync(order);

                _httpContextAccessor.HttpContext.Response.Redirect(saferPayResponse.RedirectUrl);
            }
            catch (Exception)
            {
                //throw e;
            }
        }

        public async Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return await _filuetPaymentService.ProcessPayment(processPaymentRequest);
        }

        public async Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return await Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return await Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
        }

        public async Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return await Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }

        public async override Task InstallAsync()
        {
            SaferpayPaymentSettings settings = new SaferpayPaymentSettings()
            {
                APIUrl = "https://test.saferpay.com/api/",
                APIUsername = "API_254441_29622448",
                APIPassword = "JsonApiPwd1_tF22qCLW",
                APISpecVersion = "1.19",
                CustomerId = "254441",
                TerminalId = "17727593",
                CurrencyCode = "eur"
            };

            await _settingService.SaveSettingAsync(settings);
            await InstallLocaleResources();
            await base.InstallAsync();
        }

        public async override Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<SaferpayPaymentSettings>();
            await DeleteLocaleResources();
            await base.UninstallAsync();
        }
        protected virtual async Task InstallLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/Payments.Saferpay/Localization");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, "*.xml"))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }
        }
        protected virtual async Task DeleteLocaleResources()
        {
            var file = Path.Combine(_fileProvider.MapPath("~/Plugins/Payments.Saferpay/Localization"), "ResourceString.xml");
            var languageResourceNames = from name in XDocument.Load(file).Document.Descendants("LocaleResource")
                                        select name.Attribute("Name").Value;

            foreach (var item in languageResourceNames)
            {
                await _localizationService.DeleteLocaleResourceAsync(item);
            }

        }
        public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return await Task.FromResult<IList<string>>(new List<string>());
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
                string fullName = (await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.SelectedShippingFullname))?.Trim();
                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    firstName = fullName.Split(' ')[0];
                    lastName = fullName.Split(' ').Length > 1 ? fullName.Split(' ')[1] : NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
                }
                var countryCode = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.CountryOrderOfProcessing) ?? await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.CountryOfProcessing);

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

        public Type GetPublicViewComponent()
        {
            return typeof(PaymentSaferpayViewComponent);
        }

        #endregion
    }
}
