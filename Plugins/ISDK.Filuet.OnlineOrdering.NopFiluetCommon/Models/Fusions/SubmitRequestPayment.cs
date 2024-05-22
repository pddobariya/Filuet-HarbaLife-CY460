using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Fusions
{
    public class SubmitRequestPayment
    {
        public SubmitRequestPayment()
        {
        }

        #region Properties

        public DateTime AppliedDate { get; set; }
        public string ApprovalNumber { get; set; }
        public string ClientRefNumber { get; set; }
        public object CreditCard { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime Date { get; set; }
        public string Paycode { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public decimal PaymentReceived { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentType { get; set; }

        #endregion
    }
}