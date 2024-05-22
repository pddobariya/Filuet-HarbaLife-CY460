using Nop.Core.Domain.Catalog;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Order item from the shopping cart
    /// </summary>
    public class OrderItemModel
    {
        #region Properties

        /// <summary>
        /// Stock number
        /// </summary>
        public SkuItemModel Sku { get; set; }

        /// <summary>
        /// Item count
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Get ProductType
        /// </summary>
        public ProductType ProductType { get; set; }

        #endregion

    }
}
