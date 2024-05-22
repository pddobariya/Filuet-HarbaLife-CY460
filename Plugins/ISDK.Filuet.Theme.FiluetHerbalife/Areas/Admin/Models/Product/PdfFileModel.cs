using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Product
{
    public record PdfFileModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Product.ProductDescription.PdfFile.PdfRef")]
        [Required]
        [UIHint("Download")]
        public int DownloadId { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Product.ProductDescription.PdfFile.PdfLabel")]
        [Required]
        public string Label { get; set; }

        public Guid DownloadGuid { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        #endregion
    }
}
