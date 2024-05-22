using System;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog
{
    public class ProgramSubCategoryModel
    {
        #region Ctor

        public ProgramSubCategoryModel()
        {
            ProgramBenefits = new List<ProgramBenefit>();
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public string Title { get; set; }

        public string ImageSrc { get; set; }

        public string Slogan { get; set; }

        public string Description { get; set; }

        public string DownloadLink { get; set; }

        public int ProgramTypeId { get; set; } = 1;
        public string ProgramTypeName { get; set; }

        public int SecondProgramTypeId { get; set; } = 2;
        public string SecondProgramTypeName { get; set; }
        public string SecondProgramTypeRef { get; set; }

        public string SecondProgramImgRef { get; set; }

        public int ThirdProgramTypeId { get; set; } = 3;
        public string ThirdProgramTypeName { get; set; }
        public string ThirdProgramTypeRef { get; set; }
        public string ThirdProgramImgRef { get; set; }

        public string Footnote { get; set; }

        public List<ProgramBenefit> ProgramBenefits { get; set; }

        #endregion
    }

    public class ProgramBenefit
    {
        #region Properties

        public Guid Uid { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        #endregion
    }
}
