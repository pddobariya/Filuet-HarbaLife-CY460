using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure;
using ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminProductFactory = ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Factories;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public class FiluetProductModelFactory : ProductModelFactory
    {
        #region Fields

        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public FiluetProductModelFactory(
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IReviewTypeService reviewTypeService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreService storeService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IVideoService videoService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            SeoSettings seoSettings,
            ShippingSettings shippingSettings,
            VendorSettings vendorSettings,
            ISettingService settingService) : base(captchaSettings, catalogSettings, customerSettings, categoryService, currencyService, customerService, dateRangeService, dateTimeHelper, downloadService, genericAttributeService, localizationService, manufacturerService, permissionService, pictureService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productService, productTagService, productTemplateService, reviewTypeService, shoppingCartService, specificationAttributeService, staticCacheManager, storeContext, storeService, shoppingCartModelFactory, taxService, urlRecordService, vendorService, videoService, webHelper, workContext, mediaSettings, orderSettings, seoSettings, shippingSettings, vendorSettings)
        {
            _priceFormatter = priceFormatter;
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _productService = productService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public override async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true, int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
            {
                throw new ArgumentNullException("products");
            }

            products = products.OrderBy(p => p.DisplayOrder);

            var models = new List<ProductOverviewModel>();
            var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ShortDescription = await _localizationService.GetLocalizedAsync(product, x => x.ShortDescription),
                    FullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription),
                    SeName = await _urlRecordService.GetSeNameAsync(product),
                    Sku = product.Sku,
                    ProductType = product.ProductType,
                    MarkAsNew = product.MarkAsNew &&
                        (!product.MarkAsNewStartDateTimeUtc.HasValue || product.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!product.MarkAsNewEndDateTimeUtc.HasValue || product.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                // price
                if (preparePriceModel)
                {
                    model.ProductPrice = await PrepareProductOverviewPriceModelAsync(product, forceRedirectionAfterAddingToCart);
                }

                // picture
                if (preparePictureModel)
                {
                    model.PictureModels = await PrepareProductOverviewPicturesModelAsync(product, productThumbPictureSize);
                }

                // specs
                if (prepareSpecificationAttributes)
                {
                    model.ProductSpecificationModel = await PrepareProductSpecificationModelAsync(product);
                }

                // reviews
                model.ReviewOverviewModel = await PrepareProductReviewOverviewModelAsync(product);

                // Custom properties
                model.CustomProperties.Add(NopFiluetCommonDefaults.VolumePoints, (await _genericAttributeService.GetAttributeAsync<double>(product,ProductAttributeNames.VolumePoints)).ToString());
                model.CustomProperties.Add(NopFiluetCommonDefaults.ShowProductReview, settings.ProductShowReview.ToString());
                models.Add(model);
            }

            return models;
        }


        protected override async Task<ProductDetailsModel.ProductPriceModel> PrepareProductPriceModelAsync(Product product)
        {
            var productPriceModel = await base.PrepareProductPriceModelAsync(product);
            productPriceModel.CustomProperties.Add(NopFiluetCommonDefaults.VolumePoints, (await _genericAttributeService.GetAttributeAsync<double>(product,ProductAttributeNames.VolumePoints)).ToString());

            return productPriceModel;
        }


        public override async Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            var productDetailModel = await base.PrepareProductDetailsModelAsync(product, updatecartitem, isAssociatedProduct);
            var extendedModel = PluginMapper.Mapper.Map<FiluetProductDetailsModel>(productDetailModel);
            extendedModel.BasicRetailPrice = await _priceFormatter.FormatPriceAsync(await _genericAttributeService.GetAttributeAsync<decimal>(product,AdminProductFactory.FiluetProductModelFactory.BasicRetailPriceAttribute));
            extendedModel.EarnBasePrice = await _priceFormatter.FormatPriceAsync(await _genericAttributeService.GetAttributeAsync<decimal>(product,AdminProductFactory.FiluetProductModelFactory.EarnBasePriceAttribute));
            extendedModel.ProductForOrderCategory =await _genericAttributeService.GetAttributeAsync<string>(product,AdminProductFactory.FiluetProductModelFactory.ProductForOrderCategoryAttribute);
            
            try
            {
                var parentProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
                if (parentProduct != null)
                {
                    var associatedProducts = (await _productService.GetAssociatedProductsAsync(parentProduct.Id, (await _storeContext.GetCurrentStoreAsync()).Id))
                    .Where(p => p.Id != product.Id);

                    foreach (var associatedProduct in associatedProducts)
                        extendedModel.AssociatedProducts.Add(await base.PrepareProductDetailsModelAsync(associatedProduct, null, false));
                }

                extendedModel.ProductDescription = JsonConvert.DeserializeObject<ProductDescription>(extendedModel.FullDescription);
            }
            catch { }

            return extendedModel;
        }

        #endregion
    }
}
