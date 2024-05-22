namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO
{
    #region Properties

    public class Payment
    {
        public Amount Amount { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
    }

    public class ReturnUrls
    {
        public string Success { get; set; }
        public string Fail { get; set; }
    }

    public class Notification
    {
        public string NotifyUrl { get; set; }
    }

    public class Authentication
    {
        public string ThreeDsChallenge { get; set; }
    }

    public class SaferpayInitializeRequest
    {
        public SaferpayRequestHeader RequestHeader { get; set; }
        public string TerminalId { get; set; }
        public Payment Payment { get; set; }
        public ReturnUrls ReturnUrls { get; set; }
        public Notification Notification { get; set; }
        public Authentication Authentication { get; set; }
    }

    #endregion
}
