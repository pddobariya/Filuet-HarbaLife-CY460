using Nop.Web.Models.Blogs;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models
{
    public record FiluetBlogPostModel : BlogPostModel
    {
        #region Ctor

        public FiluetBlogPostModel()
        {
            LastNews = new List<(string seName, string title)>();
        }

        #endregion

        #region Properties

        public string PictureUrl { get; set; }
        public string Day { get; set; }
        public string Month { get; set; }
        public string Category { get; set; }
        public string TagColor { get; set; }
        public string TelegramUrl { get; set; }
        public bool IsPinned { get; set; }
        
        public List<(string SeName, string Title)> LastNews { get; set; }

        #endregion   
    }
}
