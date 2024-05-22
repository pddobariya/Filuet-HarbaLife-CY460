using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class OrderPayment
    {
        #region Properties

        public string PaymentMethodName { get; set; }

        public decimal PaymentAmount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string Paycode { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime? AppliedDate { get; set; }

        public string ApprovalNumber { get; set; }

        public decimal PaymentReceived { get; set; }

        public string AuthorizationType { get; set; }

        public string VoidFlag { get; set; }

        #endregion
    }
}
