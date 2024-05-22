using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Framework.Components;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.HOMEPAGE_PRODUCT_PROGRAMS)]
    public class HomepageProductProgramsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly ThemeFiluetHerbalifeSettings _herbalifeSettings;

        #endregion

        #region Ctor

        public HomepageProductProgramsViewComponent(ICategoryService categoryService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IStaticCacheManager staticCacheManager, 
            IStoreContext storeContext,
            IUrlRecordService urlRecordService, 
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            ThemeFiluetHerbalifeSettings herbalifeSettings)
        {
            _categoryService = categoryService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _herbalifeSettings = herbalifeSettings;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var category = await _categoryService.GetCategoryByIdAsync(_herbalifeSettings.CategoryIdForProgramm);

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = await _localizationService.GetLocalizedAsync(category, y => y.Name),
                SeName = await _urlRecordService.GetSeNameAsync(category),
                Description = await _localizationService.GetLocalizedAsync(category, y => y.Description)
            };

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var pictureSize = _mediaSettings.CategoryThumbPictureSize;

            model.SubCategories = await (await _categoryService.GetAllCategoriesByParentCategoryIdAsync(_herbalifeSettings.CategoryIdForProgramm))

                .SelectAwait(async curCategory =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = curCategory.Id,
                        Name = await _localizationService.GetLocalizedAsync(curCategory, y => y.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(curCategory),
                        Description = await _localizationService.GetLocalizedAsync(curCategory, y => y.Description)
                    };

                    //prepare picture model
                    var categoryPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.CategoryPictureModelKey, curCategory,
                        pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(),
                        currentStore);

                    subCatModel.PictureModel = await _staticCacheManager.GetAsync(categoryPictureCacheKey, async () =>
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
                                .GetResourceAsync("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                            AlternateText = string.Format(await _localizationService
                                .GetResourceAsync("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                        };

                        return pictureModel;
                    });

                    return subCatModel;
                }).ToListAsync();

            return View(model);
        }

        #endregion
    }
}
