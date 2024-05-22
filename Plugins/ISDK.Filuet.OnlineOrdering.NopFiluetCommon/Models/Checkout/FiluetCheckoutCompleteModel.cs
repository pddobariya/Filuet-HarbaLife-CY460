using Nop.Web.Models.Checkout;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Checkout
{
    public record FiluetCheckoutCompleteModel : CheckoutCompletedModel
    {
        #region Properties

        public string FusionOrderId { get; set; }
        public bool ApfCompleted { get; set; }

        #endregion
    }
}
