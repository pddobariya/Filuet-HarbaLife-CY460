using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Catalog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Factories
{
    public class FiluetCatalogModelFactory : CatalogModelFactory
    {
        #region Fields

        public const decimal MaxDecimal = 99999999999999m;
        private readonly ICurrencyService _currencyService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly PriceRangeFilterService _priceRangeFilterService;

        #endregion

        #region Ctor

        public FiluetCatalogModelFactory(BlogSettings blogSettings,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            INopUrlHelper nopUrlHelper,
            IPictureService pictureService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            ISearchTermService searchTermService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings,
            PriceRangeFilterService priceRangeFilterService) : base(blogSettings,
             catalogSettings,
             displayDefaultMenuItemSettings,
             forumSettings,
             categoryService,
             categoryTemplateService,
             currencyService,
             customerService,
             eventPublisher,
             httpContextAccessor,
             localizationService,
             manufacturerService,
             manufacturerTemplateService,
             nopUrlHelper,
             pictureService,
             productModelFactory,
             productService,
             productTagService,
             searchTermService,
             specificationAttributeService,
             staticCacheManager,
             storeContext,
             topicService,
             urlRecordService,
             vendorService,
             webHelper,
             workContext,
             mediaSettings,
             vendorSettings)
        {
         
            _currencyService = currencyService;
            _productModelFactory = productModelFactory;
            _productService = productService;
            _storeContext = storeContext;
            _workContext = workContext;
            _priceRangeFilterService = priceRangeFilterService;
        }

        #endregion

        #region Methods

        public virtual async Task<PriceFilterModel> PreparePriceFilterModel(int priceFilterId, CatalogProductsCommand command)
        {
            var priceRange = await _priceRangeFilterService.GetPriceRangeByIdAsync(priceFilterId);
            if (priceRange == null)
                throw new ArgumentNullException(nameof(priceRange));

            var model = new PriceFilterModel
            {
                Id = priceFilterId,
                Name = string.Format(await priceRange.Name.ToLocalizedStringAsync(), priceRange.MinPrice, priceRange.MaxPrice)
            };

            //sorting
            await PrepareSortingOptionsAsync(model.CatalogProductsModel, command);
            //view mode
            await PrepareViewModesAsync(model.CatalogProductsModel, command);
            //page size
            await PreparePageSizeOptionsAsync(model.CatalogProductsModel, command,
                true,
                null,
                18);

            //price ranges

            var minPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(priceRange.MinPrice > priceRange.MaxPrice && priceRange.MaxPrice != 0 ? 0 : priceRange.MinPrice,await _workContext.GetWorkingCurrencyAsync());
            var maxPriceConverted = await _currencyService.ConvertToPrimaryStoreCurrencyAsync(priceRange.MaxPrice < priceRange.MinPrice ? MaxDecimal : priceRange.MaxPrice, await _workContext.GetWorkingCurrencyAsync());
            var store = await _storeContext.GetCurrentStoreAsync();
            //products
            var products = await _productService.SearchProductsAsync(pageIndex: command.PageIndex,
                pageSize: command.PageSize,
                storeId: store.Id,
                visibleIndividuallyOnly: true,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                orderBy: (ProductSortingEnum)command.OrderBy!);
            model.Products = (await _productModelFactory.PrepareProductOverviewModelsAsync(products)).ToList();

            model.CatalogProductsModel.LoadPagedList(products);

            return model;
        }

         public virtual async Task<PriceRangeListModel> PreparePriceRangeListModelAsync(PriceRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get stores
            var stores = (await _priceRangeFilterService.GetPriceRangesAsync()).ToPagedList(searchModel);

            //prepare list model
            var model = new PriceRangeListModel().PrepareToGrid(searchModel, stores, () =>
            {
                //fill in model values from the entity
                return stores.Select(store => store.ToModel<HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models.PriceRangeModel>());
            });

            return model;
        }

        public virtual Task<PriceRangeSearchModel> PreparePriceRangeListSearchModelAsync(PriceRangeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }

        #endregion
    }
}
