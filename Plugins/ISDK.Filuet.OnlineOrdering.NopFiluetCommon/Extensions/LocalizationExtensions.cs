using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class LocalizationExtensions
    {
        #region Methods

        public async static Task<string> ToLocalizedStringAsync(this string resourceKey, string lang)
        {
            if (string.IsNullOrWhiteSpace(lang))
            {
                return resourceKey;
            }

            int langId = await lang.Iso6391ToNopLanguageIdAsync();
            return await resourceKey.ToLocalizedStringAsync(langId);
        }

        public async static Task<string> ToLocalizedStringAsync(this string resourceKey, int langId = -1)
        {
            ILocalizationService localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            Task<string> localized = null;
            try
            {
                localized = langId == -1 ?  localizationService.GetResourceAsync(resourceKey) :  localizationService.GetResourceAsync(resourceKey, langId);
            }
            catch (Exception)
            {
                localized =  localizationService.GetResourceAsync(resourceKey, await "en".Iso6391ToNopLanguageIdAsync());
            }

            if (string.IsNullOrWhiteSpace(await localized))
            {
                return resourceKey;
            }

            return await localized;
        }

        /// <summary>
        /// ISO 639-1 to lang id
        /// </summary>
        public async static Task<int> Iso6391ToNopLanguageIdAsync(this string iso6391)
        {
            try
            {
                int langId = 0;
                ILanguageService languageService = EngineContext.Current.Resolve<ILanguageService>();
                langId = (await languageService.GetAllLanguagesAsync()).Where(x => x.LanguageCulture == iso6391.Iso6391ToCulture()).FirstOrDefault().Id;
                return langId;
            }
            catch (Exception)
            {
                return await "en".Iso6391ToNopLanguageIdAsync();
            }
        }

        /// <summary>
        /// ISO 639-1 to culture
        /// </summary>
        public static string Iso6391ToCulture(this string iso6391)
        {
            string cult = "en-US";
            if (iso6391 != null)
            {
                iso6391 = iso6391.ToLower().Trim();
                switch (iso6391)
                {
                    case "ru":
                        cult = "ru-RU";
                        break;
                    case "lt":
                        cult = "lt-LT";
                        break;
                    case "lv":
                        cult = "lv-LV";
                        break;
                    case "et":
                        cult = "et-EE";
                        break;
                }
            }

            return cult;
        }

        /// <summary>
        /// ISO culture to 639-1
        /// </summary>
        public static string CultureToIso6391(this string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
            {
                return "en";
            }

            if (!culture.Contains("-") && culture.Length == 2)
            {
                return culture.ToLower();
            }

            if (!culture.Contains("-"))
            {
                return "en";
            }

            return culture.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
        }

        public static void ImportPluginResourcesFromXmlAsync(this ILocalizationService localizationService, string localizationResourcesFolderPath, bool updateExistingResources = true)
        {
            INopFileProvider fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            IRepository<Language> languageRepository = EngineContext.Current.Resolve<IRepository<Language>>();
            ILogger logger = EngineContext.Current.Resolve<ILogger>();

            var directoryPath = fileProvider.MapPath(localizationResourcesFolderPath);
            var pattern = $"*.xml";
            foreach (var filePath in fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                var localesXml = fileProvider.ReadAllText(filePath, Encoding.UTF8);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(localesXml);
                string langName = xmlDoc.DocumentElement.Attributes["Name"].Value;

                Language language = languageRepository.Table
                    .FirstOrDefault(l => l.Name.ToLowerInvariant() == langName.ToLowerInvariant()
                                        || l.UniqueSeoCode.ToLowerInvariant() == langName.ToLowerInvariant());
                if (language == null)
                {
                    logger.InsertLogAsync(LogLevel.Error, $"Error: '{langName}' language not found.");
                }
                else
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(localesXml);
                    MemoryStream stream = new MemoryStream(byteArray);
                    localizationService.ImportResourcesFromXmlAsync(language, new StreamReader(stream), updateExistingResources);
                }
            }
        }

        public static void DeletePluginAllResourcesFromXml(this ILocalizationService localizationService, string localizationResourcesFolderPath)
        {
            INopFileProvider fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            var directoryPath = fileProvider.MapPath(localizationResourcesFolderPath);
            var pattern = $"*.xml";
            foreach (var filePath in fileProvider.EnumerateFiles(directoryPath, pattern))
            {
                var localesXml = fileProvider.ReadAllText(filePath, Encoding.UTF8);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(localesXml);
                XmlNodeList xmlNodeList = xmlDoc.SelectNodes("/Language/LocaleResource");

                foreach (XmlNode node in xmlNodeList)
                {
                    localizationService.DeleteLocaleResourceAsync(node.Attributes["Name"].Value);
                }
            }
        }

        #endregion
    }
}
