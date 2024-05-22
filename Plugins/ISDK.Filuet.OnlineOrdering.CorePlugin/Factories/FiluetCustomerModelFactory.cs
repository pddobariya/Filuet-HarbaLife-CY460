using ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    /// <summary>
    /// ExtendedCustomerModelFactory
    /// </summary>
    public class FiluetCustomerModelFactory : CustomerModelFactory, IFiluetCustomerModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDistributorService _distributorService;
        private readonly ILogger _logger;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FiluetCustomerModelFactory(
            AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings, 
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressModelFactory addressModelFactory, 
            IAuthenticationPluginManager authenticationPluginManager,
            ICountryService countryService, 
            ICustomerAttributeParser customerAttributeParser, 
            ICustomerAttributeService customerAttributeService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService, 
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService, 
            IOrderService orderService,
            IPermissionService permissionService, 
            IPictureService pictureService, 
            IProductService productService, 
            IReturnRequestService returnRequestService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService, 
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            SecuritySettings securitySettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            IDistributorService distributorService,
            ISettingService settingService,
            ILogger logger) 
            : base(addressSettings, 
                  captchaSettings, 
                  catalogSettings,
                  commonSettings, 
                  customerSettings,
                  dateTimeSettings,
                  externalAuthenticationSettings,
                  forumSettings,
                  gdprSettings,
                  addressModelFactory,
                  authenticationPluginManager,
                  countryService, 
                  customerAttributeParser, 
                  customerAttributeService,
                  customerService, 
                  dateTimeHelper,
                  externalAuthenticationService,
                  gdprService, 
                  genericAttributeService, 
                  localizationService, 
                  multiFactorAuthenticationPluginManager,
                  newsLetterSubscriptionService, 
                  orderService,
                  permissionService,
                  pictureService,
                  productService, 
                  returnRequestService, 
                  stateProvinceService,
                  storeContext, 
                  storeMappingService, 
                  urlRecordService,
                  workContext, 
                  mediaSettings,
                  orderSettings,
                  rewardPointsSettings,
                  securitySettings, 
                  taxSettings,
                  vendorSettings)
        {
            _localizationService = localizationService;
            _workContext = workContext;
            _distributorService = distributorService;
            _logger = logger;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>Customer navigation model</returns>
        public override async Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = await _localizationService.GetResourceAsync("Account.CustomerInfo"),
                Tab = (int)CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = await _localizationService.GetResourceAsync("Account.CustomerOrders"),
                Tab = (int)CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            model.SelectedTab = (int)(CustomerNavigationEnum)selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the extended customer info model
        /// </summary>
        /// <param name="model">Extended customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>Customer info model</returns>
        public async Task<FiluetCustomerInfoModel> PrepareFiluetCustomerInfoModelAsync(FiluetCustomerInfoModel model, Customer customer,
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            CustomerInfoModel customerInfoModel =await  base.PrepareCustomerInfoModelAsync(model, customer, excludeProperties, overrideCustomCustomerAttributesXml);
            FiluetCustomerInfoModel extendedCustomerInfoModel = PluginMapper.Mapper.Map<FiluetCustomerInfoModel>(customerInfoModel);
            var distributorFullProfile = await _distributorService.GetDistributorFullProfileAsync(customer);

            var DistributorDetailedProfileResponse = await _distributorService.GetDistributorDetailedProfileAsync(customer);
            if (DistributorDetailedProfileResponse != null)
            {
                DistributorDetailedProfileResponse.VolumeLimits = distributorFullProfile?.DistributorDetailedProfileResponse?.VolumeLimits;
            }
            else
            {
                await _logger.ErrorAsync($"[distributorFullProfile is null]");
            }

            extendedCustomerInfoModel.DistributorId =await customer.GetDistributorIdAsync();
            extendedCustomerInfoModel.Country = distributorFullProfile?.DistributorProfileResponse?.CountryOfResidence;
            extendedCustomerInfoModel.Ppv = distributorFullProfile?.DistributorVolumeResponse?.PpvValue != null ? Convert.ToDecimal(string.Format("{0:0.00}", distributorFullProfile.DistributorVolumeResponse.PpvValue)) : 0.00M;
            extendedCustomerInfoModel.Pv = distributorFullProfile?.DistributorVolumeResponse?.PvValue != null ? Convert.ToDecimal(string.Format("{0:0.00}", distributorFullProfile.DistributorVolumeResponse.PvValue)) : 0.00M;
            extendedCustomerInfoModel.Tv = distributorFullProfile?.DistributorVolumeResponse?.TvValue != null ? Convert.ToDecimal(string.Format("{0:0.00}", distributorFullProfile.DistributorVolumeResponse.TvValue)) : 0.00M;
            extendedCustomerInfoModel.StreetAddress = distributorFullProfile?.DistributorDetailedProfileResponse?.Addresses?.MailingAddress?.FullAddress;
            extendedCustomerInfoModel.Phone = distributorFullProfile?.DistributorDetailedProfileResponse?.Phones?.FirstOrDefault();
            
            var attribute = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.StreetAddressAttribute);
            if (attribute != null)
            {
                extendedCustomerInfoModel.AdditionalStreetAddresses.AddRange(JsonConvert.DeserializeObject<List<string>>(attribute));
            }

            attribute = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.PhoneAttribute);
         
             if (attribute != null)
            {
                extendedCustomerInfoModel.AdditionalPhones.AddRange(JsonConvert.DeserializeObject<List<string>>(attribute));
            }
            
            return extendedCustomerInfoModel;
        }

        #endregion
    }
}