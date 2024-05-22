using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion
{
    public interface IFusionIntegrationService
    {
        #region Methods

        Task<ShoppingCartTotalModel> GetShoppingCartTotalAsync(Customer customer, IEnumerable<ShoppingCartItem> cartItems = null);

        Task<ShoppingCartTotalModel> GetShoppingCartTotalAsync(Customer customer, Order order);

        Task SaveOrderToFusionAsync(Order savedOrder, ShoppingCartTotalModel cartTotal);

        Task<ShoppingCartTotalModel> GetShoppingCartTotalOffline(Customer customer, IEnumerable<ShoppingCartItem> cartItems = null);

        Task ReSubmitOrderAsync(Order order);

        #endregion
    }
}
