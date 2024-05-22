using Nop.Core.Domain.Directory;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Directory;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ
{
    public abstract class BaseFaq
    {
        #region Fields

        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        protected BaseFaq(
            ILocalizedModelFactory localizedModelFactory,
            ILanguageService languageService)
        {
            _localizedModelFactory = localizedModelFactory;
            _languageService = languageService;
        }

        #endregion

        #region Methods

        protected async Task<TopicModel> FaqModelFactory(string topicName, string systemName, int displayOrder, string Body)
        {
            var model = new TopicModel
            {
                Title = topicName,
                SystemName = systemName,
                DisplayOrder = displayOrder,
                Published = true,
                TopicTemplateId = 1,
                Body = Body
            };

            Func<TopicLocalizedModel, int,Task> localizedModelConfiguration = async (locale, languageId) =>
            {
                await GetLocales(locale, languageId);
            };

            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

            return model;
        }

        protected virtual async  Task<TopicLocalizedModel> GetLocales(TopicLocalizedModel locale, int languageId)
        {
            var lang = await _languageService.GetLanguageByIdAsync(languageId);
            var langName = lang.Name ?? lang.UniqueSeoCode;
            switch (langName.ToUpper())
            {
                case "RU":
                    GetRuLocales(locale);
                    break;

                case "LT":
                    GetLTLocales(locale);
                    break;

                case "LV":
                    GetLVLocales(locale);
                    break;
                
                case "EE":
                    GetEELocales(locale);
                    break;
                default:
                    GetEnLocales(locale);
                    break;
            }

            return locale;
        }

        protected virtual string GetQuestionAndAnswer(int number, string questions, string answer, string addInfo = "")
        {
            return String.Format("<div class=\"accordion_item\">" +
                "<div class=\"head\">" +
                    "<div class=\"title\">" +
                        $"<span class=\"number\">{number}.</span>" +
                        $"<span>{questions}</span>" +
                    "</div>" +
                "</div>" +
                "<div class=\"data text_block\">" +
                    $"<p>{answer}</p>" +
                    $"{addInfo}" +
                "</div>" +
                "</div>");
        }

        protected abstract void GetEnLocales(TopicLocalizedModel locale);
        protected abstract void GetRuLocales(TopicLocalizedModel locale);
        protected abstract void GetLTLocales(TopicLocalizedModel locale);
        protected abstract void GetLVLocales(TopicLocalizedModel locale);
        protected abstract void GetEELocales(TopicLocalizedModel locale);

        #endregion
    }
}
