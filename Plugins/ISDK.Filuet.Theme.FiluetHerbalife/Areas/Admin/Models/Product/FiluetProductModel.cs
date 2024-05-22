using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Product
{
    public record FiluetProductModel: FiluetAdminProductModel, ILocalizedModel<FiluetProductLocalizedModel>
    {
        #region Ctor

        public FiluetProductModel() : base()
        {
            ProductDescription = new ProductDescription();

            Locales = new List<FiluetProductLocalizedModel>();

            PdfFileSearchModel = new PdfFileSearchModel();

            PdfFileModel = new List<PdfFileModel>();
        }

        #endregion

        #region Properties
        public ProductDescription ProductDescription { get; set; }

        public new IList<FiluetProductLocalizedModel> Locales { get; set; }

        public List<PdfFileModel> PdfFileModel { get; set; }

        public PdfFileSearchModel PdfFileSearchModel { get; set; }

        #endregion

    }

    public partial record FiluetProductLocalizedModel : ProductLocalizedModel
    {
        #region Ctor

        public FiluetProductLocalizedModel() : base()
        {
            ProductDescription = new ProductDescription();
        }

        #endregion

        #region Properties

        public ProductDescription ProductDescription { get; set; }

        #endregion


    }
}
