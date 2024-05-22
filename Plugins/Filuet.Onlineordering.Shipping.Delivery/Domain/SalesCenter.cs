using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class SalesCenter : BaseEntity
    {
        #region Properties

        public decimal Price { get; set; }

        public string WarehouseCode { get; set; }

        public string FreightCode { get; set; }

        public string VolumePoints { get; set; }

        #endregion
    }
}
