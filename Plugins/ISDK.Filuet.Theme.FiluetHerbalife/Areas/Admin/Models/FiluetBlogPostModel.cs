using Nop.Web.Areas.Admin.Models.Blogs;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models
{
    public record FiluetBlogPostModel : BlogPostModel
    {
        #region Properties

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.BlogPicture")]
        [UIHint("Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.BlogTagColor")]
        public string TagColor { get; set; }

        [NopResourceDisplayName("ISDK.Filuet.Theme.FiluetHerbalife.BlogIsPinned")]
        public bool IsPinned { get; set; }

        #endregion
    }
}
