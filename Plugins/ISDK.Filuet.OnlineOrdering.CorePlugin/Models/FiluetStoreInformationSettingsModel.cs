using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models
{
    public record FiluetStoreInformationSettingsModel : StoreInformationSettingsModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Settings.GeneralCommon.InstagramLink")]
        public new string InstagramLink { get; set; }
        public new bool InstagramLink_OverrideForStore { get; set; }

        #endregion
    }
}
