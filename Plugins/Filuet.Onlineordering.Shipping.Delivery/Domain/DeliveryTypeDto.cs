using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryTypeDto : BaseEntity
    {
        #region Properties

        public string TypeName { get; set; }
        public string SystemType { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }

        #endregion
    }
}
