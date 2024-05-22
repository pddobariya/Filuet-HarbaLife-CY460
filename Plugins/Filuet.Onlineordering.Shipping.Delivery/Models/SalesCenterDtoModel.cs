using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record SalesCenterDtoModel : BaseNopEntityModel
    {
        #region Properties

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.Price")]
        public decimal Price { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.WarehouseCode")]
        public string WarehouseCode { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.FreightCode")]
        public string FreightCode { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.VolumePoints")]
        public string VolumePoints { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.WorkTime")]
        public string WorkTime { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.City")]
        public string City { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.Address")]
        public string Address { get; set; }

        #endregion
    }
}
