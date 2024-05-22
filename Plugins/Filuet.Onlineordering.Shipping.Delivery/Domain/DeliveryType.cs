using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryType : BaseEntity
    {
        #region Properties

        public bool IsActive { get; set; }
        public string SystemType { get; set; }

        #endregion
    }
}
