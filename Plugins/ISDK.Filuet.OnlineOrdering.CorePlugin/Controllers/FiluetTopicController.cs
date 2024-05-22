using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    [NameControllerModelConvention("Topic")]
    public partial class FiluetTopicController : TopicController
    {
        #region Ctor

        public FiluetTopicController(
            IAclService aclService,
            ILocalizationService localizationService,
            IPermissionService permissionService, 
            IStoreMappingService storeMappingService, 
            ITopicModelFactory topicModelFactory, 
            ITopicService topicService) 
            : base(aclService,
                  localizationService,
                  permissionService,
                  storeMappingService,
                  topicModelFactory,
                  topicService)
        {
        }

        #endregion

        #region Methods

        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public override async Task<IActionResult> TopicDetails(int topicId)
        {
            return await base.TopicDetails(topicId);
        }

        [CheckAccessClosedStore(true)]
        [CheckAccessPublicStore(true)]
        public override async Task<IActionResult> TopicDetailsPopup(string systemName)
        {
            return await base.TopicDetailsPopup(systemName);
        }

        #endregion
    }
}
