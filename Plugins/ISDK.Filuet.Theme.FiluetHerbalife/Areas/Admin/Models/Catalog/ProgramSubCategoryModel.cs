using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog
{
    public class ProgramSubCategoryModel
    {
        #region Ctor

        public ProgramSubCategoryModel()
        {
            ProgramBenefits = new List<SubProgramBenefitModel>();
            ProgramBenefitSearchModel = new SubProgramBenefitSearchModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Slogan")]
        public string Slogan { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.DownloadLink")]
        public string DownloadLink { get; set; }


        public int ProgramTypeId { get; set; } = 1;

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.ProgramTypeName")]
        public string ProgramTypeName { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.ImageSrc")]
        public string ImageSrc { get; set; }

        public int SecondProgramTypeId { get; set; } = 2;

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.SecondProgramTypeName")]
        public string SecondProgramTypeName { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.SecondProgramTypeRef")]
        public string SecondProgramTypeRef { get; set; }

        public int ThirdProgramTypeId { get; set; } = 3;

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.ThirdProgramTypeName")]
        public string ThirdProgramTypeName { get; set; } 

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.ThirdProgramTypeRef")]
        public string ThirdProgramTypeRef { get; set; }


        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Footnote")]
        public string Footnote { get; set; }

        public List<SubProgramBenefitModel> ProgramBenefits { get; set; }

        public SubProgramBenefitSearchModel ProgramBenefitSearchModel { get; set; }

        #endregion 

    }
}
