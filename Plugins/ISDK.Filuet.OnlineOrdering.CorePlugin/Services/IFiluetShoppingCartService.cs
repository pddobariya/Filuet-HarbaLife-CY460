using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public interface IFiluetShoppingCartService
    {
        #region Methods

        Task<string[]> IsCartValid();
        Task<string[]> IsCartValid(Customer currentCustomer, List<ShoppingCartItem> cart, (double monthLimit, double oneOrderLimit) limits);

        #endregion
    }
}