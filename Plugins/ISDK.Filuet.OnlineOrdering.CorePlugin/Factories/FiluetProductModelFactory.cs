using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Catalog;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
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
using Nop.Core.Infrastructure.Mapper;
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
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    /// <summary>
    /// ExtendedProductModelFactory
    /// </summary>
    public class FiluetProductModelFactory : ProductModelFactory, IProductModelFactory
    {
        #region Fields
        private readonly ILocalizationService _localizationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IUrlRecordService _urlRecordService; 
        private readonly IGenericAttributeService _genericAttributeService;

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
            ISettingService settingService)
            : base(captchaSettings, 
                  catalogSettings, 
                  customerSettings,
                  categoryService,
                  currencyService, 
                  customerService, 
                  dateRangeService, 
                  dateTimeHelper,
                  downloadService,
                  genericAttributeService,
                  localizationService, 
                  manufacturerService,
                  permissionService,
                  pictureService,
                  priceCalculationService,
                  priceFormatter,
                  productAttributeParser,
                  productAttributeService,
                  productService,
                  productTagService,
                  productTemplateService, 
                  reviewTypeService,
                  shoppingCartService,
                  specificationAttributeService,
                  staticCacheManager, 
                  storeContext, 
                  storeService, 
                  shoppingCartModelFactory, 
                  taxService,
                  urlRecordService,
                  vendorService, 
                  videoService, 
                  webHelper, 
                  workContext,
                  mediaSettings, 
                  orderSettings,
                  seoSettings, 
                  shippingSettings,
                  vendorSettings)
        {
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _priceFormatter = priceFormatter;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Contains logic of base method.
        /// Sets up custom properties to collection of <see cref="ProductOverviewModel"/>
        /// </summary>
        /// <param name="products">Products to be prepared</param>
        /// <param name="preparePriceModel">Prepare <see cref="ProductOverviewModel.ProductOverviewPriceModel"/></param>
        /// <param name="preparePictureModel">Prepare <see cref="ProductOverviewModel.DefaultPictureModel"/></param>
        /// <param name="productThumbPictureSize">ThumbPictureSize</param>
        /// <param name="prepareSpecificationAttributes">Prepare <see cref="ProductOverviewModel.SpecificationAttributeModels"/></param>
        /// <param name="forceRedirectionAfterAddingToCart">Prepare <see cref="ProductOverviewModel.ProductOverviewPriceModel"/> param</param>
        /// <returns>Product Overview Models</returns>
        public override async Task<IEnumerable<ProductOverviewModel>> PrepareProductOverviewModelsAsync(IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true, int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
            {
                throw new ArgumentNullException("products");
            }

            var models = new List<ProductOverviewModel>();
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

                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Sets up custom properties to <see cref="ProductDetailsModel.ProductPriceModel"/>
        /// </summary>
        /// <param name="product">A product to be prepared</param>
        /// <returns>Product price model</returns>
        protected override async Task<ProductDetailsModel.ProductPriceModel> PrepareProductPriceModelAsync(Product product)
        {
            var productPriceModel = await base.PrepareProductPriceModelAsync(product);
            //productPriceModel.CustomProperties.Add(NopFiluetCommonDefaults.VolumePoints, (await product.GetAttributeAsync<double>(ProductAttributeNames.VolumePoints)).ToString());
            productPriceModel.CustomProperties.Add(NopFiluetCommonDefaults.VolumePoints, (await _genericAttributeService.GetAttributeAsync<double>(product,ProductAttributeNames.VolumePoints)).ToString());

            return productPriceModel;
        }

        /// <summary>
        /// Sets up custom properties to <see cref="ProductDetailsModel"/>
        /// </summary>
        /// <param name="product">A product to be prepared</param>
        /// <param name="updatecartitem">A shopping cart item</param>
        /// <param name="isAssociatedProduct">Prevents circular references</param>
        /// <returns>ProductDetailsModel</returns>
        public override async Task<ProductDetailsModel> PrepareProductDetailsModelAsync(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            var productDetailModel = await base.PrepareProductDetailsModelAsync(product, updatecartitem, isAssociatedProduct);
            var extendedModel = AutoMapperConfiguration.Mapper.Map<FiluetProductDetailsModel>(productDetailModel);
            extendedModel.BasicRetailPrice = await _priceFormatter.FormatPriceAsync(await _genericAttributeService.GetAttributeAsync<decimal>(product,FiluetAdminProductModelFactory.BasicRetailPriceAttribute));
            extendedModel.EarnBasePrice = await _priceFormatter.FormatPriceAsync(await _genericAttributeService.GetAttributeAsync<decimal>(product,FiluetAdminProductModelFactory.EarnBasePriceAttribute));
            extendedModel.ProductForOrderCategory =await _genericAttributeService.GetAttributeAsync<string>(product,FiluetAdminProductModelFactory.ProductForOrderCategoryAttribute);
            return extendedModel;
        }

        #endregion
    }
}