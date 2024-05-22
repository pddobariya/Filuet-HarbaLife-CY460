using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    #region Properties

    /// <summary>
    /// Distributor limits validation result
    /// </summary>
    [KnownType(typeof(DistributorLimitsWithBalanceModel))]
    public class DistributorLimitsModel
    {
        /// <summary>
        /// Distributor limit
        /// </summary>
        public DistributorLimitEnum DistributorLimit { get; set; }

        public double? ExceedanceAmount { get; set; } = null;

        public bool IsValid {
            get
            {
                return DistributorLimit == DistributorLimitEnum.NotExceed || DistributorLimit == DistributorLimitEnum.NoLimits;
            }
        }
    }


    public class DistributorLimitsWithBalanceModel : DistributorLimitsModel
    {
        public IEnumerable<SkuItemModel> ExceedItems { get; set; }
    }

    #endregion
}
