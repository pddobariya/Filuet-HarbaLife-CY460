using ISDK.Filuet.OnlineOrdering.CorePlugin.Factories;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.Filters;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    public class FiluetAdminProductController : ProductController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public FiluetAdminProductController(
            IAclService aclService,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            ICategoryService categoryService,
            ICopyProductService copyProductService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IExportManager exportManager,
            IGenericAttributeService genericAttributeService,
            IHttpClientFactory httpClientFactory,
            IImportManager importManager,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService, 
            IManufacturerService manufacturerService, 
            INopFileProvider fileProvider, 
            INotificationService notificationService, 
            IPdfService pdfService, 
            IPermissionService permissionService, 
            IPictureService pictureService, 
            IProductAttributeFormatter productAttributeFormatter, 
            IProductAttributeParser productAttributeParser, 
            IProductAttributeService productAttributeService, 
            IProductModelFactory productModelFactory, 
            IProductService productService, 
            IProductTagService productTagService, 
            ISettingService settingService, 
            IShippingService shippingService, 
            IShoppingCartService shoppingCartService, 
            ISpecificationAttributeService specificationAttributeService, 
            IStoreContext storeContext, 
            IUrlRecordService urlRecordService,
            IVideoService videoService, 
            IWebHelper webHelper, 
            IWorkContext workContext, 
            VendorSettings vendorSettings, 
            IStaticCacheManager staticCacheManager) 
            : base(aclService,
                  backInStockSubscriptionService,
                  categoryService,
                  copyProductService,
                  customerActivityService,
                  customerService,
                  discountService,
                  downloadService,
                  exportManager,
                  genericAttributeService,
                  httpClientFactory,
                  importManager,
                  languageService,
                  localizationService, 
                  localizedEntityService,
                  manufacturerService,
                  fileProvider,
                  notificationService,
                  pdfService,
                  permissionService,
                  pictureService,
                  productAttributeFormatter,
                  productAttributeParser,
                  productAttributeService,
                  productModelFactory,
                  productService,
                  productTagService,
                  settingService,
                  shippingService, 
                  shoppingCartService, 
                  specificationAttributeService,
                  storeContext, 
                  urlRecordService,
                  videoService,
                  webHelper,
                  workContext,
                  vendorSettings)
        {
            _productService = productService;
            _genericAttributeService = genericAttributeService;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        [NonAction]
        public override Task<IActionResult> Edit(ProductModel model, bool continueEditing)
        {
            return base.Edit(model, continueEditing);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(FiluetAdminProductModel model, bool continueEditing)
        {
            var product = await _productService.GetProductByIdAsync(model.Id);
            await _genericAttributeService.SaveAttributeAsync(product, FiluetAdminProductModelFactory.EarnBasePriceAttribute, model.EarnBasePrice);
            await _genericAttributeService.SaveAttributeAsync(product, FiluetAdminProductModelFactory.BasicRetailPriceAttribute, model.BasicRetailPrice);
            await _genericAttributeService.SaveAttributeAsync(product, FiluetAdminProductModelFactory.ProductForOrderCategoryAttribute, model.ProductForOrderCategory);

            await _staticCacheManager.RemoveByPrefixAsync("nop.pres.nop.ajax.filters.filtered.products");            

            return await base.Edit(model, continueEditing);
        }

        #endregion
    }
}
