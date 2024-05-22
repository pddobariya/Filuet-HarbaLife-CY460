using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;
using Nop.Web.Models.Sitemap;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    public class FiluetCommonController : CommonController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public FiluetCommonController(
            ICustomerService customerService,
            ISettingService settingService, 
            IPermissionService permissionService,
            CaptchaSettings captchaSettings,
            CommonSettings commonSettings,
            ICommonModelFactory commonModelFactory,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            IGenericAttributeService genericAttributeService,
            IHtmlFormatter htmlFormatter,
            ILanguageService languageService, 
            ILocalizationService localizationService,
            ISitemapModelFactory sitemapModelFactory, 
            IStoreContext storeContext,
            IThemeContext themeContext,
            IVendorService vendorService, 
            IWorkContext workContext, 
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            SitemapSettings sitemapSettings,
            SitemapXmlSettings sitemapXmlSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings) 
            : base(captchaSettings,
                  commonSettings,
                  commonModelFactory, 
                  currencyService, 
                  customerActivityService,
                  genericAttributeService,
                  htmlFormatter, 
                  languageService,
                  localizationService,
                  sitemapModelFactory,
                  storeContext,
                  themeContext,
                  vendorService, 
                  workContext, 
                  workflowMessageService,
                  localizationSettings,
                  sitemapSettings, 
                  sitemapXmlSettings,
                  storeInformationSettings, 
                  vendorSettings)
        {
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _workContext = workContext;
            _permissionService = permissionService;
            _settingService = settingService;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        [HttpsRequirement]
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public override async Task<IActionResult> ContactUs()
        {
            ControllerContext.RouteData.Values["controller"] = "Common";
            return await base.ContactUs();
        }

        [HttpPost, ActionName("ContactUs")]
        [ValidateCaptcha]
        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public override async Task<IActionResult> ContactUsSend(ContactUsModel model, bool captchaValid)
        {
            ControllerContext.RouteData.Values["controller"] = "Common";
            return await base.ContactUsSend(model, captchaValid);
        }

        [CheckAccessPublicStore(true)]
        public override async Task<IActionResult> Sitemap(SitemapPageModel pageModel)
        {
            ControllerContext.RouteData.Values["controller"] = "Common";
            return await base.Sitemap(pageModel);
        }

        [HttpPost]
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> LoadFirst(string mainUrl)
        {
            var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.PublicStoreAllowNavigation) ||await _customerService.IsAdminAsync(await _workContext.GetCurrentCustomerAsync()))
                return Content(String.Empty);
            if (!settings.MainConditionMessageBoxEnabled || await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.MainConditionsAcceptedAttribute) || mainUrl == Url.Action("AuthError", "SSOAuth"))
                return await GetApfMessage(mainUrl);

            //oldCode to new
            //if (!settings.MainConditionMessageBoxEnabled || await (await _workContext.GetCurrentCustomerAsync()).GetAttributeAsync<bool>(CoreGenericAttributes.MainConditionsAcceptedAttribute) || mainUrl == Url.Action("AuthError", "SSOAuth"))
            //    return await GetApfMessage(mainUrl);
            return Json(new
            {
                html = await RenderPartialViewToStringAsync("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/MainConditions.cshtml", null),
                mainUrl = mainUrl
            });
        }

        [HttpPost]
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> CloseMainCondition(string mainUrl)
        {
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.MainConditionsAcceptedAttribute, true);
            return await GetApfMessage(mainUrl);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> CloseApfMessage(string mainUrl)
        {
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.ApfMessageAcceptedAttribute, true);
            return Content(Url.RouteUrl("ShoppingCart"));
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> DeclineApfMessage(string mainUrl)
        {
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.ApfMessageDeclinedAttribute, true);
            return Ok();
        }

        private async Task<IActionResult> GetApfMessage(string mainUrl)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var filuetCorePluginSettings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>();
            var distributorService = EngineContext.Current.Resolve<IDistributorService>();
            var profile = await distributorService.GetDistributorDetailedProfileAsync(currentCustomer);
            if (!filuetCorePluginSettings.IsDeptorEnabled || !(profile?.ApfDueDate < DateTimeOffset.UtcNow.AddHours(filuetCorePluginSettings.HoursShift) || profile?.ApfDueDate != null && profile.ApfDueDate.Date - DateTime.UtcNow.AddHours(filuetCorePluginSettings.HoursShift) < new TimeSpan(60, 0, 0, 0)) || mainUrl == Url.RouteUrl("ShoppingCart") ||await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer,CoreGenericAttributes.ApfMessageAcceptedAttribute) ||await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer,CoreGenericAttributes.ApfMessageDeclinedAttribute))
                return Content(string.Empty);
            return Json(new
            {
                html = await RenderPartialViewToStringAsync("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/ApfMessageBox.cshtml", profile)
            });
        }

        #endregion
    }
}