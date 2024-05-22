using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Infrastructure.Cache;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.HOME_PAGE_BEST_SELLERS)]
    public class FiluetHomepageBestSellersViewComponent : ViewComponent
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IOrderReportService _orderReportService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;

        #endregion

        #region Ctor

        public FiluetHomepageBestSellersViewComponent(CatalogSettings catalogSettings,
            IAclService aclService,
            IOrderReportService orderReportService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _orderReportService = orderReportService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
                return Content("");

            //load and cache report
            var report = await _staticCacheManager.GetAsync(
                _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.HomepageBestsellersIdsKey,
                    await _storeContext.GetCurrentStoreAsync()),
                async () => await (await _orderReportService.BestSellersReportAsync(
                    storeId: (await _storeContext.GetCurrentStoreAsync()).Id)).ToListAsync());

            //load products
            var products = await (await _productService.GetProductsByIdsAsync(report.Select(x => x.ProductId).ToArray()))
            //ACL and store mapping
            .WhereAwait(async p => await _aclService.AuthorizeAsync(p) && await _storeMappingService.AuthorizeAsync(p))
            //availability dates
            .Where(p => _productService.ProductIsAvailable(p) && p.ShowOnHomepage)
            .Take(_catalogSettings.NumberOfBestsellersOnHomepage).ToListAsync();

            if (!products.Any())
                return Content("");

            //prepare model
            var model = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
            return View(model);
        }

        #endregion
    }
}
