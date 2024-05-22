using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class Price : BaseEntity
    {
        #region Properties

        public string CriterionValues { get; set; }
        public decimal DeliveryPrise { get; set; }
        public int DeliveryOperator_DeliveryType_DeliveryCity_DependencyId { get; set; }

        #endregion
    }
}
