using Nop.Core;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class OrderStatuses : BaseEntity
    {
        #region Properties

        public string Status { get; set; }

        public virtual IEnumerable<OrderStatusesLanguage> OrderStatusesLanguages { get; set; }

        #endregion
    }
}