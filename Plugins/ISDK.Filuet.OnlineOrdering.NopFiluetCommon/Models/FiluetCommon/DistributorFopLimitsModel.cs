namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    public class DistributorFopLimitsModel
    {
        #region Properties

        public bool InFopPeriod { get; set; }

        /// <summary>
        /// Value or -1
        /// </summary>
        public double PcLimit { get; set; }

        /// <summary>
        /// Value or -1
        /// </summary>
        public double FopLimit { get; set; }

        #endregion
    }
}
