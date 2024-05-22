using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.ExportImport;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Controllers
{
    [NameControllerModelConvention("Category")]
    public class FiluetCategoryController : CategoryController
    {
        #region Fields

        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICategoryService _categoryService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ThemeFiluetHerbalifeSettings _themeFiluetHerbalifeSettings;

        #endregion

        #region Ctor

        public FiluetCategoryController(
            IAclService aclService,
            ICategoryModelFactory categoryModelFactory,
            ICategoryService categoryService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IExportManager exportManager,
            IImportManager importManager,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            ILogger logger,
            ThemeFiluetHerbalifeSettings themeFiluetHerbalifeSettings) : base(aclService, categoryModelFactory, categoryService, customerActivityService, customerService, discountService, exportManager, importManager, localizationService, localizedEntityService, notificationService, permissionService, pictureService, productService, staticCacheManager, storeMappingService, storeService, urlRecordService, workContext)
        {
            _categoryService = categoryService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _permissionService = permissionService;
            _urlRecordService = urlRecordService;
            _logger = logger;
            _themeFiluetHerbalifeSettings = themeFiluetHerbalifeSettings;
        }


        #endregion

        #region Utilities 

        protected override async Task UpdateLocalesAsync(Category category, CategoryModel model)
        {
            if (model is FiluetCategoryModel)
            {
                var filuetProgramCategoryModel = model as FiluetCategoryModel;

                foreach (var localized in filuetProgramCategoryModel.Locales)
                {
                    await _localizedEntityService.SaveLocalizedValueAsync(category,
                        x => x.Name,
                        localized.Name,
                        localized.LanguageId);

                    await _localizedEntityService.SaveLocalizedValueAsync(category,
                        x => x.Description,
                        localized.Description,
                        localized.LanguageId);

                    await _localizedEntityService.SaveLocalizedValueAsync(category,
                        x => x.MetaKeywords,
                        localized.MetaKeywords,
                        localized.LanguageId);

                    await _localizedEntityService.SaveLocalizedValueAsync(category,
                        x => x.MetaDescription,
                        localized.MetaDescription,
                        localized.LanguageId);

                    await _localizedEntityService.SaveLocalizedValueAsync(category,
                        x => x.MetaTitle,
                        localized.MetaTitle,
                        localized.LanguageId);

                    //search engine name
                    var seName = await _urlRecordService.ValidateSeNameAsync(category, localized.SeName, localized.Name, false);
                    await _urlRecordService.SaveSlugAsync(category, seName, localized.LanguageId);
                }
            }
            else
            {
                await base.UpdateLocalesAsync(category, model);
            }
        }

        #endregion

        #region Methods

        #region Create / Edit

        [NonAction]
        public override async Task<IActionResult> Create(CategoryModel model, bool continueEditing)
        {
            return await base.Create(model, continueEditing);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> Create(FiluetCategoryModel model, bool continueEditing)
        {
            await PrepareFiluetCategoryModel(model);

            return await base.Create(model, continueEditing);
        }

        [NonAction]
        public override async Task<IActionResult> Edit(CategoryModel model, bool continueEditing)
        {
            return await base.Edit(model, continueEditing);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> Edit(FiluetCategoryModel model, bool continueEditing)
        {
            await PrepareFiluetCategoryModel(model);

            return await base.Edit(model, continueEditing);
        }


        private async Task PrepareFiluetCategoryModel(FiluetCategoryModel model)
        {
            if (model.ParentCategoryId == _themeFiluetHerbalifeSettings.CategoryIdForProgramm)
            {
                model.Description = JsonConvert.SerializeObject(model.ProgramCategoryModel);
                foreach (var locale in model.Locales)
                {
                    locale.Description = JsonConvert.SerializeObject(locale.ProgramCategoryModel);
                }
            }

            var parentCategory = await _categoryService.GetCategoryByIdAsync(model.ParentCategoryId);
            if (parentCategory != null && parentCategory.ParentCategoryId == _themeFiluetHerbalifeSettings.CategoryIdForProgramm)
            {
                Category category = null;

                model.ProgramSubCategoryModel = new ProgramSubCategoryModel();
                try
                {
                    category = await _categoryService.GetCategoryByIdAsync(model.Id);
                    var subProgramCategoryModelDb = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(category.Description);
                    model.ProgramSubCategoryModel.ProgramBenefits = subProgramCategoryModelDb.ProgramBenefits;
                }
                catch { }

                model.ProgramSubCategoryModel.Name = model.Name;
                model.Description = JsonConvert.SerializeObject(model.ProgramSubCategoryModel);

                foreach (var locale in model.Locales)
                {
                    try
                    {                        
                        var descriptionDb = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId: locale.LanguageId);
                        var subProgramCategoryModelDb = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(descriptionDb);
                        locale.ProgramSubCategoryModel.ProgramBenefits = subProgramCategoryModelDb.ProgramBenefits;                        
                    }
                    catch { }

                    locale.ProgramSubCategoryModel.Name = locale.Name;
                    locale.Description = JsonConvert.SerializeObject(locale.ProgramSubCategoryModel);
                }
            }
        }

        #endregion

        #region SubCategory Benefits

        [HttpPost]
        public virtual async Task<IActionResult> SubProgramBenefitList(SubProgramBenefitSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var category = await _categoryService.GetCategoryByIdAsync(searchModel.CategoryId)
                ?? throw new ArgumentException($"No category found with the specified id: {searchModel.CategoryId}");

            var description = string.Empty;
            var subProgramCategoryModel = new ProgramSubCategoryModel();
            if (searchModel.LanguageId > 0)
            {
                description = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId: searchModel.LanguageId);
                subProgramCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(description);
            }
            else
            {
                description = category.Description;
                if (!string.IsNullOrEmpty(description))
                {
                    subProgramCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(description);
                }
            }

            var programBenefitPagedList = new PagedList<SubProgramBenefitModel>(subProgramCategoryModel.ProgramBenefits, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);
            var subProgramBenefitListModel = await new SubProgramBenefitListModel().PrepareToGridAsync(searchModel, programBenefitPagedList, () =>
            {
                return programBenefitPagedList.ToAsyncEnumerable();
            });

            return Json(subProgramBenefitListModel);
        }

        public async Task<IActionResult> SubProgramBenefitCreatePopup()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var model = new SubProgramBenefitModel();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public async Task<IActionResult> SubProgramBenefitCreatePopup(SubProgramBenefitModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId)
                ?? throw new ArgumentException($"No category found with the specified id: {model.CategoryId}");

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.LanguageId > 0)
                    {
                        var description = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId: model.LanguageId);
                        var programSubCategoryModel = new ProgramSubCategoryModel();
                        try
                        {
                            programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(description);
                        }
                        catch (Exception ex)
                        {
                            await _logger.ErrorAsync($"[FiluetCategoryController.SubCategoryBenefitCreatePopup] deserialization error. {model.CategoryId}", ex);
                        }

                        programSubCategoryModel.ProgramBenefits.Add(new SubProgramBenefitModel
                        {
                            Uid = Guid.NewGuid(),
                            Name = model.Name,
                            Description = model.Description
                        });

                        var jsonProgramSubCategoryModel = JsonConvert.SerializeObject(programSubCategoryModel);
                        await _localizedEntityService.SaveLocalizedValueAsync(category, x => x.Description, jsonProgramSubCategoryModel, model.LanguageId);
                    }
                    else
                    {
                        var programSubCategoryModel = new ProgramSubCategoryModel();
                        try
                        {
                            programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(category.Description);
                        }
                        catch (Exception ex)
                        {
                            await _logger.ErrorAsync($"[FiluetCategoryController.SubCategoryBenefitCreatePopup] deserialization error. {model.CategoryId}", ex);
                        }

                        programSubCategoryModel.ProgramBenefits.Add(new SubProgramBenefitModel
                        {
                            Uid = Guid.NewGuid(),
                            Name = model.Name,
                            Description = model.Description
                        });

                        category.Description = JsonConvert.SerializeObject(programSubCategoryModel);
                        await _categoryService.UpdateCategoryAsync(category);
                    }

                    ViewBag.RefreshPage = true;
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"[FiluetCategoryController.SubCategoryBenefitCreatePopup] {model.CategoryId} {model.LanguageId}", ex);
                }
            }

            return View(model);
        }

        public virtual async Task<IActionResult> SubProgramBenefitEditPopup(Guid subProgramBenefitUid, int categoryId, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var category = await _categoryService.GetCategoryByIdAsync(categoryId)
                ?? throw new ArgumentException($"No category found with the specified id: {categoryId}");

            var subProgramBenefitModel = new SubProgramBenefitModel();

            try
            {
                if (languageId > 0)
                {
                    var description = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId: languageId);
                    try
                    {
                        var programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(description);
                        subProgramBenefitModel = programSubCategoryModel.ProgramBenefits.FirstOrDefault(p => p.Uid == subProgramBenefitUid);
                    }
                    catch (Exception ex)
                    {
                        await _logger.ErrorAsync($"[FiluetCategoryController.SubProgramBenefitEditPopup] deserialization error. categoryId={categoryId}", ex);
                    }
                }
                else
                {
                    try
                    {
                        var programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(category.Description);
                        subProgramBenefitModel = programSubCategoryModel.ProgramBenefits.FirstOrDefault(p => p.Uid == subProgramBenefitUid);
                    }
                    catch (Exception ex)
                    {
                        await _logger.ErrorAsync($"[FiluetCategoryController.SubProgramBenefitEditPopup] deserialization error. categoryId={categoryId}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"[FiluetCategoryController.SubProgramBenefitEditPopup] categoryId={categoryId}, languageId={languageId}", ex);
            }


            return View(subProgramBenefitModel);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubProgramBenefitEditPopup(SubProgramBenefitModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var category = await _categoryService.GetCategoryByIdAsync(model.CategoryId)
                ?? throw new ArgumentException($"No category found with the specified id: {model.CategoryId}");

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.LanguageId > 0)
                    {
                        var description = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId: model.LanguageId);
                        var programSubCategoryModel = new ProgramSubCategoryModel();
                        try
                        {
                            programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(description);
                        }
                        catch (Exception ex)
                        {
                            await _logger.ErrorAsync($"[FiluetCategoryController.SubCategoryBenefitCreatePopup] deserialization error. categoryId={model.CategoryId}", ex);
                        }

                        var programBenefit = programSubCategoryModel.ProgramBenefits.FirstOrDefault(p => p.Uid == model.Uid);
                        if (programBenefit != null)
                        {
                            programBenefit.Name = model.Name;
                            programBenefit.Description = model.Description;

                            var jsonProgramSubCategoryModel = JsonConvert.SerializeObject(programSubCategoryModel);
                            await _localizedEntityService.SaveLocalizedValueAsync(category, x => x.Description, jsonProgramSubCategoryModel, model.LanguageId);
                        }
                    }
                    else
                    {
                        var programSubCategoryModel = new ProgramSubCategoryModel();
                        try
                        {
                            programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(category.Description);
                        }
                        catch (Exception ex)
                        {
                            await _logger.ErrorAsync($"[FiluetCategoryController.SubCategoryBenefitCreatePopup] deserialization error. categoryId={model.CategoryId}", ex);
                        }

                        var programBenefit = programSubCategoryModel.ProgramBenefits.FirstOrDefault(p => p.Uid == model.Uid);
                        if (programBenefit != null)
                        {
                            programBenefit.Name = model.Name;
                            programBenefit.Description = model.Description;

                            category.Description = JsonConvert.SerializeObject(programSubCategoryModel);
                            await _categoryService.UpdateCategoryAsync(category);
                        }
                    }

                    ViewBag.RefreshPage = true;
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"[FiluetCategoryController.SubCategoryBenefitCreatePopup] categoryId={model.CategoryId}, languageId={model.LanguageId}", ex);
                }
            }

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SubProgramBenefitDelete(Guid id, int categoryId, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCategories))
                return await AccessDeniedDataTablesJson();

            var category = await _categoryService.GetCategoryByIdAsync(categoryId)
                ?? throw new ArgumentException($"No category found with the specified id: {categoryId}");

            try
            {
                if (languageId > 0)
                {
                    var description = await _localizationService.GetLocalizedAsync(category, x => x.Description, languageId: languageId);
                    var programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(description);
                    programSubCategoryModel.ProgramBenefits = programSubCategoryModel.ProgramBenefits.Where(p => p.Uid != id).ToList();
                    var jsonProgramSubCategoryModel = JsonConvert.SerializeObject(programSubCategoryModel);
                    await _localizedEntityService.SaveLocalizedValueAsync(category, x => x.Description, jsonProgramSubCategoryModel, languageId);
                }
                else
                {
                    var programSubCategoryModel = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(category.Description);
                    programSubCategoryModel.ProgramBenefits = programSubCategoryModel.ProgramBenefits.Where(p => p.Uid != id).ToList();
                    category.Description = JsonConvert.SerializeObject(programSubCategoryModel);
                    await _categoryService.UpdateCategoryAsync(category);
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"[FiluetCategoryController.SubProgramBenefitDelete] categoryId={categoryId}, languageId={languageId}", ex);
            }

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}
