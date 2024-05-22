using ISDK.Filuet.OnlineOrdering.CorePlugin.Factories;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Core.Http;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin
{
    public class FiluetCustomerController : CustomerController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IFiluetCustomerService _filuetCustomerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFiluetCustomerModelFactory _filuetcustomerModelFactory;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FiluetCustomerController(
            AddressSettings addressSettings, 
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings, 
            DateTimeSettings dateTimeSettings, 
            ForumSettings forumSettings,
            GdprSettings gdprSettings, 
            HtmlEncoder htmlEncoder,
            IAddressAttributeParser addressAttributeParser,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService, 
            IAuthenticationService authenticationService, 
            ICountryService countryService, 
            ICurrencyService currencyService, 
            ICustomerActivityService customerActivityService, 
            ICustomerAttributeParser customerAttributeParser, 
            ICustomerAttributeService customerAttributeService, 
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IDownloadService downloadService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            ILogger logger, 
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService, 
            IOrderService orderService, 
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IStateProvinceService stateProvinceService, 
            IStoreContext storeContext, 
            ITaxService taxService, 
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings, 
            MediaSettings mediaSettings, 
            MultiFactorAuthenticationSettings multiFactorAuthenticationSettings, 
            StoreInformationSettings storeInformationSettings, 
            TaxSettings taxSettings, 
            IFiluetCustomerService filuetCustomerService, 
            IHttpContextAccessor httpContextAccessor, 
            IFiluetCustomerModelFactory filuetcustomerModelFactory)
            : base(addressSettings, 
                  captchaSettings,
                  customerSettings,
                  dateTimeSettings,
                  forumSettings, 
                  gdprSettings,
                  htmlEncoder,
                  addressAttributeParser,
                  addressModelFactory,
                  addressService, 
                  authenticationService,
                  countryService,
                  currencyService, 
                  customerActivityService,
                  customerAttributeParser,
                  customerAttributeService,
                  customerModelFactory, 
                  customerRegistrationService, 
                  customerService,
                  downloadService,
                  eventPublisher, 
                  exportManager,
                  externalAuthenticationService,
                  gdprService, 
                  genericAttributeService,
                  giftCardService, 
                  localizationService, 
                  logger,
                  multiFactorAuthenticationPluginManager,
                  newsLetterSubscriptionService,
                  notificationService, 
                  orderService, 
                  permissionService,
                  pictureService,
                  priceFormatter,
                  productService,
                  stateProvinceService, 
                  storeContext, taxService,
                  workContext, 
                  workflowMessageService,
                  localizationSettings,
                  mediaSettings,
                  multiFactorAuthenticationSettings,
                  storeInformationSettings, 
                  taxSettings)
        {
            _workContext = workContext;
            _filuetCustomerService = filuetCustomerService;
            _httpContextAccessor = httpContextAccessor;
            _filuetcustomerModelFactory = filuetcustomerModelFactory;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual ActionResult UserLogin()
        {
            var model = new FiluetLoginModel();

            if (TempData["UnCompleted"] as bool? == true)
            {
                model.IsUnCompleted = true;
            }

            if (TempData["CantBuy"] as bool? == true)
            {
                model.CantBuy = true;
            }

            return View(model);
        }

        public override async Task<IActionResult> Info()
        {
            if (!(await _filuetCustomerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync())))
            {
                return this.Challenge();
            }

            var model = new FiluetCustomerInfoModel();
            model = await _filuetcustomerModelFactory.PrepareFiluetCustomerInfoModelAsync(model, await _workContext.GetCurrentCustomerAsync(), false);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStreetAddress(string streetAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(streetAddress))
                    throw new Exception($"{nameof(streetAddress)} can't be null or empty");
                var attribute = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.StreetAddressAttribute);
                if (attribute != null)
                {
                    var additionalStreetAddresses = JsonConvert.DeserializeObject<List<string>>(attribute);
                    if (!additionalStreetAddresses.Remove(streetAddress))
                        throw new Exception("There isn't that element");
                    await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                        CoreGenericAttributes.StreetAddressAttribute,
                        additionalStreetAddresses.Any()
                            ? JsonConvert.SerializeObject(additionalStreetAddresses)
                            : null);
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult AddStreetAddress(string streetAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(streetAddress))
                    throw new Exception($"{nameof(streetAddress)} can't be null or empty");
                _filuetCustomerService.AddStreetAddress(streetAddress);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeletePhone(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    throw new Exception($"{nameof(phone)} can't be null or empty");
                var attribute = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.PhoneAttribute);
                if (attribute != null)
                {
                    var additionalPhones = JsonConvert.DeserializeObject<List<string>>(attribute);
                    if (!additionalPhones.Remove(phone))
                        throw new Exception("There isn't that element");
                    await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.PhoneAttribute, additionalPhones.Any() ? JsonConvert.SerializeObject(additionalPhones) : null);

                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult AddPhone(string phone)
        {
            try
            {
                if (string.IsNullOrEmpty(phone))
                    throw new Exception($"{nameof(phone)} can't be null or empty");
                _filuetCustomerService.AddPhone(phone);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }

            return Ok();
        }

        public override async Task<IActionResult> Logout()
        {
            var cookieName = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.CustomerCookie}";
            _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);
            return await base.Logout();
        }

        [JetBrains.Annotations.AspMvcSuppressViewError]
        public IActionResult OrderHistory()
        {
            return View();
        }

        #endregion
    }
}