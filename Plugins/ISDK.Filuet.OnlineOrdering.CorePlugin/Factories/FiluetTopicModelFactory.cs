using Nop.Core;
using Nop.Core.Domain.Topics;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Models.Topics;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    /// <summary>
    /// ExtendedTopicModelFactory
    /// </summary>
    public class FiluetTopicModelFactory : TopicModelFactory, ITopicModelFactory
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public FiluetTopicModelFactory(
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ITopicService topicService,
            ITopicTemplateService topicTemplateService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
            : base(localizationService,
                  storeContext, 
                  topicService,
                  topicTemplateService,
                  urlRecordService)
        {
            _localizationService = localizationService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the topic model
        /// </summary>
        /// <param name="topic">Topic</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic model
        /// </returns>
        public override async Task<TopicModel> PrepareTopicModelAsync(Topic topic)
        {
            if (topic == null)
                throw new ArgumentNullException(nameof(topic));
            var langId = (await _workContext.GetWorkingLanguageAsync()).Id;
            topic.Body =await _localizationService.GetLocalizedAsync(topic, x => x.Body, langId, false, false);

            return new TopicModel
            {
                Id = topic.Id,
                SystemName = topic.SystemName,
                IncludeInSitemap = topic.IncludeInSitemap,
                IsPasswordProtected = topic.IsPasswordProtected,
                Title = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Title),
                Body = topic.IsPasswordProtected ? string.Empty : await _localizationService.GetLocalizedAsync(topic, x => x.Body),
                MetaKeywords = await _localizationService.GetLocalizedAsync(topic, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(topic, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(topic, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(topic),
                TopicTemplateId = topic.TopicTemplateId
            };
        }

        #endregion
    }
}