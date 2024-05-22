using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter
{
    public class PriceFilterPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INopFileProvider _fileProvider;
        private readonly ILanguageService _languageService;
        private readonly INopFileProvider _nopFileProvider;

        #endregion

        #region Ctor

        public PriceFilterPlugin(IWebHelper webHelper,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            ILanguageService languageService,
            INopFileProvider nopFileProvider)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _fileProvider = fileProvider;
            _languageService = languageService;
            _nopFileProvider = nopFileProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Install LocaleResources
        /// </summary>B
        protected virtual async Task InstallLocaleResourcesAsync()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _nopFileProvider.MapPath("~/Plugins/Catalogue.PriceFilter/Localization/ResourceString");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _nopFileProvider.EnumerateFiles(directoryPath, string.Format("*.xml", language.UniqueSeoCode)))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }
        }

        /// <summary>
        /// Remove LocaleResources
        /// </summary>
        protected virtual async Task UninstallLocalResourcesAsync()
        {
            var file = Path.Combine(_fileProvider.MapPath("~/Plugins/Catalogue.PriceFilter/Localization/ResourceString"), "ResourceString.xml");
            var languageResourceNames = from name in XDocument.Load(file).Document?.Descendants("LocaleResource")
                                        select name.Attribute("Name")?.Value;

            foreach (var item in languageResourceNames)
            {
                await _localizationService.DeleteLocaleResourceAsync(item);
            }

            var file1 = Path.Combine(_fileProvider.MapPath("~/Plugins/Catalogue.PriceFilter/Localization/ResourceString"), "HrvatskiResourceString.xml");
            var languageResourceNames1 = from name in XDocument.Load(file).Document?.Descendants("LocaleResource")
                                         select name.Attribute("Name")?.Value;

            foreach (var item in languageResourceNames1)
            {
                await _localizationService.DeleteLocaleResourceAsync(item);
            }
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/PriceFilter/Configure";
        }

        public override async Task InstallAsync()
        {
            await InstallLocaleResourcesAsync();
            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await UninstallLocalResourcesAsync();
            await base.InstallAsync();
        }

        #endregion
    }
}
