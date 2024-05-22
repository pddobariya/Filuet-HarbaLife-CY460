using Nop.Web.Models.Catalog;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog
{
    public record FiluetCategoryModel : CategoryModel
    {
        #region Ctor

        public FiluetCategoryModel() : base()
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
