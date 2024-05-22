using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public partial class SalesCenterDto : BaseEntity
    {
        #region Properties

        public decimal Price { get; set; }

        public string WarehouseCode { get; set; }

        public string FreightCode { get; set; }

        public string VolumePoints { get; set; }

        public string WorkTime { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        #endregion
    }
}
