using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Infrastructure;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Services.Blogs;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Factories
{
    public class FiluetBlogModelFactory : BlogModelFactory
    {
        # region Fields

        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FiluetBlogModelFactory(
            CatalogSettings catalogSettings,
            IBaseAdminModelFactory baseAdminModelFactory,
            IBlogService blogService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper, 
            IHtmlFormatter htmlFormatter, 
            ILanguageService languageService,
            ILocalizationService localizationService,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IStoreService storeService,
            IUrlRecordService urlRecordService,
            IGenericAttributeService genericAttributeService) : base(catalogSettings, baseAdminModelFactory, blogService, customerService, dateTimeHelper, htmlFormatter, languageService, localizationService, storeMappingSupportedModelFactory, storeService, urlRecordService)
        {
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public override async Task<BlogPostModel> PrepareBlogPostModelAsync(BlogPostModel model, BlogPost blogPost, bool excludeProperties = false)
        {
            var blogPostModel = await base.PrepareBlogPostModelAsync(model, blogPost, excludeProperties);
            var filuetBlogPostModel = PluginMapper.Mapper.Map<FiluetBlogPostModel>(blogPostModel);

            if (blogPost != null)
            {
                filuetBlogPostModel.PictureId = await _genericAttributeService.GetAttributeAsync<int>(blogPost, FiluetThemePluginDefaults.BlogPictureId);
                filuetBlogPostModel.TagColor = await _genericAttributeService.GetAttributeAsync<string>(blogPost, FiluetThemePluginDefaults.BlogTagColor);
                filuetBlogPostModel.IsPinned = await _genericAttributeService.GetAttributeAsync<bool>(blogPost, FiluetThemePluginDefaults.BlogIsPinned);
            }

            return filuetBlogPostModel;
        }

        #endregion
    }
}
