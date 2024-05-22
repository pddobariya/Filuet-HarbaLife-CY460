using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Filuet.OnlineOrdering.CyExtendedFunctions.Areas.Admin.Models
{
    public record class ConfigurationModel : BaseNopModel
    {
        #region Properties
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Filuet.OnlineOrdering.CyExtendedFunctions.AddSKU3798ForSKU5451")]
        public bool AddSKU3798ForSKU5451 { get; set; }
        [NopResourceDisplayName("Filuet.OnlineOrdering.CyExtendedFunctions.AddSKU3798ForSKU5451")]
        public bool AddSKU3798ForSKU5451_OverrideForStore { get; set; }
        #endregion
    }
}
