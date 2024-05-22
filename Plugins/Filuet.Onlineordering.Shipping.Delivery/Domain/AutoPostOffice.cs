using Nop.Core;


namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class AutoPostOffice : BaseEntity
    {
        #region Properties

        public string PointId { get; set; }
        public bool Blocked { get; set; }
        public int DeliveryOperator_DeliveryType_DeliveryCity_DependencyId { get; set; }

        #endregion
    }
}
