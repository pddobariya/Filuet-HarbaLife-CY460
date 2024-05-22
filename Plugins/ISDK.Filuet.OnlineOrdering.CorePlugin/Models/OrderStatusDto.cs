using System;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models
{
    public class OrderStatusDto
    {
        #region Properties
        public string Status { get; set; }

        public DateTime? StatusDate { get; set; }

        public string OrderNum { get; set; }

        public string OrderStatusClass { get; set; }

        #endregion
    }
}
