using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.ExportImport.Help;
using Nop.Services.Forums;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions.ExportImport
{
    class FiluetExportManager : ExportManager
    {
        #region Fields 

        private readonly CatalogSettings _catalogSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ICountryService _countryService;
        private readonly ICustomerAttributeFormatter _customerAttributeFormatter;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IAddressService _addressService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public FiluetExportManager(
            AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            ICustomerActivityService customerActivityService,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings, 
            ForumSettings forumSettings,
            IAddressService addressService,
            ICategoryService categoryService,
            ICountryService countryService,
            ICurrencyService currencyService, 
            ICustomerAttributeFormatter customerAttributeFormatter, 
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IForumService forumService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IManufacturerService manufacturerService,
            IMeasureService measureService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IShipmentService shipmentService, 
            ISpecificationAttributeService specificationAttributeService,
            IStateProvinceService stateProvinceService,
            IStoreMappingService storeMappingService, 
            IStoreService storeService,
            ITaxCategoryService taxCategoryService,
            IUrlRecordService urlRecordService, 
            IVendorService vendorService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ProductEditorSettings productEditorSettings
            ) : base(addressSettings,
                catalogSettings,
                customerActivityService,
                customerSettings,
                dateTimeSettings,
                forumSettings, 
                addressService,
                categoryService,
                countryService,
                currencyService,
                customerAttributeFormatter,
                customerService,
                dateRangeService, 
                dateTimeHelper,
                discountService,
                forumService,
                gdprService,
                genericAttributeService,
                languageService, 
                localizationService,
                localizedEntityService,
                manufacturerService,
                measureService, 
                newsLetterSubscriptionService,
                orderService,
                pictureService,
                priceFormatter,
                productAttributeService,
                productService, 
                productTagService,
                productTemplateService,
                shipmentService,
                specificationAttributeService,
                stateProvinceService, 
                storeMappingService,
                storeService,
                taxCategoryService,
                urlRecordService,
                vendorService, 
                workContext,
                orderSettings,
                productEditorSettings)
        {
            _catalogSettings = catalogSettings;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _countryService = countryService;
            _customerAttributeFormatter = customerAttributeFormatter;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _stateProvinceService = stateProvinceService;
            _vendorService = vendorService;
            _workContext = workContext;
            _addressService = addressService;
            _languageService = languageService; 

        }

        #endregion

        #region Methods

        public override async Task<byte[]> ExportCustomersToXlsxAsync(IList<Customer> customers)
        {

            async Task<object> getPasswordFormat(Customer customer)
            {
                var password = await _customerService.GetCurrentPasswordAsync(customer.Id);

                var passwordFormatId = password?.PasswordFormatId ?? 0;

                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return passwordFormatId;

                return CommonHelper.ConvertEnum(((PasswordFormat)passwordFormatId).ToString());
            }

            var vendors = await _vendorService.GetVendorsByCustomerIdsAsync(customers.Select(c => c.Id).ToArray());

            object getVendor(Customer customer)
            {
                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return customer.VendorId;

                return vendors.FirstOrDefault(v => v.Id == customer.VendorId)?.Name ?? string.Empty;
            }

            async Task<object> getCountry(Customer customer)
            {
                var countryId = await _genericAttributeService.GetAttributeAsync<int>(customer, /*NopCustomerDefaults.CountryIdAttribute*/ "CountryId");

                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return countryId;

                var country = await _countryService.GetCountryByIdAsync(countryId);

                return country?.Name ?? string.Empty;
            }

            async Task<object> getStateProvince(Customer customer)
            {
                var stateProvinceId = await _genericAttributeService.GetAttributeAsync<int>(customer, /*NopCustomerDefaults.StateProvinceIdAttribute*/ "StateProvinceId");

                if (!_catalogSettings.ExportImportRelatedEntitiesByName)
                    return stateProvinceId;

                var stateProvince = await _stateProvinceService.GetStateProvinceByIdAsync(stateProvinceId);

                return stateProvince?.Name ?? string.Empty;
            }
            //old code

            //async Task<object> getVatNumberStatus(Customer customer)
            //{
            //    var vatNumberStatusId = await _genericAttributeService.GetAttributeAsync<int>(customer, /*NopCustomerDefaults.VatNumberStatusIdAttribute*/ "VatNumberStatusId");

            //    if (!_catalogSettings.ExportImportRelatedEntitiesByName)
            //        return vatNumberStatusId;

            //    return CommonHelper.ConvertEnum(((VatNumberStatus)vatNumberStatusId).ToString());
            //}

            //property manager 
            var manager = new PropertyManager<Customer, Language>(new[]
            {
                new PropertyByName<Customer,Language>("CustomerId", (p,l) => p.Id),
                new PropertyByName<Customer,Language>("DistributorId", async (p,l) => await p.GetDistributorIdAsync()),
                new PropertyByName<Customer,Language>("CustomerGuid", (p, l) => p.CustomerGuid),
                new PropertyByName<Customer,Language>("Email", (p,l) => p.Email),
                new PropertyByName<Customer,Language>("Username", (p, l) => p.Username),
                new PropertyByName<Customer,Language>("Password", async (p,l) => (await _customerService.GetCurrentPasswordAsync(p.Id))?.Password),
                new PropertyByName<Customer,Language>("PasswordFormat",  (p,l) => getPasswordFormat(p)),
                new PropertyByName<Customer,Language>("PasswordSalt", async (p,l) => (await _customerService.GetCurrentPasswordAsync(p.Id))?.PasswordSalt),
                new PropertyByName<Customer,Language>("IsTaxExempt", (p, l) => p.IsTaxExempt),
                new PropertyByName<Customer,Language>("AffiliateId", (p, l) => p.AffiliateId),
                new PropertyByName<Customer,Language>("Vendor", (p, l) => getVendor(p)),
                new PropertyByName<Customer,Language>("Active", (p, l) => p.Active),
                new PropertyByName<Customer,Language>("CustomerRoles", async (p,l)=>string.Join(", ",
                    (await _customerService.GetCustomerRolesAsync(p)).Select(role => _catalogSettings.ExportImportRelatedEntitiesByName ? role.Name : role.Id.ToString()))),
                new PropertyByName<Customer,Language>("IsGuest", async (p,l) => await _customerService.IsGuestAsync(p)),
                new PropertyByName<Customer,Language>("IsRegistered", async (p,l) => await _customerService.IsRegisteredAsync(p)),
                new PropertyByName<Customer,Language>("IsAdministrator", async (p,l) => await _customerService.IsAdminAsync(p)),
                new PropertyByName<Customer,Language>("IsForumModerator", async (p,l) => await _customerService.IsForumModeratorAsync(p)),
                new PropertyByName<Customer,Language>("IsVendor", async (p,l) => await _customerService.IsVendorAsync(p)),
                new PropertyByName<Customer,Language>("CreatedOnUtc", (p,l) => p.CreatedOnUtc),
                //attributes
                new PropertyByName<Customer,Language>("LastName", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "FirstName"), !_customerSettings.LastNameEnabled),
                new PropertyByName<Customer,Language>("Gender", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "Gender"), !_customerSettings.GenderEnabled),
                new PropertyByName<Customer,Language>("Company", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "Company"), !_customerSettings.CompanyEnabled),
                new PropertyByName<Customer,Language>("StreetAddress", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "StreetAddress"), !_customerSettings.StreetAddressEnabled),
                new PropertyByName<Customer,Language>("StreetAddress2", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "StreetAddress2"), !_customerSettings.StreetAddress2Enabled),
                new PropertyByName<Customer,Language>("ZipPostalCode", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "ZipPostalCode"), !_customerSettings.ZipPostalCodeEnabled),
                new PropertyByName<Customer,Language>("City", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "City"), !_customerSettings.CityEnabled),
                new PropertyByName<Customer,Language>("County", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "County"), !_customerSettings.CountyEnabled),
                new PropertyByName<Customer,Language>("Country", async (p, l) => await getCountry(p), !_customerSettings.CountryEnabled),
                new PropertyByName<Customer,Language>("StateProvince", async (p, l) => await getStateProvince(p), !_customerSettings.StateProvinceEnabled),
                new PropertyByName<Customer,Language>("Phone", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "Phone"), !_customerSettings.PhoneEnabled),
                new PropertyByName<Customer,Language>("Fax", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "Fax"), !_customerSettings.FaxEnabled),
                new PropertyByName<Customer,Language>("VatNumber", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "VatNumber")),
                new PropertyByName<Customer,Language>("VatNumberStatus", (p, l) => p.VatNumberStatusId),
                new PropertyByName<Customer,Language>("TimeZone", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "TimeZoneId"), !_dateTimeSettings.AllowCustomersToSetTimeZone),
                new PropertyByName<Customer,Language>("AvatarPictureId", async (p,l) => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.AvatarPictureIdAttribute), !_customerSettings.AllowCustomersToUploadAvatars),
                new PropertyByName<Customer,Language>("ForumPostCount", async (p,l) => await _genericAttributeService.GetAttributeAsync<int>(p, NopCustomerDefaults.ForumPostCountAttribute)),
                new PropertyByName<Customer,Language>("Signature", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, NopCustomerDefaults.SignatureAttribute)),
                new PropertyByName<Customer,Language>("CustomCustomerAttributes",  async (p, l) => await GetCustomCustomerAttributesAsync(p)),
                new PropertyByName<Customer,Language>("FirstName", async (p,l) => await _genericAttributeService.GetAttributeAsync<string>(p, "FirstName"), !_customerSettings.FirstNameEnabled),
            }, _catalogSettings);

            return await manager.ExportToXlsxAsync(customers);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task<object> GetCustomCustomerAttributesAsync(Customer customer)
        {
            var selectedCustomerAttributes = await _genericAttributeService.GetAttributeAsync<string>(customer, "CustomCustomerAttributes");

            return await _customerAttributeFormatter.FormatAttributesAsync(selectedCustomerAttributes, ";");
        }

        public async override Task<byte[]> ExportOrdersToXlsxAsync(IList<Order> orders)
        {
            var languages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            //a vendor should have access only to part of order information
            var ignore = await _workContext.GetCurrentVendorAsync() != null;

            //lambda expressions for choosing correct order address
            async Task<Address> orderAddress(Order o) => await _addressService.GetAddressByIdAsync((o.PickupInStore ? o.PickupAddressId : o.ShippingAddressId) ?? 0);
            async Task<Address> orderBillingAddress(Order o) => await _addressService.GetAddressByIdAsync(o.BillingAddressId);

            //property array
            var properties = new[]
            {
                new PropertyByName<Order, Language>("OrderId", (o, l) => o.Id),
                new PropertyByName<Order, Language>("FusionOrderNumber", async (order,l) =>await order.GetFusionOrderNumberAsync()),
                new PropertyByName<Order, Language>("ShippingCountry", async (order,l) => (await _countryService.GetCountryByAddressAsync(await orderAddress(order)))?.Name ?? string.Empty),
                new PropertyByName<Order, Language>("ID Herbalife", async (order,l) => await(await _customerService.GetCustomerByIdAsync(order.CustomerId)).GetDistributorIdAsync()),
                new PropertyByName<Order, Language>("Discount", async (order,l) => (await _genericAttributeService.GetAttributeAsync<double>(order,OrderAttributeNames.DiscountPercent)).FormatPercent()),
                new PropertyByName<Order, Language>("BillingFirstName", async (order,l) => (await orderBillingAddress(order))?.FirstName ?? string.Empty),
                new PropertyByName<Order, Language>("BillingLastName", async (order,l) => (await orderBillingAddress(order))?.LastName ?? string.Empty),
                new PropertyByName<Order, Language>("VP заказа", async (order,l) => await(await _genericAttributeService.GetAttributeAsync < double >(order, OrderAttributeNames.VolumePoints)).FormatPriceAsync(true)),
                new PropertyByName<Order, Language>("OrderTotal", (order,l) => order.OrderTotal),
                new PropertyByName<Order, Language>("RefundedAmount", (order,l) => order.RefundedAmount),
                new PropertyByName<Order, Language>("BillingEmail", async (order,l) => (await orderBillingAddress(order))?.Email ?? string.Empty),
                new PropertyByName<Order, Language>("PaymentMethodSystemName", (order,l) => order.PaymentMethodSystemName),
                new PropertyByName<Order, Language>("OrderGuid", (order,l) => order.OrderGuid)
            };

            return await new PropertyManager<Order, Language>(properties, _catalogSettings, null, languages).ExportToXlsxAsync(orders);
        }

        #endregion
    }
}
