using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure.Mapper;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Factories;
using System;
using System.Linq;
using System.Threading.Tasks;
using AdminFactories = Nop.Web.Areas.Admin.Factories;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    class FiluetAdminCustomerModelFactory : AdminFactories.CustomerModelFactory
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        #endregion

        #region Ctor

        public FiluetAdminCustomerModelFactory(
            AddressSettings addressSettings,
            CustomerSettings customerSettings, 
            DateTimeSettings dateTimeSettings,
            GdprSettings gdprSettings,
            ForumSettings forumSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressAttributeFormatter addressAttributeFormatter,
            AdminFactories.IAddressModelFactory addressModelFactory, 
            IAffiliateService affiliateService, 
            IAuthenticationPluginManager authenticationPluginManager, 
            IBackInStockSubscriptionService backInStockSubscriptionService, 
            AdminFactories.IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICustomerActivityService customerActivityService, 
            ICustomerAttributeParser customerAttributeParser, 
            ICustomerAttributeService customerAttributeService, 
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper, 
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IRewardPointService rewardPointService, 
            IShoppingCartService shoppingCartService, 
            IStateProvinceService stateProvinceService, 
            IStoreContext storeContext,
            IStoreService storeService, 
            ITaxService taxService,
            MediaSettings mediaSettings, 
            RewardPointsSettings rewardPointsSettings,
            TaxSettings taxSettings) 
            : base(addressSettings,
                  customerSettings,
                  dateTimeSettings, 
                  gdprSettings, 
                  forumSettings,
                  aclSupportedModelFactory, 
                  addressAttributeFormatter,
                  addressModelFactory, 
                  affiliateService,
                  authenticationPluginManager,
                  backInStockSubscriptionService,
                  baseAdminModelFactory,
                  countryService, 
                  customerActivityService, 
                  customerAttributeParser,
                  customerAttributeService,
                  customerService, 
                  dateTimeHelper,
                  externalAuthenticationService,
                  gdprService, 
                  genericAttributeService,
                  geoLookupService,
                  localizationService, 
                  newsLetterSubscriptionService,
                  orderService, 
                  pictureService, 
                  priceFormatter,
                  productAttributeFormatter,
                  productService,
                  rewardPointService,
                  shoppingCartService,
                  stateProvinceService,
                  storeContext,
                  storeService,
                  taxService, 
                  mediaSettings,
                  rewardPointsSettings,
                  taxSettings)
        {
            _customerSettings = customerSettings;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _externalAuthenticationService = externalAuthenticationService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        #endregion

        #region Methods

        public override async Task<CustomerSearchModel> PrepareCustomerSearchModelAsync(CustomerSearchModel searchModel)
        {
            var filuetCustomerSearchModel = AutoMapperConfiguration.Mapper.Map<FiluetCustomerSearchModel>(await base.PrepareCustomerSearchModelAsync(searchModel));

            return filuetCustomerSearchModel;
        }

        public override async Task<CustomerListModel> PrepareCustomerListModelAsync(CustomerSearchModel searchModel)
        {
            var model = await base.PrepareCustomerListModelAsync(searchModel);
            var filuetCustomerSearchModel = searchModel as FiluetCustomerSearchModel;
          
            int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);
            var customers = _customerService.GetAllCustomers(
                searchExternalIdentifier: filuetCustomerSearchModel?.SearchExternalIdentifier,
                loadOnlyWithShoppingCart: false,
                customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                company: searchModel.SearchCompany,
                phone: searchModel.SearchPhone,
                zipPostalCode: searchModel.SearchZipPostalCode,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            model = new CustomerListModel
            {
                Data = await customers.SelectAwait(async customer =>
                {

                    //fill in model values from the entity
                    var filuetCustomerModel = new FiluetCustomerModel
                    {
                        Id = customer.Id,
                        Email = (await _customerService.IsRegisteredAsync(customer))
                            ? customer.Email
                            :await _localizationService.GetResourceAsync("Admin.Customers.Guest"),
                        Username = customer.Username,
                        FullName =await _customerService.GetCustomerFullNameAsync(customer),
                        Company =await _genericAttributeService.GetAttributeAsync<string>(customer,
                            customer.Company),
                        Phone =await _genericAttributeService.GetAttributeAsync<string>(customer,
                            customer.Phone),
                        ZipPostalCode =
                            await _genericAttributeService.GetAttributeAsync<string>(customer,
                                customer.ZipPostalCode),
                        Active = customer.Active
                    };

                    //convert dates to the user time
                    filuetCustomerModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc,
                        TimeZoneInfo.Utc, TimeZoneInfo.Local);
                    filuetCustomerModel.LastActivityDate =
                        _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, TimeZoneInfo.Utc,
                            TimeZoneInfo.Local);



                    //fill in additional values (not existing in the entity)
                    filuetCustomerModel.CustomerRoleNames = string.Join(", ",
                        (await _customerService.GetCustomerRolesAsync(customer)).Select(role => role.Name));
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        var avatarPictureId =
                            await _genericAttributeService.GetAttributeAsync<int>(customer,
                                NopCustomerDefaults.AvatarPictureIdAttribute);
                        filuetCustomerModel.AvatarUrl =await _pictureService.GetPictureUrlAsync(avatarPictureId,
                            _mediaSettings.AvatarPictureSize,
                            _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

                    filuetCustomerModel.ExternalIdentifier =
                        (await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
                        .FirstOrDefault()?.ExternalIdentifier;

                    return filuetCustomerModel;
                }).ToArrayAsync(),
                RecordsTotal = customers.TotalCount,
                RecordsFiltered = customers.TotalCount,
                Draw = searchModel?.Draw
            };

            return model;
        }

        #endregion
    }
}
