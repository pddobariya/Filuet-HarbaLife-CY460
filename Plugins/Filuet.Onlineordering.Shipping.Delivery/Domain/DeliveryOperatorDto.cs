using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperatorDto : BaseEntity
    {
        #region Properties

        public string OperatorName { get; set; }
        public string FreightCode { get; set; }
        public string WarehouseCode { get; set; }

        #endregion
    }
}
