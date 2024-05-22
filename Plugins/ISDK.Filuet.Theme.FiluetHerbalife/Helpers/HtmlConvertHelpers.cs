using AngleSharp;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Topics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers
{
    public class HtmlConvertHelpers
    {
        #region Fields

        private readonly int categoryIdForProgramm;

        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly ICategoryService _categoryService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ITopicService _topicService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public HtmlConvertHelpers(
            ILogger logger,
            IStoreContext storeContext,
            ICategoryService categoryService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IUrlRecordService urlRecordService,
            ITopicService topicService,
            IProductService productService)
        {
            categoryIdForProgramm = FiluetThemePluginDefaults.CategoryIdForProgramm;
            _logger = logger;
            _storeContext = storeContext;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _urlRecordService = urlRecordService;
            _topicService = topicService;
            _productService = productService;
        }

        #endregion

        #region ConvertProgramsAsync

        public async Task ConvertProgramsAsync()
        {
            var programCategories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(categoryIdForProgramm);
            var languages = await _languageService.GetAllLanguagesAsync();
            var context = BrowsingContext.New(Configuration.Default);
            var specCategories = new int[] { 15, 58 };

            foreach (var programCategory in programCategories)
            {
                foreach (var language in languages)
                {
                    var description = await _localizationService.GetLocalizedAsync(programCategory, x => x.Description, languageId: language.Id);

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        try
                        {
                            if (description.Contains("Slogan") && description.Contains("ExpertReview"))
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] " +
                                    $"CategoryId={programCategory.Id}, languageId={language.Id}. Missed. Has been updated.");
                                continue;
                            }

                            var document = await context.OpenAsync(req => req.Content(description));

                            var tableRows = document.QuerySelectorAll("table tr");
                            if (!tableRows.Any())
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] " +
                                    $"CategoryId={programCategory.Id}, languageId={language.Id}. Missed. No table rows.");
                                continue;
                            }

                            if (tableRows.Length < 6)
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] " +
                                    $"CategoryId={programCategory.Id}, languageId={language.Id}. Missed. Less rows.");
                                continue;
                            }

                            var newProgramModel = new ProgramCategoryModel();
                            var startIndex = 0;
                            if (specCategories.Contains(programCategory.Id) || tableRows.Length < 8)
                                startIndex = -1;

                            var tableRow = tableRows[startIndex + 4];
                            var spanElements = tableRow.QuerySelectorAll("span");

                            if (spanElements.Length > 1)
                            {
                                newProgramModel.Slogan = spanElements[0].TextContent?.Trim();

                                newProgramModel.Description = spanElements[1].TextContent?.Trim();
                            }
                            else
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] " +
                                    $"CategoryId={programCategory.Id}, languageId={language.Id}. No SLOGAN and DESCRIPTION found.");
                            }


                            var aElements = tableRows[startIndex + 2].QuerySelectorAll("a");
                            if (aElements.Length > 0)
                            {
                                newProgramModel.DownloadLink = aElements[0].GetAttribute("href")?.Trim();
                            }
                            else
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] " +
                                    $"CategoryId={programCategory.Id}, languageId={language.Id}. No DOWNLOAD_LINK found.");
                            }


                            newProgramModel.ExpertPhotoUrl = $"/images/uploaded/FiluetHerbalifeTheme/ProductPrograms/Experts/expert_photo_for_category_{programCategory.Id}.png";


                            spanElements = tableRows[startIndex + 5].QuerySelectorAll("span");
                            if (spanElements.Length > 2)
                            {
                                newProgramModel.ExpertReview = spanElements[2].TextContent?.Trim();
                            }
                            else
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] " +
                                    $"CategoryId={programCategory.Id}, languageId={language.Id}. No EXPERT_QUOTE found.");
                            }

                            var jsonProgramModel = JsonConvert.SerializeObject(newProgramModel);
                            await _localizedEntityService.SaveLocalizedValueAsync(programCategory, x => x.Description, jsonProgramModel, language.Id);

                            await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] CategoryId={programCategory.Id}, languageId={language.Id}. Description updated.");
                        }
                        catch (Exception exc)
                        {
                            await _logger.ErrorAsync($"[HtmlConvertHelpers.ConvertProgramsAsync] CategoryId={programCategory.Id}, languageId={language.Id}", exc);
                        }
                    }
                }
            }
        }

        #endregion

        #region ConvertProgramTypesAsync

        public async Task ConvertProgramTypesAsync()
        {
            var programCategories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(categoryIdForProgramm);
            var languages = await _languageService.GetAllLanguagesAsync();

            foreach (var programCategory in programCategories)
            {
                var programSubCategories = await _categoryService.GetAllCategoriesByParentCategoryIdAsync(programCategory.Id);

                Dictionary<string, string> types = new Dictionary<string, string>();
                Dictionary<string, string> refTypes = new Dictionary<string, string>();

                foreach (var programSubCategory in programSubCategories)
                {
                    foreach (var language in languages)
                    {
                        var key = programSubCategory.Name + "_" + language.Id;
                        var value = await _localizationService.GetLocalizedAsync(programSubCategory, x => x.Name, languageId: language.Id);
                        types.Add(key, value);

                        var refKey = programSubCategory.Name + "_" + language.Id;
                        var seName = await _urlRecordService.GetSeNameAsync(programSubCategory);
                        var refValue = $"/{seName}";

                        refTypes.Add(refKey, refValue);
                    }
                }

                foreach (var programSubCategory in programSubCategories)
                {
                    string template = programSubCategory.Name switch
                    {
                        "Basic" => NewProgramType1Html,
                        "Extended" => NewProgramType2Html,
                        "Advanced" => NewProgramType3Html,
                        _ => null
                    };

                    if (string.IsNullOrEmpty(template))
                    {
                        continue;
                    }

                    string benefitRowHtml = BenefitRowHtml;

                    foreach (var language in languages)
                    {
                        StringBuilder newProgramHtml = new StringBuilder(template);

                        newProgramHtml.Replace("{{Basic}}", types["Basic_" + language.Id]);
                        newProgramHtml.Replace("{{Extended}}", types["Extended_" + language.Id]);
                        newProgramHtml.Replace("{{Advanced}}", types["Advanced_" + language.Id]);

                        newProgramHtml.Replace("{{BasicRef}}", refTypes["Basic_" + language.Id]);
                        newProgramHtml.Replace("{{ExtendedRef}}", refTypes["Extended_" + language.Id]);
                        newProgramHtml.Replace("{{AdvancedRef}}", refTypes["Advanced_" + language.Id]);

                        var name = await _localizationService.GetLocalizedAsync(programSubCategory, x => x.Name, languageId: language.Id);
                        newProgramHtml.Replace("{{PROGRAMMTYPENAME}}", name);

                        var description = await _localizationService.GetLocalizedAsync(programSubCategory, x => x.Description, languageId: language.Id);

                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            try
                            {
                                description = description.Replace("\n", "").Trim('\n');

                                var config = Configuration.Default;
                                var context = BrowsingContext.New(config);
                                var document = await context.OpenAsync(req => req.Content(description));

                                var tableRows = document.QuerySelectorAll("table tr");
                                if (!tableRows.Any())
                                {
                                    continue;
                                }

                                StringBuilder benefitRowsHtml = new StringBuilder();

                                StringBuilder footNotesBlock = new StringBuilder();

                                int rowNumber = 0;
                                bool hasTitle = false;
                                foreach (var tableRow in tableRows)
                                {
                                    rowNumber++;

                                    if (rowNumber == 1)
                                    {
                                        var spanElement = tableRow.QuerySelectorAll("span").FirstOrDefault();
                                        if (spanElement != null)
                                        {
                                            newProgramHtml.Replace("{{PROGRAMMTYPETITLE}}", spanElement.TextContent);
                                        }
                                    }
                                    else if (rowNumber == 2)
                                    {
                                        var imgElement = tableRow.QuerySelectorAll("img").FirstOrDefault();
                                        if (imgElement != null)
                                        {
                                            var imageSrc = imgElement.GetAttribute("src");
                                            newProgramHtml.Replace("{{IMAGESRC}}", imageSrc);
                                        }
                                    }
                                    else if (rowNumber == 3)
                                    {
                                        var spanElement = tableRow.QuerySelectorAll("span").FirstOrDefault();
                                        if (spanElement != null)
                                        {
                                            newProgramHtml.Replace("{{SLOGAN}}", spanElement.TextContent);
                                        }
                                    }
                                    else
                                    {
                                        //если четное, то это заголовок
                                        if ((rowNumber % 2) == 0)
                                        {
                                            var clearText = tableRow.TextContent.Replace("\n", "").Trim('\n');
                                            if (clearText.Count() < 2)
                                            {
                                                continue;
                                            }

                                            string title = tableRow.TextContent.Replace("\n", "").Trim('\n');
                                            title = title.Substring(title.IndexOf('.') + 1);
                                            if (!string.IsNullOrWhiteSpace(title) && !title.Trim().Equals("."))
                                            {
                                                var newBenefitRowHtml = benefitRowHtml.Replace("{{BENEFITROWTITLE}}", $"'{title.Trim()}'");
                                                newBenefitRowHtml = newBenefitRowHtml.Replace("{{BENEFITUID}}", $"'{Guid.NewGuid()}'");
                                                benefitRowsHtml.Append(newBenefitRowHtml);
                                            }
                                            hasTitle = true;
                                        }
                                        else
                                        {
                                            var clearText = tableRow.TextContent.Replace("\n", "").Trim('\n');
                                            if (clearText.Count() < 2)
                                            {
                                                continue;
                                            }

                                            var fullRow = tableRow.TextContent.Replace("\n", "").Trim('\n');
                                            var spanElement = tableRow.QuerySelectorAll("span").FirstOrDefault();
                                            if (spanElement != null)
                                            {
                                                var strongElement = spanElement.QuerySelectorAll("strong").FirstOrDefault();
                                                if (strongElement != null)
                                                {
                                                    string strongPart = strongElement.TextContent.Replace("\n", "").Trim('\n');

                                                    fullRow = fullRow.Replace(strongPart, $"<span>{strongPart}</span>");
                                                    benefitRowsHtml.Replace("{{BENEFITROW}}", $"'{fullRow}'");
                                                    hasTitle = false;
                                                }
                                                else
                                                {
                                                    if (hasTitle)
                                                    {
                                                        benefitRowsHtml.Replace("{{BENEFITROW}}", $"'{fullRow}'");
                                                        hasTitle = false;
                                                    }
                                                    else
                                                    {
                                                        //если есть дивы, то это нижний блок со сносками
                                                        var divElements = tableRow.QuerySelectorAll("div");
                                                        if (divElements.Any())
                                                        {
                                                            foreach (var divElement in divElements)
                                                            {
                                                                var divHtml = FootNoteHtml.Replace("\n", "")?.Trim('\n');
                                                                footNotesBlock.Append(divHtml.Replace("{{FOOTNOTE}}", divElement.TextContent.Replace("\n", "")?.Trim('\n')));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                var lastRow = tableRows[^1];
                                var linkElement = lastRow.QuerySelectorAll("a").FirstOrDefault();
                                if (linkElement != null)
                                {
                                    var downloadLink = linkElement.GetAttribute("href");
                                    newProgramHtml.Replace("{{DOWNLOAD_LINK}}", downloadLink);
                                }

                                newProgramHtml.Replace("{{BENEFITROWS}}", "ProgramBenefits : [ " + benefitRowsHtml.ToString() + "]");

                                newProgramHtml.Replace("{{FOOTNOTES}}", footNotesBlock.ToString());

                                var program = JsonConvert.DeserializeObject<ProgramSubCategoryModel>(newProgramHtml.ToString());
                                var json = JsonConvert.SerializeObject(program);

                                await _localizedEntityService.SaveLocalizedValueAsync(programSubCategory, x => x.Description, json, language.Id);
                            }
                            catch (Exception exc)
                            {
                                await _logger.ErrorAsync($"[HtmlConvertHelpers.ConvertProgramTypesAsync] Error. CategoryId={programSubCategory.Id}, languageId={language.Id}", exc);
                            }
                        }
                    }
                }
            }
        }

        private static string FootNoteHtml => @"
		<div>{{FOOTNOTE}}</div>
        ";

        private static string BenefitRowHtml => @"{ Name: {{BENEFITROWTITLE}}, Description : {{BENEFITROW}}, Uid: {{BENEFITUID}} }, ";

        //базовая
        private static string NewProgramType1Html => @"{
            Name: '{{PROGRAMMTYPENAME}}',
            Title: '{{PROGRAMMTYPETITLE}}',
            ImageSrc: '{{IMAGESRC}}',
            Slogan: '{{SLOGAN}}',
            Description: '',
            DownloadLink: '{{DOWNLOAD_LINK}}',
            ProgramTypeId: '1',
            ProgramTypeName: '{{Basic}}',
            SecondProgramTypeId: '2',
            SecondProgramTypeName: '{{Extended}}',
            SecondProgramTypeRef: '{{ExtendedRef}}',
            ThirdProgramTypeId: '3',
            ThirdProgramTypeName: '{{Advanced}}',
            ThirdProgramTypeRef: '{{AdvancedRef}}',
            Footnote: '{{FOOTNOTES}}',
            {{BENEFITROWS}}
        }";

        //расширенная
        private static string NewProgramType2Html => @"{
            Name: '{{PROGRAMMTYPENAME}}',
            Title: '{{PROGRAMMTYPETITLE}}',
            ImageSrc: '{{IMAGESRC}}',
            Slogan: '{{SLOGAN}}',
            Description: '',
            DownloadLink: '{{DOWNLOAD_LINK}}',
            ProgramTypeId: '2',
            ProgramTypeName: '{{Extended}}',
            SecondProgramTypeId: '1',
            SecondProgramTypeName: '{{Basic}}',
            SecondProgramTypeRef: '{{BasicRef}}',
            ThirdProgramTypeId: '3',
            ThirdProgramTypeName: '{{Advanced}}',
            ThirdProgramTypeRef: '{{AdvancedRef}}',
            Footnote: '{{FOOTNOTES}}',
            {{BENEFITROWS}}
        }";

        //продвинутая
        private static string NewProgramType3Html => @"{
            Name: '{{PROGRAMMTYPENAME}}',
            Title: '{{PROGRAMMTYPETITLE}}',
            ImageSrc: '{{IMAGESRC}}',
            Slogan: '{{SLOGAN}}',
            Description: '',
            DownloadLink: '{{DOWNLOAD_LINK}}',
            ProgramTypeId: '3',
            ProgramTypeName: '{{Advanced}}',
            SecondProgramTypeId: '1',
            SecondProgramTypeName: '{{Basic}}',
            SecondProgramTypeRef: '{{BasicRef}}',
            ThirdProgramTypeId: '2',
            ThirdProgramTypeName: '{{Extended}}',
            ThirdProgramTypeRef: '{{ExtendedRef}}',
            Footnote: '{{FOOTNOTES}}',
            {{BENEFITROWS}}
        }";

        #endregion

        #region ConvertTopicsAsync

        public async Task ConvertTopicsAsync()
        {

            var topics = await (await _topicService.GetAllTopicsAsync((await _storeContext.GetCurrentStoreAsync()).Id, onlyIncludedInTopMenu: true)).ToListAsync();

            var languages = await _languageService.GetAllLanguagesAsync();

            foreach (var topic in topics)
            {
                foreach (var language in languages)
                {
                    string template = GetTopicTemplate;

                    StringBuilder newTopicHtml = new StringBuilder(template);

                    StringBuilder rowsBlock = new StringBuilder();

                    var resultStr = await _localizationService.GetLocalizedAsync(topic, x => x.Body, languageId: language.Id);

                    if (!string.IsNullOrWhiteSpace(resultStr))
                    {
                        resultStr = resultStr.Replace("&nbsp;", "").Replace("\n", "").Trim('\n').Replace("\r", "").Trim('\r');

                        var config = Configuration.Default;
                        var context = BrowsingContext.New(config);
                        var document = await context.OpenAsync(req => req.Content(resultStr));

                        var hasBeenUpdated = document.QuerySelectorAll(".row").Any();
                        if (hasBeenUpdated)
                        {
                            continue;
                        }

                        var tableRows = document.QuerySelectorAll("table tr");
                        if (!tableRows.Any())
                        {
                            continue;
                        }

                        int rowCount = tableRows.Length;
                        int rowIndex = 0;
                        while (true)
                        {
                            if (rowCount <= rowIndex + 1)
                            {
                                break;
                            }

                            bool isTitleRow = false;

                            var titleRow = tableRows[rowIndex];
                            var linksRow = tableRows[rowIndex + 1];

                            var titleTds = titleRow.QuerySelectorAll("td");
                            var linkTds = linksRow.QuerySelectorAll("td");

                            for (int i = 0; i < titleTds.Length; i++)
                            {
                                var titleTd = titleTds[i];
                                var text = titleTd.TextContent;
                                if (text.Length > 1)
                                {
                                    isTitleRow = true;

                                    var rowHtml = GetRowTemplate;

                                    rowHtml = rowHtml.Replace("{{TITLE}}", text);

                                    var linkTd = linkTds[i];

                                    var linkElement = linkTd.QuerySelectorAll("a").FirstOrDefault();
                                    if (linkElement != null)
                                    {
                                        var link = linkElement.GetAttribute("href");
                                        rowHtml = rowHtml.Replace("{{LINK}}", link);
                                    }

                                    var imgElement = linkTd.QuerySelectorAll("img").FirstOrDefault();
                                    if (imgElement != null)
                                    {
                                        var src = imgElement.GetAttribute("src");
                                        rowHtml = rowHtml.Replace("{{IMGSRC}}", src);
                                    }

                                    rowsBlock.Append(rowHtml);
                                }
                            }

                            if (isTitleRow)
                            {
                                rowIndex = rowIndex + 2;
                            }
                            else
                            {
                                rowIndex++;
                            }
                        }
                    }

                    newTopicHtml = newTopicHtml.Replace("{{ROWS}}", rowsBlock.ToString());

                    await _localizedEntityService.SaveLocalizedValueAsync(topic, x => x.Body, newTopicHtml.ToString(), language.Id);
                }
            }
        }

        private string GetTopicTemplate => "<div class='row'>{{ROWS}}</div>";

        private string GetRowTemplate => "<a href='{{LINK}}' class='item'><div class='thumb'><img data-src='{{IMGSRC}}' alt='' class='lozad loaded' src='{{IMGSRC}}' data-loaded='true'></div><div class='name'>{{TITLE}}</div></a>";

        #endregion

        #region ConvertProductDescriptionAsync

        public async Task ConvertProductDescriptionAsync()
        {
            var products = await _productService.SearchProductsAsync(showHidden: true);

            var languages = await _languageService.GetAllLanguagesAsync();

            foreach (var product in products)
            {
                if (!string.IsNullOrWhiteSpace(product.FullDescription))
                {
                    try
                    {
                        var description = product.FullDescription.Replace("&nbsp;", "").Replace("\n", "").Trim('\n').Replace("\r", "").Trim('\r');

                        if (!HasBeenUpdated(description))
                        {
                            var productDescription = await GetProductDescription(description);
                            product.FullDescription = JsonConvert.SerializeObject(productDescription);

                            await _productService.UpdateProductAsync(product);
                        }
                    }
                    catch (Exception exc)
                    {
                        await _logger.ErrorAsync($"[HtmlConvertHelpers.ConvertProductDescriptionAsync] Error. ProductId={product.Id}", exc);
                    }
                }

                foreach (var language in languages)
                {
                    var description = await _localizationService.GetLocalizedAsync(product, x => x.FullDescription, languageId: language.Id);

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        try
                        {
                            description = description.Replace("&nbsp;", "").Replace("\n", "").Trim('\n').Replace("\r", "").Trim('\r');

                            if (HasBeenUpdated(description))
                            {
                                await _logger.InformationAsync($"[HtmlConvertHelpers.ConvertProductDescriptionAsync] " +
                                    $"CategoryId={product.Id}, languageId={language.Id}. Missed. Has been updated.");
                                continue;
                            }

                            var localizedProductDescription = await GetProductDescription(description);

                            var jsonProductDescription = JsonConvert.SerializeObject(localizedProductDescription);
                            await _localizedEntityService.SaveLocalizedValueAsync(product, x => x.FullDescription, jsonProductDescription, language.Id);
                        }
                        catch (Exception exc)
                        {

                            await _logger.ErrorAsync($"[HtmlConvertHelpers.ConvertProductDescriptionAsync] Error. ProductId={product.Id}, languageId={language.Id}", exc);
                        }
                    }
                }
            }
        }

        private bool HasBeenUpdated(string description)
        {
            return description.Contains("\"Description\":") && description.Contains("\"PdfFiles\":");
        }

        private async Task<ProductDescription> GetProductDescription(string description)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(description));

            var spanElements = document.QuerySelectorAll("p span");

            foreach (var spanElement in spanElements)
            {
                spanElement.RemoveAttribute("style");
            }

            var templateSplit = document.Body.InnerHtml.Split("<table");

            return new ProductDescription { Description = templateSplit[0]?.Trim() };
        }

        #endregion
    }
}
