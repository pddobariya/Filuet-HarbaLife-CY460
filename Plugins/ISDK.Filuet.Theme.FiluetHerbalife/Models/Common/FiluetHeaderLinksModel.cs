using Nop.Web.Models.Common;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models.Common
{
    public partial record FiluetHeaderLinksModel : HeaderLinksModel
    {
        #region Properties

        public decimal TV { get; set; }
        public string LogoutLink { get; set; }

        #endregion
    }
}