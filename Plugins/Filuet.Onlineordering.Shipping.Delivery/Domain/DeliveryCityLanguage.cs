using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryCityLanguage : BaseEntity
    {
        #region Properties

        public string CityName { get; set; }
        public int DeliveryCityId { get; set; }
        public int LanguageId { get; set; }

        #endregion
    }
}
