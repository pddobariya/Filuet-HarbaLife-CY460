using Nop.Web.Models.Blogs;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models
{
    public class FiluetBlogPostListModel
    {
        #region Ctor

        public FiluetBlogPostListModel()
        {
            PagingFilteringContext = new BlogPagingFilteringModel();
            ArticleModels = new List<ArticleModel>();
        }

        #endregion

        #region Properties

        public int WorkingLanguageId { get; set; }
        public BlogPagingFilteringModel PagingFilteringContext { get; set; }
        public ArticleBigModel ArticleBigModel { get; set; }
        public IList<ArticleModel> ArticleModels { get; set; }

        #endregion
    }
}
