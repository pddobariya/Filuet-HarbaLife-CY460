using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class PickupPointsCostDto : BaseEntity
    {
        #region Properties

        public string PointId { get; set; }

        public bool Blocked { get; set; }

        public string WarehouseCode { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        public int PickupPointsOperatorId { get; set; }

        #endregion
    }
}
