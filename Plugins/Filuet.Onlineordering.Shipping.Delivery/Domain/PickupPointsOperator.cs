using Newtonsoft.Json;
using Nop.Core;
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class PickupPointsOperator : BaseEntity
    {
        #region Properties

        public string OperatorName { get; set; }
        public string WarehouseCode { get; set; }
        public string RetailPrice { get; set; }
        public string DeliveryPrise { get; set; }
        public string FreightCode { get; set; }
        [JsonIgnore]
        public virtual List<PickupPointsCost> PickupPointsCosts { get; set; }

        #endregion
    }
}
