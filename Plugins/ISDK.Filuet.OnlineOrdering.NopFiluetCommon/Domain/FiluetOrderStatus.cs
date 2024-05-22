using Nop.Core;
using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class FiluetOrderStatus : BaseEntity
    {
        #region Properties
        public int OrderId { get; set; }

        public int StatusId { get; set; }

        public DateTime? StatusDate { get; set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime UpdateOnUtc { get; set; }

        #endregion
    }
}
