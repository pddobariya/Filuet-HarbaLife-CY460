using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class SalesCenterLanguage : BaseEntity
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
