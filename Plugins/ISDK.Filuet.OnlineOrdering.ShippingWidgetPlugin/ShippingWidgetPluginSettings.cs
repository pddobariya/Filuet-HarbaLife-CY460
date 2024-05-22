using Nop.Core.Configuration;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin
{
    public class ShippingWidgetPluginSettings : ISettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets an address of the DPD Pickup Points FTP
        /// </summary>
        public string DpdFtpUrl { get; set; }

        /// <summary>
        /// Gets or sets a login name of the DPD Pickup Points FTP
        /// </summary>
        public string DpdFtpLogin { get; set; }

        /// <summary>
        /// Gets or sets a password of the DPD Pickup Points FTP
        /// </summary>
        public string DpdFtpPwd { get; set; }

        /// <summary>
        /// Gets or sets an address of the Omniva Pickup Points source
        /// </summary>
        public string OmnivaUrl { get; set; }

        #endregion
    }
}
