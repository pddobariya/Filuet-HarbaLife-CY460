using Nop.Web.Models.Common;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models
{
    public record FiluetHerbalifeSocialModel : SocialModel
    {
        #region Properties
        public new string InstagramLink { get; set; }
        public string FiluetLinkedinLink { get; set; }
        public string FiluetInstagramLink { get; set; }
        public string FiluetFacebookLink { get; set; }

        #endregion
    }
}
