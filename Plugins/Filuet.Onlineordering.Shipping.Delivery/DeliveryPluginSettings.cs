using Nop.Core.Configuration;

namespace Filuet.Onlineordering.Shipping.Delivery
{
    public class DeliveryPluginSettings : ISettings
    {
        #region Properties

        public string PhonePrefix { get; set; }
        public string PhoneMask { get; set; }
        public bool SelfPickupActive { get; set; }
        public bool AddAddressToComment { get; set; }
        public bool RequirePostCode { get; set; }
        public string SalesCenterElectronicQueueInvitation { get; set; }
        public string NotificationHtmlAboveModule { get; set; }
        public bool ShowInvitation { get; set; }
        public int MinCriterion { get; set; }
        public decimal PickPoint { get; set; }
        public decimal HomeDelivery { get; set; }

        #endregion
    }
}
