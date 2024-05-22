using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperatorsCityDto : BaseEntity
    {
        #region Properties

        public string CityName { get; set; }
        public DeliveryOperator_DeliveryType_DeliveryCity_Dependency[] DeliveryOperator_DeliveryType_DeliveryCity_Dependencies { get; set; }
        
        #endregion
    }
}
