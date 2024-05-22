using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog
{
    public record FiluetCategoryModel : CategoryModel, ILocalizedModel<FiluetCategoryLocalizedModel>
    {
        #region Ctor

        public FiluetCategoryModel() : base()
        {
            ProgramCategoryModel = new ProgramCategoryModel();
            ProgramSubCategoryModel = new ProgramSubCategoryModel();

            Locales = new List<FiluetCategoryLocalizedModel>();
        }

        #endregion

        #region Properties
        public ProgramCategoryModel ProgramCategoryModel { get; set; }
        public ProgramSubCategoryModel ProgramSubCategoryModel { get; set; }

        public new IList<FiluetCategoryLocalizedModel> Locales { get; set; }

        #endregion

    }

    public partial record FiluetCategoryLocalizedModel : CategoryLocalizedModel
    {
        #region Ctor

        public FiluetCategoryLocalizedModel() : base()
        {
            ProgramCategoryModel = new ProgramCategoryModel();
            ProgramSubCategoryModel = new ProgramSubCategoryModel();
        }

        #endregion

        #region Properties

        public ProgramCategoryModel ProgramCategoryModel { get; set; }
        public ProgramSubCategoryModel ProgramSubCategoryModel { get; set; }

        #endregion
 
    }
}
