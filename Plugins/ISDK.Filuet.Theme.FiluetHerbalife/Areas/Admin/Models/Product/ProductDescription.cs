using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Product
{
    public class ProductDescription
    {
        #region Ctor

        public ProductDescription()
        {
            PdfFiles = new List<PdfFile>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Product.ProductDescription.Description")]
        public string Description { get; set; }

        public List<PdfFile> PdfFiles { get; set; }

        #endregion

    }
    public record PdfFile
    {
        #region Properties

        public int DownloadId { get; set; }
        public Guid DownloadGuid { get; set; }
        public string Label { get; set; }

        #endregion
    }
}
