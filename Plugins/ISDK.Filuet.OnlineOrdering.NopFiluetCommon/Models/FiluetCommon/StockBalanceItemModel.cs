using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Element of <see cref="StockBalanceModel.StockBalances"/>
    /// </summary>
    public class StockBalanceItemModel
    {
        #region Properties

        /// <summary>
        /// SKU
        /// </summary>
        public SkuItemModel Sku { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        public int StockQty { get; set; }

        /// <summary>
        /// Block state
        /// </summary>
        public bool IsBlocked { get; set; }
        
        /// <summary>
        /// Is Valid SKU
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Requested qty is available
        /// </summary>
        public bool IsAvailable { get; set; }


        public StockBalanceItemAvailabilityEnum StockBalanceItemAvailability { get; set; }

        #endregion
    }
}