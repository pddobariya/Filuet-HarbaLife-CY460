using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface IFiluetShippingCartService
    {
        #region Methods

        Task<string[]> IsCartValid();

        Task<string[]> IsCartValid(Customer currentCustomer, List<ShoppingCartItem> cart, (double monthLimit, double oneOrderLimit) limits);

        #endregion
    }
}
