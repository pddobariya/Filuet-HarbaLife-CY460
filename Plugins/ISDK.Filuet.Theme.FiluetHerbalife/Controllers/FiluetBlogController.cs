using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using ISDK.Filuet.Theme.FiluetHerbalife.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Security;
using Nop.Core.Events;
using Nop.Services.Blogs;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Blogs;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Controllers
{
    [NameControllerModelConvention("Blog")]
    public class FiluetBlogController : BlogController
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly IBlogService _blogService;
        private readonly INewsBlockModelFactory _newsBlockModelFactory;

        #endregion

        #region Ctor

        public FiluetBlogController(
            BlogSettings blogSettings,
            INewsBlockModelFactory newsBlockModelFactory,
            CaptchaSettings captchaSettings, 
            IBlogModelFactory blogModelFactory, 
            IBlogService blogService,
            ICustomerActivityService customerActivityService, 
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService, 
            INopUrlHelper nopUrlHelper,
            IPermissionService permissionService, 
            IStoreContext storeContext,
            IStoreMappingService storeMappingService, 
            IUrlRecordService urlRecordService,
            IWebHelper webHelper, 
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings) : base(blogSettings, captchaSettings, blogModelFactory, blogService, customerActivityService, customerService, eventPublisher, localizationService, nopUrlHelper, permissionService, storeContext, storeMappingService, urlRecordService, webHelper, workContext, workflowMessageService, localizationSettings)
        {
            _blogSettings = blogSettings;
            _blogService = blogService;
            _newsBlockModelFactory = newsBlockModelFactory;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> List(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await _newsBlockModelFactory.PrepareBlogPostListModelAsync(command);
            return View("List", model);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<IActionResult> BlogByTag(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await _newsBlockModelFactory.PrepareBlogPostListModelAsync(command);
            return View("List", model);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<IActionResult> BlogByMonth(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
                return RedirectToRoute("Homepage");

            var model = await _newsBlockModelFactory.PrepareBlogPostListModelAsync(command);
            return View("List", model);
        }

        public override async Task<IActionResult> BlogPost(int blogPostId)
        {
            IActionResult baseResult = await base.BlogPost(blogPostId);
            var baseModel = (BlogPostModel)((ViewResult)baseResult).Model;

            string baseSerialized = JsonConvert.SerializeObject(baseModel);
            var model = JsonConvert.DeserializeObject<FiluetBlogPostModel>(baseSerialized);

            var blogPost = await _blogService.GetBlogPostByIdAsync(blogPostId);
            await _newsBlockModelFactory.PrepareBlogPostModelAsync(model, blogPost);
            
            return View(model);
        }

        #endregion
    }
}
