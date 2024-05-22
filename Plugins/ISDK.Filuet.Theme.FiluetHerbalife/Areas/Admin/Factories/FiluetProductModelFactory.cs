using ISDK.Filuet.OnlineOrdering.CorePlugin.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Product;
using ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Factories
{
    public class FiluetProductModelFactory : FiluetAdminProductModelFactory
    {
        #region Ctor
        public FiluetProductModelFactory(
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings, 
            IAclSupportedModelFactory aclSupportedModelFactory, 
            IAddressService addressService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService, 
            ICurrencyService currencyService,
            ICustomerService customerService, 
            IDateTimeHelper dateTimeHelper, 
            IDiscountService discountService, 
            IDiscountSupportedModelFactory discountSupportedModelFactory, 
            ILocalizationService localizationService, 
            ILocalizedModelFactory localizedModelFactory, 
            IManufacturerService manufacturerService, 
            IMeasureService measureService, 
            IOrderService orderService, 
            IPictureService pictureService,
            IProductAttributeFormatter productAttributeFormatter,
            IProductAttributeParser productAttributeParser, 
            IProductAttributeService productAttributeService, 
            IProductService productService,
            IProductTagService productTagService, 
            IProductTemplateService productTemplateService, 
            ISettingModelFactory settingModelFactory,
            ISettingService settingService, 
            IShipmentService shipmentService, 
            IShippingService shippingService, 
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreContext storeContext, 
            IStoreService storeService, 
            IUrlRecordService urlRecordService,
            IVideoService videoService, 
            IWorkContext workContext, 
            MeasureSettings measureSettings,
            NopHttpClient nopHttpClient,
            TaxSettings taxSettings, 
            VendorSettings vendorSettings,
            IGenericAttributeService genericAttributeService) : base(catalogSettings, currencySettings, aclSupportedModelFactory, addressService, baseAdminModelFactory, categoryService, currencyService, customerService, dateTimeHelper, discountService, discountSupportedModelFactory, localizationService, localizedModelFactory, manufacturerService, measureService, orderService, pictureService, productAttributeFormatter, productAttributeParser, productAttributeService, productService, productTagService, productTemplateService, settingModelFactory, settingService, shipmentService, shippingService, shoppingCartService, specificationAttributeService, storeMappingSupportedModelFactory, storeContext, storeService, urlRecordService, videoService, workContext, measureSettings, nopHttpClient, taxSettings, vendorSettings, genericAttributeService)
        {
        }

        #endregion

        #region Methods

        public override async Task<ProductModel> PrepareProductModelAsync(ProductModel model, Product product, bool excludeProperties = false)
        {
            model = await base.PrepareProductModelAsync(model, product, excludeProperties);
            
            var extendedModel = PluginMapper.Mapper.Map<FiluetProductModel>(model);
            
            if(extendedModel.FullDescription != null)
            {
                try
                {
                    extendedModel.ProductDescription = JsonConvert.DeserializeObject<ProductDescription>(extendedModel.FullDescription);
                }
                catch { }
            }

            foreach (var locale in extendedModel.Locales)
            {
                if(locale.FullDescription != null)
                {
                    try
                    {
                        locale.ProductDescription = JsonConvert.DeserializeObject<ProductDescription>(locale.FullDescription);
                    }
                    catch { }
                }
            }

            return extendedModel;
        }

        #endregion
    }
}
