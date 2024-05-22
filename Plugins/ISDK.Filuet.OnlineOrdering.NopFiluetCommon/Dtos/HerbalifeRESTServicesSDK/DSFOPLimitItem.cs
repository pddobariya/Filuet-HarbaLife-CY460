namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    using System;

    public class DSFOPLimitItem
    {
        #region Properties

        public string EarnedFOP { get; set; }

        public decimal? AvailableFOPLimit { get; set; }

        public decimal? ThresholdFOPLimit { get; set; }

        public DateTime? FOPFirstOrderDate { get; set; }

        public int? FOPThresholdPeriod { get; set; }

        #endregion
    }
}
