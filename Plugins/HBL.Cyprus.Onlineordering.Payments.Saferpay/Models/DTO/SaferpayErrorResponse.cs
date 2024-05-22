namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    public class SaferpayErrorResponse
    {
        #region Properties

        public SaferpayResponseHeader ResponseHeader { get; set; }
        public string Behavior { get;set; }
        public string ErrorName { get; set; }
        public string ErrorMessage { get; set; }
        public string[] ErrorDetail { get; set; }

        #endregion
    }
}
