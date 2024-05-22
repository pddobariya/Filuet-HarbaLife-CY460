using ISDK.Filuet.Theme.FiluetHerbalife.Helpers.FAQ;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Topics;
using Nop.Web.Framework.Factories;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Helpers
{
    public class FaqDataInitHelper
    {
        #region Fields

        private readonly IUrlRecordService _urlRecordService;
        private readonly ITopicService _topicService;
        private readonly ILogger _logger;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly FaqGeneralQuestions _faqGeneralQuestions;
        private readonly FaqProducts _faqProducts;
        private readonly FaqEventTickets _faqEventTickets;
        private readonly FaqOrderProcessing _faqOrderProcessing;
        private readonly FaqCompanyAccount _faqCompanyAccount;
        
        #endregion

        #region Ctor

        public FaqDataInitHelper(IUrlRecordService urlRecordService, 
            ITopicService topicService, 
            ILogger logger, 
            ILocalizedEntityService localizedEntityService,
            ILocalizedModelFactory localizedModelFactory,
            ILanguageService languageService)
        {
            _urlRecordService = urlRecordService;
            _topicService = topicService;
            _logger = logger;
            _localizedEntityService = localizedEntityService;
            _faqGeneralQuestions = new FaqGeneralQuestions(localizedModelFactory, languageService);
            _faqProducts = new FaqProducts(localizedModelFactory, languageService);
            _faqEventTickets = new FaqEventTickets(localizedModelFactory, languageService);
            _faqOrderProcessing = new FaqOrderProcessing(localizedModelFactory, languageService);
            _faqCompanyAccount = new FaqCompanyAccount(localizedModelFactory, languageService);
        }

        #endregion

        #region Methods

        public async Task InitTopics()
        {
            try
            {
                await CreateTopic(await _faqGeneralQuestions.Get());
                await CreateTopic(await _faqProducts.Get());
                await CreateTopic(await _faqEventTickets.Get());
                await CreateTopic(await _faqOrderProcessing.Get());
                await CreateTopic(await _faqCompanyAccount.Get());
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"[FaqDataInitHelper] Error: {ex}");
            }
        }

        #region CreateTopic

        private async Task CreateTopic(TopicModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var existTopic = await _topicService.GetTopicBySystemNameAsync(model.SystemName);
            if (existTopic != null)
            {
                await _logger.InformationAsync($"[FaqDataInitHelper] topic '{existTopic.SystemName}' already exist.");
                return;
            }

            var topic = model.ToEntity<Topic>();
            await _topicService.InsertTopicAsync(topic);

            //search engine name
            string seName = await _urlRecordService.ValidateSeNameAsync(topic, null, topic.Title ?? topic.SystemName, true);
            await _urlRecordService.SaveSlugAsync(topic, seName, 0);

            //locales
            await UpdateLocalesAsync(topic, model);

            await _logger.InformationAsync($"[FaqDataInitHelper] topic '{topic.SystemName}' successfully created.");
        }

        #endregion

        #region Utilities

        private async Task UpdateLocalesAsync(Topic topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.Title,
                    localized.Title,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.Body,
                    localized.Body,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaKeywords,
                    localized.MetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaDescription,
                    localized.MetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(topic,
                    x => x.MetaTitle,
                    localized.MetaTitle,
                    localized.LanguageId);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(topic, localized.SeName, localized.Title, false);
                await _urlRecordService.SaveSlugAsync(topic, seName, localized.LanguageId);
            }
        }

        #endregion

        #endregion
    }
}
