using Nop.Core;
using Nop.Core.Domain.Localization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class DeliveryTypeLanguage : BaseEntity
    {
        #region Properties

        public string TypeName { get; set; }
        [ForeignKey("DeliveryType")]
        public int DeliveryTypeId { get; set; }
        public virtual DeliveryType DeliveryType { get; set; }
        [ForeignKey("Language")]
        public int LanguageId { get; set; }
        public virtual Language Language { get; set; }

        #endregion
    }
}
