using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components;
using ISDK.Filuet.Theme.FiluetHerbalife.Components;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Helpers;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using SevenSpikes.Nop.Plugins.ProductRibbons.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure
{
    public class ThemeFiluetHerbalifePlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly INopFileProvider _nopFileProvider;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly HtmlConvertHelpers _htmlConvertHelpers;
        private readonly FaqDataInitHelper _faqDataInitHelper;

        #endregion

        #region Ctor

        public ThemeFiluetHerbalifePlugin(IStoreContext storeContext, 
            ILocalizationService localizationService,
            ILanguageService languageService,
            IWebHelper webHelper, 
            ISettingService settingService,
            INopFileProvider nopFileProvider, 
            ILogger logger, 
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            HtmlConvertHelpers htmlConvertHelpers, 
            FaqDataInitHelper faqDataInitHelper)
        {
            _storeContext = storeContext;
            _localizationService = localizationService;
            _languageService = languageService;
            _webHelper = webHelper;
            _settingService = settingService;
            _nopFileProvider = nopFileProvider;
            _logger = logger;
            _categoryService = categoryService;
            _categoryTemplateService = categoryTemplateService;
            _htmlConvertHelpers = htmlConvertHelpers;
            _faqDataInitHelper = faqDataInitHelper;
        }

        #endregion

        #region Methods

        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/FiluetHerbalife/Configure";
        }

        #region InstallAsync

        public override async Task InstallAsync()
        {
            try
            {
                await InstallSettings();

                await InstallLocaleResources();

                await _htmlConvertHelpers.ConvertProgramsAsync();
                await _htmlConvertHelpers.ConvertProgramTypesAsync();
                await _htmlConvertHelpers.ConvertTopicsAsync();
                await _htmlConvertHelpers.ConvertProductDescriptionAsync();

                await InstallProgramCategoryTemplateAsync();

                await _faqDataInitHelper.InitTopics();

                await base.InstallAsync();
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"[ThemeFiluetHerbalifePlugin] error: {ex}");
            }
        }

        #region InstallSettings

        private async Task InstallSettings()
        {
            await _settingService.SaveSettingAsync(new ThemeFiluetHerbalifeSettings
            {
                CategoryIdForCatalogue = FiluetThemePluginDefaults.CategoryIdForCatalogue,
                CategoryIdForProgramm = FiluetThemePluginDefaults.CategoryIdForProgramm,
                OmnivaCarrierUrl = FiluetThemePluginDefaults.OmnivaCarrierUrl,
                DPDLatviaCarrierUrl = FiluetThemePluginDefaults.DPDLatviaCarrierUrl,
                DPDLithuaniaCarrierUrl = FiluetThemePluginDefaults.DPDLithuaniaCarrierUrl,
                DPDEstoniaCarrierUrl = FiluetThemePluginDefaults.DPDEstoniaCarrierUrl
            });

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var productRibbonsSettings = await _settingService.LoadSettingAsync<ProductRibbonsSettings>(storeScope);
            productRibbonsSettings.ProductBoxSelector = GetProductRibbonsSettingsProps(productRibbonsSettings.ProductBoxSelector, "info");
            productRibbonsSettings.ProductBoxPictureContainerSelector = GetProductRibbonsSettingsProps(productRibbonsSettings.ProductBoxPictureContainerSelector, "name");
            productRibbonsSettings.ProductPageBugPictureContainerSelector = GetProductRibbonsSettingsProps(productRibbonsSettings.ProductPageBugPictureContainerSelector, "name");

            await _settingService.SaveSettingAsync(productRibbonsSettings);
        }

        private string GetProductRibbonsSettingsProps(string props, string pattern)
        {
            if (string.IsNullOrWhiteSpace(props))
                props = $".{pattern}";
            else if (!Regex.IsMatch(props, $@"\s*\.?{pattern}(\s|,|$)", RegexOptions.IgnoreCase))
                props += $", .{pattern}";

            return props;
        }

        #endregion

        #region InstallProgramCategoryTemplateAsync

        private async Task InstallProgramCategoryTemplateAsync()
        {
            var categoryTemplates = await _categoryTemplateService.GetAllCategoryTemplatesAsync();
            var programCategoryTemplate = categoryTemplates.FirstOrDefault(p => p.Name == FiluetThemePluginDefaults.ProgramCategoryTemplateName);

            if (programCategoryTemplate == null)
            {
                var newCategoryTemplate = new CategoryTemplate
                {
                    Name = FiluetThemePluginDefaults.ProgramCategoryTemplateName,
                    ViewPath = "~/Plugins/Theme.FiluetHerbalife/Views/Catalog/ProgramCategoryTemplate.ProductsInGridOrLines.cshtml",
                    DisplayOrder = 1
                };

                await _categoryTemplateService.InsertCategoryTemplateAsync(newCategoryTemplate);

                var programSubCategories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(FiluetThemePluginDefaults.CategoryIdForProgramm);
                foreach (var programSubCategory in programSubCategories)
                {
                    programSubCategory.CategoryTemplateId = newCategoryTemplate.Id;
                    await _categoryService.UpdateCategoryAsync(programSubCategory);
                }
            }
        }

        #endregion

        #endregion

        #region UninstallAsync

        public override async Task UninstallAsync()
        {
            await UnInstallSettings();

            await DeleteLocalResourcesAsync();

            await base.UninstallAsync();
        }

        #region UnInstallSettings

        private async Task UnInstallSettings()
        {
            await _settingService.DeleteSettingAsync<ThemeFiluetHerbalifeSettings>();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var productRibbonsSettings = await _settingService.LoadSettingAsync<ProductRibbonsSettings>(storeScope);
            productRibbonsSettings.ProductBoxSelector = DeleteProductRibbonsSettingsProps(productRibbonsSettings.ProductBoxSelector, "info");
            productRibbonsSettings.ProductBoxPictureContainerSelector = DeleteProductRibbonsSettingsProps(productRibbonsSettings.ProductBoxPictureContainerSelector, "name");
            productRibbonsSettings.ProductPageBugPictureContainerSelector = DeleteProductRibbonsSettingsProps(productRibbonsSettings.ProductPageBugPictureContainerSelector, "name");

            await _settingService.SaveSettingAsync(productRibbonsSettings);
        }

        private string DeleteProductRibbonsSettingsProps(string props, string pattern)
        {
            var propsArray = props.Split(',');
            var resultProps = "";
            bool flag = false;
            for (int i = 0; i < propsArray.Length; i++)
            {
                propsArray[i] = Regex.Replace(propsArray[i], $@",?\s*\.?{pattern}(\s|,|$)", "", RegexOptions.IgnoreCase);
                if (!string.IsNullOrWhiteSpace(propsArray[i]))
                {
                    if (flag)
                    {
                        resultProps += ",";
                    }
                    flag = true;
                    resultProps += propsArray[i];
                }
            }

            return resultProps;
        }

        #endregion

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>() { PublicWidgetZones.HeaderAfter, PublicWidgetZones.HeaderBefore, PublicWidgetZones.HeaderSelectors });
        }


        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.HeaderBefore)
            {
                return typeof(ShippingCountryPopupViewComponent);
            }
            if (widgetZone == PublicWidgetZones.HeaderAfter)
            {
                return typeof(CartSummaryBarViewComponent);
            }
            if (widgetZone == PublicWidgetZones.HeaderSelectors)
            {
                return typeof(ShippingCountrySelectorViewComponent);
            }
            return null;
        }
        #endregion

        #region InstallLocaleResources

        /// <summary>
        ///Import Resource string from xml and save
        /// </summary>
        protected virtual async Task InstallLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _nopFileProvider.MapPath("~/Plugins/Theme.FiluetHerbalife/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _nopFileProvider.EnumerateFiles(directoryPath, string.Format("language_{0}_pack.xml", language.UniqueSeoCode)))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }
        }

        #endregion

        #region DeleteLocalResourcesAsync

        ///<summry>
        ///Delete Resource String
        ///</summry>
        protected virtual async Task DeleteLocalResourcesAsync()
        {

            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _nopFileProvider.MapPath("~/Plugins/Theme.FiluetHerbalife/Localization/ResourceString");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _nopFileProvider.EnumerateFiles(directoryPath, string.Format("language_{0}_pack.xml", language.UniqueSeoCode)))
                {
                    var languageResourceNames = from name in XDocument.Load(filePath).Document.Descendants("LocaleResource")
                                                select name.Attribute("Name").Value;

                    foreach (var item in languageResourceNames)
                    {
                        await _localizationService.DeleteLocaleResourcesAsync(item);
                    }
                }
            }

        }


        #endregion

        #endregion
    }
}