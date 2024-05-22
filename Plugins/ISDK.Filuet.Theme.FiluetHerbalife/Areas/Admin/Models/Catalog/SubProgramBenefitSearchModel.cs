using Nop.Web.Framework.Models;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models.Catalog
{
    public partial record SubProgramBenefitSearchModel : BaseSearchModel
    {
        #region Properties

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int LanguageId { get; set; }

        #endregion
    }
}
