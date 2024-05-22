using Nop.Core;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class CustomerLimits : BaseEntity
    {
        #region Properties

        public int CustomerId { get; set; }

        public bool InFopPeriod { get; set; }

        public decimal PcLimit { get; set; }

        public decimal FopLimit { get; set; }

        public bool IsValidInfo { get; set; }

        #endregion
    }
}
