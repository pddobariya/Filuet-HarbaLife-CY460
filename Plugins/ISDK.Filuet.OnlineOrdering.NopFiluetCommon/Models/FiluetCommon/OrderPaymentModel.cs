using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    public class OrderPaymentModel
    {
        #region Properties

        public decimal? PaymentAmount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string CurrencyCode { get; set; }

        public string ApprovalNumber { get; set; }

        public DateTime? AppliedDate { get; set; }

        public decimal? PaymentReceived { get; set; }

        public string PaymentMethodName { get; set; }

        public string Paycode { get; set; }

        public string PaymentType { get; set; }
        public string PaymentMethodId { get; set; }

        #endregion
    }
}
