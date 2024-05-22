using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Localization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryOperatorLanguage : BaseEntity
    {
        #region Properties

        public string OperatorName { get; set; }
        [ForeignKey("DeliveryOperator")]
        public int DeliveryOperatorId { get; set; }
        [JsonIgnore]
        public virtual DeliveryOperator DeliveryOperator { get; set; }
        [ForeignKey("Language")]
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }

        #endregion
    }
}
