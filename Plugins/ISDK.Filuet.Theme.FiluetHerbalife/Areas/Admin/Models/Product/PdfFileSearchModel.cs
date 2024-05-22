using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Product
{
    public record PdfFileSearchModel : BaseSearchModel
    {
        #region Properties

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        public int DownloadId { get; set; }

        #endregion
    }
}