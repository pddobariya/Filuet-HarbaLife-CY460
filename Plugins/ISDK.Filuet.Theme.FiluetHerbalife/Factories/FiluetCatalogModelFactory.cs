using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public class FiluetCatalogModelFactory : CatalogModelFactory, IFiluetCatalogModelFactory
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly ThemeFiluetHerbalifeSettings _themeFiluetHerbalifeSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CatalogSettings _catalogSettings;
        private readonly ISearchTermService _searchTermService;
        private readonly IProductService _productService;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Ctor

        public FiluetCatalogModelFactory(
            BlogSettings blogSettings,
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
            ILogger logger,
            ThemeFiluetHerbalifeSettings themeFiluetHerbalifeSettings) : base(blogSettings, catalogSettings, displayDefaultMenuItemSettings, forumSettings, categoryService, categoryTemplateService, currencyService, customerService, eventPublisher, httpContextAccessor, localizationService, manufacturerService, manufacturerTemplateService, nopUrlHelper, pictureService, productModelFactory, productService, productTagService, searchTermService, specificationAttributeService, staticCacheManager, storeContext, topicService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, vendorSettings)
        {
            _logger = logger;
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _eventPublisher = eventPublisher;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _productService = productService;
            _searchTermService = searchTermService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _themeFiluetHerbalifeSettings = themeFiluetHerbalifeSettings;
        }

        #endregion

        #region Methods

        public override async Task<CategoryModel> PrepareCategoryModelAsync(Category category, CatalogProductsCommand command)
        {

            var model = await base.PrepareCategoryModelAsync(category, command);

            if (category.ParentCategoryId == _themeFiluetHerbalifeSettings.CategoryIdForProgramm)
            {
                var programModel = PluginMapper.Mapper.Map<FiluetCategoryModel>(model);

                try
                {
                    programModel.ProgramCategoryModel = JsonConvert.DeserializeObject<ProgramCategoryModel>(model.Description);
                }
                catch { }

                return programModel;
            }


            var parentCategory = await _categoryService.GetCategoryByIdAsync(category.ParentCategoryId);
            if (parentCategory != null && parentCategory.ParentCategoryId == _themeFiluetHerbalifeSettings.CategoryIdForProgramm)
            {
                var parentModel = await base.PrepareCategoryModelAsync(parentCategory, command);
                var filuetCategoryModel = PluginMapper.Mapper.Map<FiluetCategoryModel>(model);
                try
                {
                    filuetCategoryModel.ProgramSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(model.Description);

                    filuetCategoryModel.ProgramSubCategoryModel.ImageSrc = (await PrepearPictureModel(category)).ImageUrl;

                    var secondSubCategory = parentModel.SubCategories.FirstOrDefault(x => x.Name.Trim().ToLower() == filuetCategoryModel.ProgramSubCategoryModel.SecondProgramTypeName?.Trim().ToLower());

                    filuetCategoryModel.ProgramSubCategoryModel.SecondProgramImgRef = secondSubCategory?.PictureModel.ImageUrl;

                    var thirdSubCategory = parentModel.SubCategories.FirstOrDefault(x => x.Name.Trim().ToLower() == filuetCategoryModel.ProgramSubCategoryModel.ThirdProgramTypeName?.Trim().ToLower());

                    filuetCategoryModel.ProgramSubCategoryModel.ThirdProgramImgRef = thirdSubCategory?.PictureModel.ImageUrl;



                }
                catch { }

                return filuetCategoryModel;
            }


            return model;
        }

        private async Task<PictureModel> PrepearPictureModel(Category curCategory)
        {
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey, curCategory,
                                    pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(),
                                    currentStore);

            return await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
            {
                var picture = await _pictureService.GetPictureByIdAsync(curCategory.PictureId);
                string fullSizeImageUrl, imageUrl;

                (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = fullSizeImageUrl,
                    ImageUrl = imageUrl,
                    Title = string.Format(await _localizationService
                        .GetResourceAsync("Media.Category.ImageLinkTitleFormat"), curCategory.Name),
                    AlternateText = string.Format(await _localizationService
                        .GetResourceAsync("Media.Category.ImageAlternateTextFormat"), curCategory.Name)
                };

                return pictureModel;
            });
        }

        public async Task<FiluetSearchModel> PrepareFiluetSearchModelAsync(FiluetSearchModel model, CatalogProductsCommand command)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.SearchPriceRangeFrom) && !string.IsNullOrEmpty(model.SearchPriceRangeTo))
                    command.Price = $"{model.SearchPriceRangeFrom}-{model.SearchPriceRangeTo}";

                await base.PrepareSearchModelAsync(model, command);

                await _logger.InformationAsync($"command.Price: {command.Price}, " +
                          $"model.CatalogProductsModel.PriceRangeFilter.From: {model?.CatalogProductsModel?.PriceRangeFilter?.SelectedPriceRange?.From}," +
                          $"model.CatalogProductsModel.PriceRangeFilter.To: {model?.CatalogProductsModel?.PriceRangeFilter?.SelectedPriceRange?.To}");

                return model;
            }
            catch (Exception ex)
            {
                await _logger.InformationAsync($"[FiluetCatalogModelFactory] Error: {ex}");
            }
            return model;
        }

        //Override method to enable catalogSetting in EnablePriceRangeFiltering field 
        public override async Task<CatalogProductsModel> PrepareSearchProductsModelAsync(SearchModel searchModel, CatalogProductsCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var model = new CatalogProductsModel
            {
                UseAjaxLoading = _catalogSettings.UseAjaxCatalogProductsLoading
            };

            //sorting
            await PrepareSortingOptionsAsync(model, command);
            //view mode
            await PrepareViewModesAsync(model, command);
            //page size
            await PreparePageSizeOptionsAsync(model, command, _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions, _catalogSettings.SearchPageProductsPerPage);

            var searchTerms = searchModel.q == null
                ? string.Empty
                : searchModel.q.Trim();

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            //only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            //we don't use "!string.IsNullOrEmpty(searchTerms)" in cases of "ProductSearchTermMinimumLength" set to 0 but searching by other parameters (e.g. category or price filter)
            var isSearchTermSpecified = _httpContextAccessor.HttpContext.Request.Query.ContainsKey("q");
            if (isSearchTermSpecified)
            {
                var currentStore = await _storeContext.GetCurrentStoreAsync();

                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.WarningMessage =
                        string.Format(await _localizationService.GetResourceAsync("Search.SearchTermMinimumLengthIsNCharacters"),
                            _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    var manufacturerId = 0;
                    var searchInDescriptions = false;
                    var vendorId = 0;
                    if (searchModel.advs)
                    {
                        //advanced search
                        var categoryId = searchModel.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (searchModel.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(
                                    await _categoryService.GetChildCategoryIdsAsync(categoryId, currentStore.Id));
                            }
                        }

                        manufacturerId = searchModel.mid;

                        if (searchModel.asv)
                            vendorId = searchModel.vid;

                        searchInDescriptions = searchModel.sid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;
                    var workingLanguage = await _workContext.GetWorkingLanguageAsync();

                    //price range
                    PriceRangeModel selectedPriceRange = null;
                    ISettingService settingService = EngineContext.Current.Resolve<ISettingService>();
                    if (!_catalogSettings.EnablePriceRangeFiltering)
                    {
                        _catalogSettings.EnablePriceRangeFiltering = true;
                        await settingService.SaveSettingAsync(_catalogSettings);
                    }

                    if (_catalogSettings.EnablePriceRangeFiltering && _catalogSettings.SearchPagePriceRangeFiltering)
                    {
                        selectedPriceRange = await GetConvertedPriceRangeAsync(command);

                        PriceRangeModel availablePriceRange;
                        async Task<decimal?> getProductPriceAsync(ProductSortingEnum orderBy)
                        {
                            var products = await _productService.SearchProductsAsync(0, 1,
                                categoryIds: categoryIds,
                                manufacturerIds: new List<int> { manufacturerId },
                                storeId: currentStore.Id,
                                visibleIndividuallyOnly: true,
                                keywords: searchTerms,
                                searchDescriptions: searchInDescriptions,
                                searchProductTags: searchInProductTags,
                                languageId: workingLanguage.Id,
                                vendorId: vendorId,
                                orderBy: orderBy);

                            return products?.FirstOrDefault()?.Price ?? 0;
                        }

                        if (_catalogSettings.SearchPageManuallyPriceRange)
                        {
                            var to = await getProductPriceAsync(ProductSortingEnum.PriceDesc);

                            availablePriceRange = new PriceRangeModel
                            {
                                From = _catalogSettings.SearchPagePriceFrom,
                                To = to == 0 ? 0 : _catalogSettings.SearchPagePriceTo
                            };
                        }
                        else
                            availablePriceRange = new PriceRangeModel
                            {
                                From = await getProductPriceAsync(ProductSortingEnum.PriceAsc),
                                To = await getProductPriceAsync(ProductSortingEnum.PriceDesc)
                            };

                        model.PriceRangeFilter = await PreparePriceRangeFilterAsync(selectedPriceRange, availablePriceRange);
                    }

                    //products
                    products = await _productService.SearchProductsAsync(
                        command.PageNumber - 1,
                        command.PageSize,
                        categoryIds: categoryIds,
                        manufacturerIds: new List<int> { manufacturerId },
                        storeId: currentStore.Id,
                        visibleIndividuallyOnly: true,
                        keywords: searchTerms,
                        priceMin: selectedPriceRange?.From,
                        priceMax: selectedPriceRange?.To,
                        searchDescriptions: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: workingLanguage.Id,
                        orderBy: (ProductSortingEnum)command.OrderBy,
                        vendorId: vendorId);

                    //search term statistics
                    if (!string.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm =
                            await _searchTermService.GetSearchTermByKeywordAsync(searchTerms, currentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            await _searchTermService.UpdateSearchTermAsync(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = currentStore.Id,
                                Count = 1
                            };
                            await _searchTermService.InsertSearchTermAsync(searchTerm);
                        }
                    }

                    //event
                    await _eventPublisher.PublishAsync(new ProductSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = workingLanguage.Id,
                        VendorId = vendorId
                    });
                }
            }

            var isFiltering = !string.IsNullOrEmpty(searchTerms);
            await PrepareCatalogProductsAsync(model, products, isFiltering);

            return model;
        }

        #endregion


    }
}
