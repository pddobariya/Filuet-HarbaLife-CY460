using Nop.Web.Framework.Models;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Models
{
    public record BaseErrorModel  : BaseNopModel
    {
        #region Properties

        public bool HasOrder { get; set; }

        public string RedirectUrl { get; set; }

        public string Message { get; set; }

        #endregion

        #region Ctor
        public BaseErrorModel(bool hasOrder, string redirectUrl, string message)
        {
            HasOrder = hasOrder;
            RedirectUrl = redirectUrl;
            Message = message;
        }

        public BaseErrorModel()
        {
            HasOrder = false;
            RedirectUrl = null;
            Message = null;
        }

        #endregion
    }
}
