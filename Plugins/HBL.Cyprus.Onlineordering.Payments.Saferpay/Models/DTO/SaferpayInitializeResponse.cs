using System;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    public class SaferpayInitializeResponse
    {
        #region Properties

        public SaferpayResponseHeader ResponseHeader { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RedirectUrl { get; set; }

        #endregion
    }
}