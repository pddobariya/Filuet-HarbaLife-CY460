using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Nop.Core.Domain.Blogs;
using Nop.Web.Models.Blogs;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public interface INewsBlockModelFactory
    {
        #region Methods

        Task<NewsBlockModel> PrepareNewsBlockModelAsync();
        Task PrepareBlogPostModelAsync(FiluetBlogPostModel model, BlogPost blogPost, bool prepareLastNews = true);
        Task<FiluetBlogPostListModel> PrepareBlogPostListModelAsync(BlogPagingFilteringModel command);

        #endregion
    }
}
