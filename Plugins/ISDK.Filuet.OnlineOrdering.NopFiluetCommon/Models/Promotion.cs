using Nop.Core.Domain.Orders;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    class Promotion
    {
        #region Properties

        public string Name { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public Func<Order, Task<bool>> PromotionAction { get; set; }

        #endregion

    }
}
