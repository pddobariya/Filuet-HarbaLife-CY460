using ISDK.Filuet.ExternalSSOAuthPlugin.Components;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISDK.Filuet.ExternalSSOAuthPlugin
{
    public class SSOAuthPlugin : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly INopFileProvider _fileProvider;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public SSOAuthPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            INopFileProvider fileProvider,
            ILanguageService languageService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _fileProvider = fileProvider;
            _languageService = languageService;
        }

        #endregion

        #region Methods

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/SSOAuthAdmin/Configure";
        }

        public Type GetPublicViewComponent()
        {
            return typeof(SSOAuthViewComponent);
        }

        public string GetPublicViewComponentName()
        {
            return SSOAuthHerbalifeDefaults.ViewComponentName;
        }

        public override async Task InstallAsync()
        {
            var settings = new SSOAuthPluginSettings()
            {
                ClientId = "49",
                ClientSecret = "QmFsdGljc09ubGluZU9yZGVyaW5nU29sdXRpb25Qcm9kZ2ZzZGhnaGprc2RnZmprc2RnZmRzZ2suanNkZmd2Ymtqc2Ri",//"QmFsdGljc09ubGluZU9yZGVyaW5nU29sdXRpb25RQWprc2RnZmprc2RnZmRzZ2suanNkZmd2Ymtqc2Ri""QmFsdGljc09ubGluZU9yZGVyaW5nU29sdXRpb25Qcm9kZ2ZzZGhnaGprc2RnZmprc2RnZmRzZ2suanNkZmd2Ymtqc2Ri",
                AuthorizationEndpoint = "https://accounts.myherbalife.com",//https://zus2prs-accounts.myherbalife.com/ https://accounts.myherbalife.com http://accounts.zuswqa4.myherbalife.com
                TokenEndpoint = "https://accounts.myherbalife.com/token",//https://zus2prs-accounts.myherbalife.com/token https://accounts.myherbalife.com/token http://accounts.zuswqa4.myherbalife.com/token
                ApiUrl = "https://www.myherbalife.com/api"//https://zus2prs.myherbalife.com/api https://www.myherbalife.com/api http://www.zuswqa4.myherbalife.com/api
            };
            await _settingService.SaveSettingAsync(settings);

            await InstallLocaleResources();

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            await _settingService.DeleteSettingAsync<SSOAuthPluginSettings>();

            await DeleteLocaleResources();

            await base.UninstallAsync();
        }

        protected virtual async Task InstallLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, string.Format("ResourceString_{0}.xml", language.UniqueSeoCode)))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }
        }
        protected virtual async Task DeleteLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, string.Format("ResourceString_{0}.xml", language.UniqueSeoCode)))
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
    }
}