using Newtonsoft.Json;
using Nop.Core;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class OrdersStatuses : BaseEntity
    {
        #region Properties

        public DateTime? StatusDate { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        
        [ForeignKey("OrderStatus")]
        public int OrderStatusId { get; set; }
        [JsonIgnore]
        public virtual OrderStatuses OrderStatus { get; set; }

        #endregion
    }
}