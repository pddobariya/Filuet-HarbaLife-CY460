using Nop.Core;


namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperator : BaseEntity
    {
        #region Properties

        public string FreightCode { get; set; }
        public string WarehouseCode { get; set; }

        #endregion
    }
}
