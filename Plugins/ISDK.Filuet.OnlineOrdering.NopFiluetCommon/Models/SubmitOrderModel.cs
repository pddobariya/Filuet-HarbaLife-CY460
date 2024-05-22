using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public class SubmitOrderModel
    {
        #region Properties

        public FusionServiceParamsModel FusionServiceParams { get; set; }
        public ShoppingCartTotalModel ShoppingCartTotalModel { get; set; }
        public OrderPaymentModel OrderPaymentModel { get; set; }
        public string Notes { get; set; }
        public DateTime OrderWebDate { get; set; }
        public string PaymentStatus { get; set; }

        #endregion
    }
}
