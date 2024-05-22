namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    // TODO: the same as OrderPriceLine
    public class OrderLine
    {
        #region Properties

        public string SkuName { get; set; }

        public int OrderedQty { get; set; }

        public decimal? LineAmount { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? Earnbase { get; set; }

        public decimal? TotalEarnbase { get; set; }

        public decimal? UnitVolume { get; set; }

        public decimal? TotalRetailPrice { get; set; }

        public decimal? TotalDiscountedPrice { get; set; }

        #endregion
    }
}
