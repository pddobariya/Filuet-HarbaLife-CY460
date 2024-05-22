namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{

    /// <summary>
    /// Used as result of getshoppingcart and parameter in submitorder
    /// </summary>
    public class ShoppingCartLineModel : OrderItemModel
    {
        #region Properties

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
