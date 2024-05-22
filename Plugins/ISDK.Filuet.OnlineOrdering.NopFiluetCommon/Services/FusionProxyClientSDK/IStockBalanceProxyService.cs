using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    /// <summary>
    /// Stock balance service
    /// </summary>    
    public interface IStockBalanceProxyService
    {
        #region Methods

        /// <summary>
        /// Gets information about balance for the selected stock
        /// </summary>
        /// <param name="skuList">SKU list of the selected stock</param>
        /// <returns>The selected stock balance</returns>
        Task<StockBalanceModel> GetStockBalance(IEnumerable<OrderItemModel> skuList);

        #endregion
    }
}
