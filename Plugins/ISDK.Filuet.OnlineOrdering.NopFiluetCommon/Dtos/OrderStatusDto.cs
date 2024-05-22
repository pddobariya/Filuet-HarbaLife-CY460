using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos
{
    public class OrderStatusDto
    {
        #region Properties

        public string Status { get; set; }

        public DateTime? StatusDate { get; set; }

        public string OrderNum { get; set; }

        #endregion
    }
}
