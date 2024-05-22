using Nop.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperator_DeliveryType_Dependency : BaseEntity
    {
        #region Properties

        [ForeignKey("DeliveryOperator")]
        public int DeliveryOperatorId { get; set; }
        public virtual DeliveryOperator DeliveryOperator { get; set; }
        [ForeignKey("DeliveryType")]
        public int DeliveryTypeId { get; set; }
        public virtual DeliveryType DeliveryType { get; set; }

        public virtual List<Price> Prices { get; set; }
        public virtual List<AutoPostOffice> AutoPostOffices { get; set; }

        #endregion
    }
}
