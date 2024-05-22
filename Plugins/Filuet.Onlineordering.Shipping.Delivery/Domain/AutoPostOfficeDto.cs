using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class AutoPostOfficeDto : BaseEntity
    {
        #region Properties

        public string PointId { get; set; }
        public bool Blocked { get; set; }
        public int DeliveryCityId { get; set; }
        public int DeliveryOperatorId { get; set; }
        public int DeliveryTypeId { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public bool PriceIsAbsent { get; set; }
        public int DeliveryOperator_DeliveryType_DeliveryCity_DependencyId { get; set; }

        #endregion
    }
}
