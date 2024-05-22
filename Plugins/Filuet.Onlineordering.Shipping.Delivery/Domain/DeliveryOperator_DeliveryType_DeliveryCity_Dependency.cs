using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperator_DeliveryType_DeliveryCity_Dependency : BaseEntity
    {
        #region Properties

        public int DeliveryOperatorId { get; set; }
        public int DeliveryTypeId { get; set; }
        public int DeliveryCityId { get; set; }

        #endregion
    }
}
