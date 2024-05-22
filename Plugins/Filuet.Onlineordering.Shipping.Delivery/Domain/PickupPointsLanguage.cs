using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Localization;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class PickupPointsLanguage : BaseEntity
    {
        #region Properties

        public int PickupPointsCostId { get; set; }

        public int LanguageId { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string Comment { get; set; }

        [JsonIgnore]
        public virtual PickupPointsCost PickupPointsCost { get; set; }

        public virtual Language Language { get; set; }

        #endregion
    }
}
