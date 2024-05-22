using Filuet.Hrbl.Ordering.Abstractions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    /// <summary>
    /// Shopping cart service
    /// </summary>        
    public interface IShoppingCartProxyService
    {
        #region Methods

        DistributorFopLimitsModel GetDistributorLimits(string distributorId, string countryCode, string orderMonth);

        /// <summary>
        /// Recalculates the distributor's shopping cart
        /// </summary>
        /// <param name="distributorId">Distributor Id</param>
        /// <param name="processingLocation"></param>
        /// <param name="warehouseCode"></param>
        /// <param name="orderMonth"></param>
        /// <param name="freightCode"></param>
        /// <param name="countryCode">Country code</param>
        /// <param name="postalCode"></param>
        /// <param name="address"></param>
        /// <param name="city"></param>
        /// <param name="orderItems">List of ordered items</param>
        /// <param name="orderNumber"></param>
        /// <param name="orderCategory"></param>
        /// <param name="orderType"></param>
        /// <returns>The shopping cart recalculation result</returns>
        Task<ShoppingCartTotalModel> GetShoppingCartTotalAsync(string distributorId, string processingLocation,
            string warehouseCode, string orderMonth, string freightCode, string countryCode,
            string postalCode, string address, string city, IEnumerable<OrderItemModel> orderItems,
            string orderNumber, string orderCategory, string orderType, string currencyCode);

        Task<SubmitOrderResultModel> SubmitOrderAsync(SubmitRequestPayment submitRequestPayment, SubmitRequestHeader submitRequestHeader, SubmitRequestOrderLine[] submitRequestOrderLines);// SubmitOrderModel submitOrderModel, out string orderJSON);
        
        #endregion
    }
}
