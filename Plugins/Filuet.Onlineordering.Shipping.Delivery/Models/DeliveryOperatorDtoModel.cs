using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record DeliveryOperatorDtoModel : BaseNopEntityModel
    {
        #region Properties

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.OperatorName")]
        public string OperatorName { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.FreightCode")]
        public string FreightCode { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.Models.WarehouseCode")]
        public string WarehouseCode { get; set; }

        #endregion
    }
}
