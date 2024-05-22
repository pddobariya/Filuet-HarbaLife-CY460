using Nop.Web.Models.Common;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models
{
    public record FiluetSocialModel : SocialModel
    {
        #region Properties

        public new string InstagramLink { get; set; }

        #endregion
    }
}
