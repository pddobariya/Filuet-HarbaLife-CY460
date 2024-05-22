using ISDK.Filuet.Theme.FiluetHerbalife.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Controllers
{
    public class FiluetCatalogController : CatalogController
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly IFiluetCatalogModelFactory _filuetCatalogModelFactory;

        #endregion

        #region Ctor

        public FiluetCatalogController(CatalogSettings catalogSettings,
            IFiluetCatalogModelFactory filuetCatalogModelFactory,
            IAclService aclService, 
            ICatalogModelFactory catalogModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService, 
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INopUrlHelper nopUrlHelper,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService, 
            IStoreContext storeContext,
            IStoreMappingService storeMappingService, 
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings) : base(catalogSettings, aclService, catalogModelFactory, categoryService, customerActivityService, genericAttributeService, localizationService, manufacturerService, nopUrlHelper, permissionService, productModelFactory, productService, productTagService, storeContext, storeMappingService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, vendorSettings)
        {
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _filuetCatalogModelFactory = filuetCatalogModelFactory;

        }

        #endregion

        #region Methods

        public async Task<IActionResult> FiluetSearch(FiluetSearchModel model, CatalogProductsCommand command)
        {
            await _genericAttributeService.SaveAttributeAsync(await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.LastContinueShoppingPageAttribute,
                _webHelper.GetThisPageUrl(true),
                (await _storeContext.GetCurrentStoreAsync()).Id);

            if (model == null)
                model = new FiluetSearchModel();
            
            model = await _filuetCatalogModelFactory.PrepareFiluetSearchModelAsync(model, command);

            return View("Search", model);
        }

        //ignore SEO friendly URLs checks
        [CheckLanguageSeoCode(true)]
        public async Task<IActionResult> FiluetSearchProducts(FiluetSearchModel searchModel, CatalogProductsCommand command)
        {
            if (searchModel == null)
                searchModel = new FiluetSearchModel();

            var model = await _filuetCatalogModelFactory.PrepareSearchProductsModelAsync(searchModel, command);

            return PartialView("_ProductsInGridOrLines", model);
        }

        #endregion
    }
}
