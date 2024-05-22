using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Seo;
using Nop.Core.Events;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Controllers
{
    [NameControllerModelConvention("Blog")]
    public class FiluetBlogController : BlogController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IBlogService _blogService;
        private readonly IBlogModelFactory _blogModelFactory;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly INotificationService _notificationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPictureService _pictureService;

        #endregion

        #region Ctor

        public FiluetBlogController(
            IBlogModelFactory blogModelFactory,
            IBlogService blogService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IGenericAttributeService genericAttributeService,
            IPictureService pictureService) : base(blogModelFactory, blogService, customerActivityService, eventPublisher, localizationService, notificationService, permissionService, storeMappingService, storeService, urlRecordService)
        {
            _blogModelFactory = blogModelFactory;
            _blogService = blogService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _urlRecordService = urlRecordService;
            _genericAttributeService = genericAttributeService;
            _pictureService = pictureService;
        }


        #endregion

        #region Utilities

        private async Task DeletePicture(BlogPost blogPost)
        {
            int pictureId = await _genericAttributeService.GetAttributeAsync<int>(blogPost, "PictureId");

            var picture = await _pictureService.GetPictureByIdAsync(pictureId);
            if (picture != null)
                await _pictureService.DeletePictureAsync(picture);
        }

        #endregion

        #region Methods

        #region BlogPostCreate

        [NonAction]
        public override async Task<IActionResult> BlogPostCreate(BlogPostModel model, bool continueEditing)
        {
            return await base.BlogPostCreate(model, continueEditing);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> BlogPostCreate(FiluetBlogPostModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var blogPost = model.ToEntity<BlogPost>();
                blogPost.CreatedOnUtc = DateTime.UtcNow;
                await _blogService.InsertBlogPostAsync(blogPost);

                await _genericAttributeService.SaveAttributeAsync(blogPost, FiluetThemePluginDefaults.BlogPictureId, model.PictureId);
                await _genericAttributeService.SaveAttributeAsync(blogPost, FiluetThemePluginDefaults.BlogTagColor, model.TagColor);
                await _genericAttributeService.SaveAttributeAsync(blogPost, FiluetThemePluginDefaults.BlogIsPinned, model.IsPinned);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewBlogPost",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewBlogPost"), blogPost.Id), blogPost);

                //search engine name
                var seName = await _urlRecordService.ValidateSeNameAsync(blogPost, model.SeName, model.Title, true);
                await _urlRecordService.SaveSlugAsync(blogPost, seName, blogPost.LanguageId);

                //Stores
                await SaveStoreMappingsAsync(blogPost, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Added"));

                if (!continueEditing)
                    return RedirectToAction("BlogPosts");

                return RedirectToAction("BlogPostEdit", new { id = blogPost.Id });
            }

            //prepare model
            model = await _blogModelFactory.PrepareBlogPostModelAsync(model, null, true) as FiluetBlogPostModel;

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region BlogPostEdit
        
        [NonAction]
        public override async Task<IActionResult> BlogPostEdit(BlogPostModel model, bool continueEditing)
        {
            return await base.BlogPostEdit(model, continueEditing);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> BlogPostEdit(FiluetBlogPostModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();
            
            //try to get a blog post with the specified id
            var blogPost = await _blogService.GetBlogPostByIdAsync(model.Id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            //update picture and generic attributes
            int pictureId = await _genericAttributeService.GetAttributeAsync<int>(blogPost, FiluetThemePluginDefaults.BlogPictureId);
            if (pictureId != model.PictureId)
            {
                await DeletePicture(blogPost);
                await _genericAttributeService.SaveAttributeAsync(blogPost, FiluetThemePluginDefaults.BlogPictureId, model.PictureId);
            }

            await _genericAttributeService.SaveAttributeAsync(blogPost, FiluetThemePluginDefaults.BlogTagColor, model.TagColor);
            await _genericAttributeService.SaveAttributeAsync(blogPost, FiluetThemePluginDefaults.BlogIsPinned, model.IsPinned);
            
            return await base.BlogPostEdit(model, continueEditing);
        }

        #endregion

        #region Delete

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageBlog))
                return AccessDeniedView();

            //try to get a blog post with the specified id
            var blogPost = await _blogService.GetBlogPostByIdAsync(id);
            if (blogPost == null)
                return RedirectToAction("BlogPosts");

            await DeletePicture(blogPost);

            var blogPostAttributes = await _genericAttributeService.GetAttributesForEntityAsync(blogPost.Id, blogPost.GetType().Name);
            await _genericAttributeService.DeleteAttributesAsync(blogPostAttributes);
            
            await _blogService.DeleteBlogPostAsync(blogPost);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteBlogPost",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteBlogPost"), blogPost.Id), blogPost);

            //delete engine name
            var slug = await _urlRecordService.GetActiveSlugAsync(blogPost.Id, nameof(BlogPost), blogPost.LanguageId);
            await _urlRecordService.DeleteUrlRecordsAsync(new List<UrlRecord> 
            {
                await _urlRecordService.GetBySlugAsync(slug)
            });
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.ContentManagement.Blog.BlogPosts.Deleted"));
            return RedirectToAction("BlogPosts");
        }

        #endregion

        #endregion
    }
}
