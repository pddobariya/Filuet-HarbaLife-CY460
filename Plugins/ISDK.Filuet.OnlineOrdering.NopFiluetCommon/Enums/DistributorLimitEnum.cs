namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums
{
    /// <summary>
    /// Distributor limit enum
    /// </summary>
    public enum DistributorLimitEnum
    {
        /// <summary>
        /// Limit has not been exceed
        /// </summary>
        NotExceed = 0,

        /// <summary>
        /// The first order limit has been exceed
        /// </summary>
        FirstOrderLimitExceed = 1,

        /// <summary>
        /// Monthly limit has been exceed
        /// </summary>
        MonthlyLimitExceed = 2,

        /// <summary>
        /// 
        /// </summary>
        Exceed = 3,

        /// <summary>
        /// 
        /// </summary>
        NoLimits = 4,


        VolumeAllowanceExceed = 5,
    }

}
