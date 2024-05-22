using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class PriceDto : BaseEntity
    {
        #region Properties

        public decimal DeliveryPrise { get; set; }
        public int DeliveryCityId { get; set; }
        public int DeliveryOperatorId { get; set; }
        public int DeliveryTypeId { get; set; }
        public decimal MinCriterionValue { get; set; }
        public decimal MaxCriterionValue { get; set; }

        #endregion
    }
}
