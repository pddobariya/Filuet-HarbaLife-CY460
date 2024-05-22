using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class PickupPointsOperatorDto : BaseEntity
    {
        #region Properties

        public string OperatorName { get; set; }
        public string WarehouseCode { get; set; }
        public decimal MinRetailPrice { get; set; }
        public decimal MaxRetailPrice { get; set; }
        public string DeliveryPrise { get; set; }
        public string FreightCode { get; set; }

        #endregion
    }
}
