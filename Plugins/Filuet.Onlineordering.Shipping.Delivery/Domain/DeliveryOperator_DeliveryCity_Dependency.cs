using Nop.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperator_DeliveryCity_Dependency : BaseEntity
    {
        #region Properties

        [ForeignKey("DeliveryOperator")]
        public int DeliveryOperatorId { get; set; }
        public virtual DeliveryOperator DeliveryOperator { get; set; }
        [ForeignKey("DeliveryCity")]
        public int DeliveryCityId { get; set; }
        public virtual DeliveryCity DeliveryCity { get; set; }

        #endregion
    }
}
