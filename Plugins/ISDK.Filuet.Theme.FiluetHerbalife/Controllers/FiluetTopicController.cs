using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Controllers
{
    public class FiluetTopicController : TopicController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ITopicModelFactory _topicModelFactory;
        private readonly ITopicService _topicService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public FiluetTopicController(IAclService aclService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            ITopicModelFactory topicModelFactory,
            ITopicService topicService,
            IStoreContext storeContext) : base(aclService, localizationService,
            permissionService, storeMappingService, topicModelFactory, topicService)
        {
            _localizationService = localizationService;
            _permissionService = permissionService;
            _topicModelFactory = topicModelFactory;
            _topicService = topicService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> Faq()
        {
            //allow administrators to preview any topic
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics);

            var topics = (await _topicService
                .GetAllTopicsAsync((await _storeContext.GetCurrentStoreAsync()).Id))
                .Where(t => t.SystemName != null && t.SystemName.ToLower().StartsWith("faq_"))
                .OrderBy(t => t.DisplayOrder);

            var faqList = new FaqListModel
            {
                AllQuestionsActive = true
            };

            foreach (var topic in topics)
            {
                var faqModel = new FaqModel
                {
                    Faq = await _topicModelFactory.PrepareTopicModelAsync(await _topicService.GetTopicByIdAsync(topic.Id))
                };
                faqList.FaqModels.Add(faqModel);
            }

            return View("Faq", faqList);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> FaqDetails(int topicId)
        {
            //allow administrators to preview any topic
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTopics);

            var model = await _topicModelFactory.PrepareTopicModelAsync(await _topicService.GetTopicByIdAsync(topicId));
            if (model == null)
                return InvokeHttp404();

            if (!model.SystemName.ToLower().StartsWith("faq_"))
                return RedirectToAction("TopicDetails", topicId);

            var topics = (await _topicService
                    .GetAllTopicsAsync((await _storeContext.GetCurrentStoreAsync()).Id))
                    .Where(t => t.SystemName != null && t.SystemName.ToLower().StartsWith("faq_"))
                    .OrderBy(t => t.DisplayOrder);

            var faqList = new FaqListModel();
            foreach (var topic in topics)
            {
                var topicModel = new FaqModel();
                if (topic.Id != topicId)
                {
                    topicModel.Faq.Id = topic.Id;
                    topicModel.Faq.Title = await _localizationService.GetLocalizedAsync(topic, x => x.Title);
                }
                else
                {
                    topicModel.Faq = model;
                    topicModel.IsActive = true;
                }
                faqList.FaqModels.Add(topicModel);
            }

            //display "edit" (manage) link
            if (hasAdminAccess)
                DisplayEditLink(Url.Action("Edit", "Topic", new { id = model.Id, area = AreaNames.Admin }));

            return View("FaqDetails", faqList);
        }

        #endregion
    }
}
