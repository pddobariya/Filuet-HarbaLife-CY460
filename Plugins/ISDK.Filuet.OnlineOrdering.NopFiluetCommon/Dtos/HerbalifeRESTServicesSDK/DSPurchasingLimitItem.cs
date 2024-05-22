namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class DSPurchasingLimitItem
    {
        #region Properties

        public string PPVOrderMonth { get; set; }

        public string EarnedPC { get; set; }

        public decimal? AvailablePCLimit { get; set; }

        public decimal? ThresholdPCLimit { get; set; }

        public string EarnedAI { get; set; }

        public decimal? AvailableAILimit { get; set; }

        public decimal? ThresholdAILimit { get; set; }

        #endregion
    }
}
