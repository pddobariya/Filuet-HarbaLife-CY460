using Nop.Core.Configuration;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay
{
    public class SaferpayPaymentSettings : ISettings
    {
        #region Properties

        public string CustomerId { get; set; }

        public string TerminalId { get; set; }

        public bool Bypass { get; set; }

        public string APIUrl { get; set; }

        public string APIUsername { get; set; }

        public string APIPassword { get; set; }

        public string APISpecVersion { get; set; }

        public string CurrencyCode { get; set; }

        #endregion
    }
}
