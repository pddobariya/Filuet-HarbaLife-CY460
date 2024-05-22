namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    public class SaferpayCaptureResponse
    {
        #region Properties

        public SaferpayResponseHeader ResponseHeader { get; set; }
        public string CaptureId { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }

        #endregion
    }
}
