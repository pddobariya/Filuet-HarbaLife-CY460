using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.ShoppingCart
{
    public interface ICartValidator
    {
        #region Methods

        Task<bool> IsValidCustomerAttributes(Customer currentCustomer, int storeId);
        Task<bool> IsValidFormAsync(IFormCollection form);

        #endregion
    }
}
