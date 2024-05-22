
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryObject
    {
        #region Properties

        public decimal DeliveryPrise { get; set; }
        public string DeliveryCity { get; set; }
        public string Address { get; set; }
        public string FreightCode { get; set; }
        public string WarehouseCode { get; set; }
        public string OperatorName { get; set; }
        public string PointId { get; set; }
        public Dictionary<int, string> Comment { get; set; }

        #endregion
    }
}
