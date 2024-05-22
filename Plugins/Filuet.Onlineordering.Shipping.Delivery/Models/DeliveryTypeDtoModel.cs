using Nop.Web.Framework.Models;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record DeliveryTypeDtoModel : BaseNopEntityModel
    {
        #region Properties

        public string TypeName { get; set; }
        public string SystemType { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }

        #endregion
    }
}
