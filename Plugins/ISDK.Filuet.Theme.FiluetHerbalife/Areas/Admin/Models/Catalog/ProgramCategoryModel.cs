using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog
{
    public class ProgramCategoryModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.Slogan")]
        public string Slogan { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.DownloadLink")]
        public string DownloadLink { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.ExpertPhotoUrl")]
        public string ExpertPhotoUrl { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.ExpertReview")]
        public string ExpertReview { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.ExpertName")]
        public string ExpertName { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.Category.ExpertPost")]
        public string ExpertPost { get; set; }

        #endregion
    }
}
