using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.Payeezy.Models
{
    public record PaymentErrorModel: BaseNopModel
    {
        #region Properties

        public bool HasOrder { get; set; }
        public string RedirectUrl { get; set; }

        #endregion

        #region Ctor

        public PaymentErrorModel(bool hasOrder, string redirectUrl)
        {
            HasOrder = hasOrder;
            RedirectUrl = redirectUrl;
        }

        #endregion
    }
}
