using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Localization;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class OrderStatusesLanguage : BaseEntity
    {
        #region Properties

        public string Status { get; set; }
        public string Comment { get; set; }

        [ForeignKey("OrderStatus")]
        public int OrderStatusId { get; set; }
        [JsonIgnore]
        public virtual OrderStatuses OrderStatus { get; set; }

        [ForeignKey("Language")]
        public int LanguageId { get; set; }

        public virtual Language Language { get; set; }

        #endregion
    }
}