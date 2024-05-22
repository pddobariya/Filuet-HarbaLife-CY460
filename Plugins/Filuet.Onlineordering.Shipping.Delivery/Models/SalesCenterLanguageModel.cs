using Nop.Web.Framework.Models;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record SalesCenterLanguageModel : BaseNopEntityModel
    {
        #region Properties

        public int SalesCenterId { get; set; }
        public int LanguageId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string WorkTime { get; set; }

        #endregion
    }
}

