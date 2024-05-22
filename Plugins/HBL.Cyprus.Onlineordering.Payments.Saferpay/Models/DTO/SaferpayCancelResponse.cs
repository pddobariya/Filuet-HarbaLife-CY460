namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    public class SaferpayCancelResponse
    {
        #region Properties

        public SaferpayResponseHeader ResponseHeader { get; set; }
        public string TransactionId { get; set; }
        public string OrderId { get; set; }
        public string Date { get; set; }

        #endregion
    }
}
