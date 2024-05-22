using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
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
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Controllers
{
    [NameControllerModelConvention("Product")]
    public class FiluetProductController : ProductController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILogger _logger;


        #endregion

        #region Ctor

        public FiluetProductController(IAclService aclService,
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
            IStaticCacheManager staticCacheManager,
            ILogger logger) : base(aclService, backInStockSubscriptionService, categoryService, copyProductService, customerActivityService, customerService, discountService, downloadService, exportManager, genericAttributeService, httpClientFactory, importManager, languageService, localizationService, localizedEntityService, manufacturerService, fileProvider, notificationService, pdfService, permissionService, pictureService, productAttributeFormatter, productAttributeParser, productAttributeService, productModelFactory, productService, productTagService, settingService, shippingService, shoppingCartService, specificationAttributeService, storeContext, urlRecordService, videoService, webHelper, workContext, vendorSettings)
        {
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _permissionService = permissionService;
            _productService = productService;
            _urlRecordService = urlRecordService;
            _staticCacheManager = staticCacheManager;
            _logger = logger;
        }


        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected override async Task UpdateLocalesAsync(Product product, ProductModel model)
        {
            if (model is FiluetProductModel)
            {
                var filuetProductModel = model as FiluetProductModel;

                foreach (var localized in filuetProductModel.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(product,
                        x => x.Name,
                        localized.Name,
                        localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(product,
                        x => x.ShortDescription,
                        localized.ShortDescription,
                        localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(product,
                        x => x.FullDescription,
                        localized.FullDescription,
                        localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(product,
                        x => x.MetaKeywords,
                        localized.MetaKeywords,
                        localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(product,
                        x => x.MetaDescription,
                        localized.MetaDescription,
                        localized.LanguageId);
                    await _localizedEntityService.SaveLocalizedValueAsync(product,
                        x => x.MetaTitle,
                        localized.MetaTitle,
                        localized.LanguageId);

                    //search engine name
                    var seName = await _urlRecordService.ValidateSeNameAsync(product, localized.SeName, localized.Name, false);
                    await _urlRecordService.SaveSlugAsync(product, seName, localized.LanguageId);
                }
            }
            else
            {
                await base.UpdateLocalesAsync(product, model);
            }
        }

        #endregion

        #region Methods

        [NonAction]
        public override Task<IActionResult> Create(ProductModel model, bool continueEditing)
        {
            return base.Create(model, continueEditing);
        }

        [NonAction]
        public override Task<IActionResult> Edit(ProductModel model, bool continueEditing)
        {
            return base.Edit(model, continueEditing);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(FiluetProductModel model, bool continueEditing)
        {
            await PrepareFiluetProductModel(model);

            return await base.Edit(model, continueEditing);
        }

        private async Task PrepareFiluetProductModel(FiluetProductModel model)
        {
            var product = await _productService.GetProductByIdAsync(model.Id);

            var productDescription = new ProductDescription();
            try
            {
                productDescription = JsonConvert.DeserializeObject<ProductDescription>(product.FullDescription);
            }
            catch { }

            productDescription.Description = model.ProductDescription.Description;
            model.FullDescription = JsonConvert.SerializeObject(productDescription);

            foreach (var locale in model.Locales)
            {
                try
                {
                    var localeFullDescription = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: locale.LanguageId);
                    productDescription = JsonConvert.DeserializeObject<ProductDescription>(localeFullDescription);
                }
                catch { }

                productDescription.Description = locale.ProductDescription.Description;
                locale.FullDescription = JsonConvert.SerializeObject(productDescription);
            }

            await _genericAttributeService.SaveAttributeAsync(product, FiluetAdminProductModelFactory.EarnBasePriceAttribute, model.EarnBasePrice);
            await _genericAttributeService.SaveAttributeAsync(product, FiluetAdminProductModelFactory.BasicRetailPriceAttribute, model.BasicRetailPrice);
            await _genericAttributeService.SaveAttributeAsync(product, FiluetAdminProductModelFactory.ProductForOrderCategoryAttribute, model.ProductForOrderCategory);

            await _staticCacheManager.RemoveByPrefixAsync("nop.pres.nop.ajax.filters.filtered.products");
            
        }

        [HttpPost]
        public async Task<IActionResult> PdfFileList(PdfFileSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            var product = await _productService.GetProductByIdAsync(searchModel.ProductId)
                ?? throw new ArgumentException($"No product found with the specified id: {searchModel.ProductId}");

            var model = new PdfFileListModel();

            try
            {
                var description = string.Empty;
                if (searchModel.LanguageId > 0)
                {
                    description = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: searchModel.LanguageId);
                }
                else
                {
                    description = product.FullDescription;
                }
                if (description != null)
                {
                    var productDescription = JsonConvert.DeserializeObject<ProductDescription>(description);

                    var pdfFilePagedList = new PagedList<PdfFile>(productDescription.PdfFiles, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

                    model = await model.PrepareToGridAsync(searchModel, pdfFilePagedList, () =>
                    {
                        return pdfFilePagedList.SelectAwait(async pdfFile =>
                        {
                            var pdfFileModel = new PdfFileModel
                            {
                                DownloadId = pdfFile.DownloadId,
                                Label = pdfFile.Label,
                                ProductId = product.Id,
                                LanguageId = searchModel.LanguageId
                            };
                            return await Task.FromResult(pdfFileModel);
                           // return pdfFileModel;
                        });
                    });
                }
                
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("[FiluetProductController.PdfFileList]", ex);
            }

            return Json(model);
        }

        public async Task<IActionResult> PdfFileCreatePopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var model = new PdfFileModel();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> PdfFileCreatePopup(PdfFileModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var product = await _productService.GetProductByIdAsync(model.ProductId)
                    ?? throw new ArgumentException($"No product found with the specified id: {model.ProductId}");

            var download = await _downloadService.GetDownloadByIdAsync(model.DownloadId);
            if (download == null)
                return View(model);

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.LanguageId > 0)
                    {
                        var description = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: model.LanguageId);
                        var productLacaleDescription = new ProductDescription();
                        try
                        {
                            productLacaleDescription = JsonConvert.DeserializeObject<ProductDescription>(description);
                        }
                        catch (Exception ex)
                        {
                            await _logger.ErrorAsync("[FiluetProductController.PdfFileCreatePopup] deserialization error", ex);
                        }

                        productLacaleDescription.PdfFiles.Add(new PdfFile
                        {
                            DownloadGuid = download.DownloadGuid,
                            Label = model.Label,
                            DownloadId = model.DownloadId
                        });

                        var jsonLocaleProductDescription = JsonConvert.SerializeObject(productLacaleDescription);
                        await _localizedEntityService.SaveLocalizedValueAsync(product, x => x.FullDescription, jsonLocaleProductDescription, model.LanguageId);
                    }
                    else
                    {
                        var productDescription = new ProductDescription();
                        try
                        {
                            productDescription = JsonConvert.DeserializeObject<ProductDescription>(product.FullDescription);
                        }
                        catch (Exception ex)
                        {
                            await _logger.ErrorAsync("[FiluetProductController.PdfFileCreatePopup] deserialization error", ex);
                        }

                        productDescription.PdfFiles.Add(new PdfFile
                        {
                            DownloadGuid = download.DownloadGuid,
                            Label = model.Label,
                            DownloadId = model.DownloadId
                        });

                        product.FullDescription = JsonConvert.SerializeObject(productDescription);
                        await _productService.UpdateProductAsync(product);
                    }

                    ViewBag.RefreshPage = true;
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync("[FiluetProductController.PdfFileCreatePopup]", ex);
                }
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PdfFileDelete(PdfFileModel model,int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var product = await _productService.GetProductByIdAsync(model.ProductId)
                    ?? throw new ArgumentException($"No product found with the specified id: {model.ProductId}");

            // var download = await _downloadService.GetDownloadByIdAsync(model.DownloadId);
            // Use model id instead of download id
            var download = await _downloadService.GetDownloadByIdAsync(model.Id);
            if (download == null)
            {
                await _logger.WarningAsync($"[FiluetProductController.cs PdfFileDelete]. No Download file found with the specified id: {model.DownloadId}");
            }

            try
            {
                if (model.LanguageId > 0)
                {
                    var description = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: model.LanguageId);

                    var productLacaleDescription = JsonConvert.DeserializeObject<ProductDescription>(description);
                    productLacaleDescription.PdfFiles = productLacaleDescription.PdfFiles
                        .Where(p => p.DownloadId != model.Id)
                        .ToList();

                    var jsonLocaleProductDescription = JsonConvert.SerializeObject(productLacaleDescription);
                    await _localizedEntityService.SaveLocalizedValueAsync(product, x => x.FullDescription, jsonLocaleProductDescription, model.LanguageId);
                }
                else
                {
                    var productDescription = JsonConvert.DeserializeObject<ProductDescription>(product.FullDescription);
                    productDescription.PdfFiles = productDescription.PdfFiles
                        .Where(p => p.DownloadId != model.Id)
                        .ToList();
                    product.FullDescription = JsonConvert.SerializeObject(productDescription);
                    await _productService.UpdateProductAsync(product);
                }

                var downloadToDelete = await _downloadService.GetDownloadByIdAsync(model.Id);
                if (downloadToDelete != null)
                {
                    await _downloadService.DeleteDownloadAsync(downloadToDelete);
                }

                ViewBag.RefreshPage = true;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync("[FiluetProductController.PdfFileDelete]", ex);

                return new NullJsonResult();
            }


            return new NullJsonResult();
        }
    }

    #endregion
}
