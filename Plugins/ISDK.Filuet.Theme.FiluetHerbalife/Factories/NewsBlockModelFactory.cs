using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Models.Blogs;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public class NewsBlockModelFactory : INewsBlockModelFactory
    {
        #region Fields

        private readonly BlogSettings _blogSettings;
        private readonly IBlogService _blogService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        #region Ctor

        public NewsBlockModelFactory(
            BlogSettings blogSettings,
            IBlogService blogService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IUrlRecordService urlRecordService,
            IPictureService pictureService,
            IGenericAttributeService genericAttributeService, IHttpContextAccessor httpContextAccessor)
        {
            _blogSettings = blogSettings;
            _blogService = blogService;
            _workContext = workContext;
            _urlRecordService = urlRecordService;
            _pictureService = pictureService;
            _genericAttributeService = genericAttributeService;
            _httpContextAccessor = httpContextAccessor;
            _storeContext = storeContext;
        }

        #endregion

        #region Utilities

        private string GetDay(BlogPost blogPost)
        {
            var date = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
            return date.Day.ToString("00");
        }

        private string GetMonth(BlogPost blogPost, string languageCulture)
        {
            var date = blogPost.StartDateUtc ?? blogPost.CreatedOnUtc;
            return date.ToString("MMM", new CultureInfo(languageCulture));
        }

        private string GetCategory(string tags)
        {
            var tagArr = tags?.Split(',');
            if (tagArr != null && tagArr.Length > 0)
                return tagArr[0];
            return "";
        }

        private async Task<string> GetPictureUrlAsync(BlogPost blogPost)
        {
            var pictureId = await _genericAttributeService.GetAttributeAsync<int>(blogPost, FiluetThemePluginDefaults.BlogPictureId);

            var url = await _pictureService.GetPictureUrlAsync(pictureId, showDefaultPicture: false) ?? "";
            return url;
        }

        private string GetMessangersUrl()
        {
            var host = "https://telegram.me/share/url?";
            return string.Format($"{host}url={_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}");
        }

        #endregion

        #region Methods

        public virtual async Task<NewsBlockModel> PrepareNewsBlockModelAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var posts = await _blogService.GetAllBlogPostsAsync(store.Id, language.Id, pageSize: 5);
            var newsBlockModel = new NewsBlockModel();
            if (posts.Any())
            {
                newsBlockModel.ArticleBigModel =
                    await PrepareArticleBigModelAsync(posts.First(), language.LanguageCulture);
                newsBlockModel.ArticleModels = await posts.Skip(1)
                    .SelectAwait(async post => await PrepareArticleModelAsync(post, language.LanguageCulture))
                    .ToListAsync();
            }

            while (newsBlockModel.ArticleModels.Count < 4)
            {
                newsBlockModel.ArticleModels.Add(new ArticleModel());
            }

            return newsBlockModel;
        }

        public virtual async Task<ArticleModel> PrepareArticleModelAsync(BlogPost blogPost, string languageCulture)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            var articleDto = new ArticleModel();
            articleDto.Id = blogPost.Id;
            articleDto.PictureUrl = await GetPictureUrlAsync(blogPost);
            articleDto.Day = GetDay(blogPost);
            articleDto.Month = GetMonth(blogPost, languageCulture);
            articleDto.Name = blogPost.Title;
            articleDto.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            articleDto.Category = GetCategory(blogPost.Tags);
            articleDto.TagColor = await _genericAttributeService.GetAttributeAsync<string>(blogPost, FiluetThemePluginDefaults.BlogTagColor);
            articleDto.TelegramUrl = GetMessangersUrl();
            articleDto.IsPinned = await _genericAttributeService.GetAttributeAsync<bool>(blogPost, FiluetThemePluginDefaults.BlogIsPinned);

            return articleDto;
        }

        public virtual async Task<ArticleBigModel> PrepareArticleBigModelAsync(BlogPost blogPost, string languageCulture)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            var articleBigDto = new ArticleBigModel();
            articleBigDto.Id = blogPost.Id;
            articleBigDto.PictureUrl = await GetPictureUrlAsync(blogPost);
            articleBigDto.Day = GetDay(blogPost);
            articleBigDto.Month = GetMonth(blogPost, languageCulture);
            articleBigDto.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            articleBigDto.Name = blogPost.Title;
            articleBigDto.Category = GetCategory(blogPost.Tags);
            articleBigDto.TagColor = await _genericAttributeService.GetAttributeAsync<string>(blogPost, FiluetThemePluginDefaults.BlogTagColor);
            articleBigDto.TelegramUrl = GetMessangersUrl();
            articleBigDto.BodyOverview = !string.IsNullOrEmpty(blogPost.BodyOverview) ? blogPost.BodyOverview : blogPost.Body;
            articleBigDto.IsPinned = await _genericAttributeService.GetAttributeAsync<bool>(blogPost, FiluetThemePluginDefaults.BlogIsPinned);
            return articleBigDto;
        }

        public virtual async Task PrepareBlogPostModelAsync(FiluetBlogPostModel model, BlogPost blogPost, bool prepareLastNews = true)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));

            var language = await _workContext.GetWorkingLanguageAsync();

            model.Id = blogPost.Id;
            model.PictureUrl = await GetPictureUrlAsync(blogPost);
            model.Day = GetDay(blogPost);
            model.Month = GetMonth(blogPost, language.LanguageCulture);
            model.SeName = await _urlRecordService.GetSeNameAsync(blogPost, blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = blogPost.Title;
            model.Category = GetCategory(blogPost.Tags);
            model.TagColor = await _genericAttributeService.GetAttributeAsync<string>(blogPost, FiluetThemePluginDefaults.BlogTagColor);
            model.TelegramUrl = GetMessangersUrl();
            model.BodyOverview = !string.IsNullOrEmpty(blogPost.BodyOverview) ? blogPost.BodyOverview : blogPost.Body;
            model.IsPinned = await _genericAttributeService.GetAttributeAsync<bool>(blogPost, FiluetThemePluginDefaults.BlogIsPinned);

            if (prepareLastNews)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var posts = await _blogService.GetAllBlogPostsAsync(store.Id, language.Id, pageSize: 5);

                foreach (var post in posts)
                {
                    if (blogPost.Id == post.Id)
                        continue;

                    var seName = await _urlRecordService.GetSeNameAsync(post, post.LanguageId, ensureTwoPublishedLanguages: false);
                    model.LastNews.Add((SeName: seName, Title: post.Title));
                }
            }
        }

        public virtual async Task<FiluetBlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.PageSize <= 0)
                command.PageSize = _blogSettings.PostsPageSize;
            if (command.PageNumber <= 0)
                command.PageNumber = 1;

            var dateFrom = command.GetFromMonth();
            var dateTo = command.GetToMonth();

            var language = await _workContext.GetWorkingLanguageAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var blogPosts = string.IsNullOrEmpty(command.Tag)
                ? await _blogService.GetAllBlogPostsAsync(store.Id, language.Id, dateFrom, dateTo, command.PageNumber - 1, command.PageSize)
                : await _blogService.GetAllBlogPostsByTagAsync(store.Id, language.Id, command.Tag, command.PageNumber - 1, command.PageSize);

            var model = new FiluetBlogPostListModel
            {
                PagingFilteringContext = { Tag = command.Tag, Month = command.Month },
                WorkingLanguageId = language.Id
            };

            if (blogPosts.Any())
            {
                model.ArticleBigModel = await PrepareArticleBigModelAsync(blogPosts.First(), language.LanguageCulture);
                model.ArticleModels = await blogPosts.Skip(1).SelectAwait(async blogPost =>
                    {
                        return await PrepareArticleModelAsync(blogPost, language.LanguageCulture);
                    }).ToListAsync();
            }
            model.PagingFilteringContext.LoadPagedList(blogPosts);

            return model;
        }

        #endregion

       
    }
}
