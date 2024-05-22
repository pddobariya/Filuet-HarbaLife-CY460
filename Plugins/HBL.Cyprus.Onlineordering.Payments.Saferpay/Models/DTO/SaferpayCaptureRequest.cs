namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    #region Properties

    public class SaferpayCaptureRequest
    {
        public SaferpayRequestHeader RequestHeader { get; set; }
        public TransactionReference TransactionReference { get; set; }
    }

    public class TransactionReference
    {
        public string TransactionId { get; set; }
    }

    #endregion
}
