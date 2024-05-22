using System;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog
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

        public string Description { get; set; }

        public List<PdfFile> PdfFiles { get; set; }

        #endregion

    }

    public class PdfFile
    {
        #region Properties

        public Guid DownloadGuid { get; set; }
        public string Label { get; set; }

        #endregion
    }
}
