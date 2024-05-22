using ISDK.Filuet.ExternalSSOAuthPlugin;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using ISDK.Filuet.Theme.FiluetHerbalife.Models.Common;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Models.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public class FiluetHerbalifeCommonModelFactory : CommonModelFactory
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISettingService _settingService;
        private readonly IRepository<CustomerGenericAttribute> _customerGenericAttributeRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _storeContext;

        public FiluetHerbalifeCommonModelFactory(IRepository<CustomerGenericAttribute> customerGenericAttributeRepository, ISettingService settingService, BlogSettings blogSettings, CaptchaSettings captchaSettings, CatalogSettings catalogSettings, CommonSettings commonSettings, CustomerSettings customerSettings, DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings, ForumSettings forumSettings, ICurrencyService currencyService, ICustomerService customerService, IForumService forumService, IGenericAttributeService genericAttributeService, IHttpContextAccessor httpContextAccessor, ILanguageService languageService, ILocalizationService localizationService, INopFileProvider fileProvider, INopHtmlHelper nopHtmlHelper, IPermissionService permissionService, IPictureService pictureService, IShoppingCartService shoppingCartService, IStaticCacheManager staticCacheManager, IStoreContext storeContext, IThemeContext themeContext, IThemeProvider themeProvider, ITopicService topicService, IUrlRecordService urlRecordService, IWebHelper webHelper, IWorkContext workContext, LocalizationSettings localizationSettings, MediaSettings mediaSettings, NewsSettings newsSettings, RobotsTxtSettings robotsTxtSettings, SitemapSettings sitemapSettings, SitemapXmlSettings sitemapXmlSettings, StoreInformationSettings storeInformationSettings, VendorSettings vendorSettings) : base(blogSettings, captchaSettings, catalogSettings, commonSettings, customerSettings, displayDefaultFooterItemSettings, forumSettings, currencyService, customerService, forumService, genericAttributeService, httpContextAccessor, languageService, localizationService, fileProvider, nopHtmlHelper, permissionService, pictureService, shoppingCartService, staticCacheManager, storeContext, themeContext, themeProvider, topicService, urlRecordService, webHelper, workContext, localizationSettings, mediaSettings, newsSettings, robotsTxtSettings, sitemapSettings, sitemapXmlSettings, storeInformationSettings, vendorSettings)
        {
            _workContext = workContext;
            _httpContextAccessor = httpContextAccessor;
            _settingService = settingService;
            _customerGenericAttributeRepository = customerGenericAttributeRepository;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
        }

        #endregion


        #region Methods

        public override async Task<SocialModel> PrepareSocialModelAsync()
        {
            #region FiluetCommonModelFactory

            var socialModel = await base.PrepareSocialModelAsync();
            var extendedSocialModel = AutoMapperConfiguration.Mapper.Map<FiluetSocialModel>(
                socialModel);
            var currentCulture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            extendedSocialModel.FacebookLink =
                await _settingService.GetSettingByKeyAsync(string.Format(NopFiluetCommonDefaults.SocialFacebookLinkKeyTemplate, currentCulture), extendedSocialModel.FacebookLink);
            extendedSocialModel.YoutubeLink =
                await _settingService.GetSettingByKeyAsync(string.Format(NopFiluetCommonDefaults.SocialYoutubeLinkKeyTemplate, currentCulture), extendedSocialModel.YoutubeLink);
            extendedSocialModel.CustomProperties.Add("social.site.url",
                await _settingService.GetSettingByKeyAsync(string.Format(NopFiluetCommonDefaults.SiteLinkKeyTemplate, currentCulture), "http://www.herbalife.com/"));
            extendedSocialModel.InstagramLink = await _genericAttributeService.GetAttributeAsync<string>(_storeContext.GetCurrentStore(), FiluetSettingController.InstagramLinkAttribute);

            var prepareSocialModelAsync1 = extendedSocialModel;

            #endregion

            #region BltFiluetCommonModelFactory

            var socialModel1 = prepareSocialModelAsync1 as FiluetSocialModel;
            if (socialModel1 is null)
                return prepareSocialModelAsync1;
            socialModel1.FacebookLink = "https://www.facebook.com/HerbalifeGreeceCyprus/";
            socialModel1.YoutubeLink = "https://www.youtube.com/playlist?list=PL29529CF1FF4A369F";
            switch ((await _workContext.GetWorkingLanguageAsync()).LanguageCulture)
            {
                case "en-US":
                    socialModel1.InstagramLink = "https://www.instagram.com/herbalife.gr.cy/";
                    break;
                case "ru-RU":
                    socialModel1.InstagramLink = "https://instagram.com/herbalife_rsm";
                    break;
                case "lt-LT":
                    socialModel1.InstagramLink = "https://instagram.com/herbalife_lithuania";
                    break;
                case "lv-LV":
                    socialModel1.InstagramLink = "https://instagram.com/herbalife_latvia";
                    break;
                case "et-EE":
                    socialModel1.InstagramLink = "https://instagram.com/herbalife_estonia";
                    break;
                case "el-GR":
                    socialModel1.InstagramLink = "https://www.instagram.com/herbalife.gr.cy/";
                    break;
            }
            var prepareSocialModelAsync = socialModel1;

            #endregion

            #region FiluetHerbalifeCommonModelFactory

            var socialModel2 = prepareSocialModelAsync as FiluetSocialModel;

            if (socialModel2 is null)
                return prepareSocialModelAsync;

            var filuetHerbalifeModel = new FiluetHerbalifeSocialModel
            {
                FacebookLink = socialModel2.FacebookLink,
                TwitterLink = socialModel2.TwitterLink,
                YoutubeLink = socialModel2.YoutubeLink,

                InstagramLink = socialModel2.InstagramLink,

                FiluetLinkedinLink = "https://www.linkedin.com/company/filuet-group/",
                FiluetInstagramLink = "https://www.instagram.com/herbalife.gr.cy/",
                FiluetFacebookLink = "https://www.facebook.com/HerbalifeGreeceCyprus/",
            };

            return filuetHerbalifeModel;

            #endregion
        }

        public override async Task<HeaderLinksModel> PrepareHeaderLinksModelAsync()
        {
            var model = await base.PrepareHeaderLinksModelAsync();

            var customer = await _workContext.GetCurrentCustomerAsync();
            var customerGenericAttribute = _customerGenericAttributeRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);

            var host = _httpContextAccessor.HttpContext.Request.Host.Host;
            var ssoAuthPluginSettings = await _settingService.LoadSettingAsync<SSOAuthPluginSettings>();
            var https = _httpContextAccessor.HttpContext.Request.IsHttps ? "https://" : "http://";

            //https://accounts.myherbalife.com/Logout/Callback?returnUrl=https://mydelivery.shop/logout

            var logoutLink = $"{ssoAuthPluginSettings.AuthorizationEndpoint}/Logout/Callback?returnUrl={https}{host}/logout";

            FiluetHeaderLinksModel filuetModel = new()
            {
                IsAuthenticated = model.IsAuthenticated,
                CustomerName = model.CustomerName,

                ShoppingCartEnabled = model.ShoppingCartEnabled,
                ShoppingCartItems = model.ShoppingCartItems,

                WishlistEnabled = model.WishlistEnabled,
                WishlistItems = model.WishlistItems,

                AllowPrivateMessages = model.AllowPrivateMessages,
                UnreadPrivateMessages = model.UnreadPrivateMessages,
                AlertMessage = model.AlertMessage,
                RegistrationType = model.RegistrationType,

                TV = Convert.ToDecimal(string.Format("{0:0.00}", customerGenericAttribute?.TV ?? 0)),
                LogoutLink = logoutLink
            };

            return filuetModel;
        }

        #endregion
    }
}