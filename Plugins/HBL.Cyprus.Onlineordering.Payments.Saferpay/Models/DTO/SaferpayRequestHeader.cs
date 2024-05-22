namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    #region Properties

    public class SaferpayRequestHeader
    {
        public string SpecVersion { get; set; }
        public string CustomerId { get; set; }
        public string RequestId { get; set; }
        public int RetryIndicator { get; set; }
    }

    public class Amount
    {
        public string Value { get; set; }
        public string CurrencyCode { get; set; }
    }

    #endregion
}
