using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog
{
    public partial record SubProgramBenefitModel : BaseNopEntityModel
    {
        #region Properties

        public Guid Uid { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Benefits.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.ISDK.Filuet.Theme.FiluetHerbalife.Program.SubCategory.Benefits.Description")]
        public string Description { get; set; }

        public int LanguageId { get; set; }

        public int CategoryId { get; set; }

        #endregion
    }
}
