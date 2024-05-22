using Nop.Web.Areas.Admin.Models.Settings;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models
{
    public record FiluetGeneralCommonSettingsModel : GeneralCommonSettingsModel
    {
        #region Ctor

        public FiluetGeneralCommonSettingsModel()
        {
            StoreInformationSettings = new FiluetStoreInformationSettingsModel();
        }

        #endregion
    }
}
