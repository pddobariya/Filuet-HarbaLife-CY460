using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog;
using ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Factories
{
    public class FiluetCategoryModelFactory : CategoryModelFactory
    {
        #region Fields

        private readonly int _categoryIdForProgramm;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor

        public FiluetCategoryModelFactory(
            CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICurrencyService currencyService,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICategoryService categoryService,
            IDiscountService discountService,
            IDiscountSupportedModelFactory discountSupportedModelFactory,
            ILocalizationService localizationService,
            ILocalizedModelFactory localizedModelFactory,
            IProductService productService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IUrlRecordService urlRecordService,
            ThemeFiluetHerbalifeSettings themeFiluetHerbalifeSettings)
            : base(catalogSettings, currencySettings, currencyService, aclSupportedModelFactory,
            baseAdminModelFactory, categoryService, discountService, discountSupportedModelFactory,
            localizationService, localizedModelFactory, productService, storeMappingSupportedModelFactory,
            urlRecordService)
        {
            _categoryIdForProgramm = themeFiluetHerbalifeSettings.CategoryIdForProgramm;
            _categoryService = categoryService;
        }

        #endregion

        #region Methods 

        public override async Task<CategoryModel> PrepareCategoryModelAsync(CategoryModel model, Category category, bool excludeProperties = false)
        {
            model = await base.PrepareCategoryModelAsync(model, category, excludeProperties);

            if (category == null)
                return model;

            if(category.ParentCategoryId == _categoryIdForProgramm)
            {
                var filuetCategoryModel = PluginMapper.Mapper.Map<FiluetCategoryModel>(model);

                foreach (var locale in filuetCategoryModel.Locales)
                {
                    try
                    {
                        locale.ProgramCategoryModel = JsonConvert.DeserializeObject<ProgramCategoryModel>(locale.Description);
                    }
                    catch { }
                }

                return filuetCategoryModel;
            }

            var parentCategory = await _categoryService.GetCategoryByIdAsync(category.ParentCategoryId);
            if (parentCategory != null && parentCategory.ParentCategoryId == _categoryIdForProgramm)
            {
                var filuetCategoryModel = PluginMapper.Mapper.Map<FiluetCategoryModel>(model);
                filuetCategoryModel.ProgramSubCategoryModel.ProgramBenefitSearchModel = new SubProgramBenefitSearchModel
                {
                    CategoryId = filuetCategoryModel.Id,
                    LanguageId = 0
                };

                foreach (var locale in filuetCategoryModel.Locales)
                {
                    try
                    {
                        locale.ProgramSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(locale.Description);
                        locale.ProgramSubCategoryModel.ProgramBenefitSearchModel = new SubProgramBenefitSearchModel
                        {
                            CategoryId = filuetCategoryModel.Id,
                            LanguageId = locale.LanguageId
                        };
                    }
                    catch { }
                }
                return filuetCategoryModel;
            }  

            return model;
        }

        #endregion
    }
}
