namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    public class SaferpayAssertRequest
    {
        #region Properties

        public SaferpayRequestHeader RequestHeader { get; set; }
        public string Token { get; set; }

        #endregion
    }
}
