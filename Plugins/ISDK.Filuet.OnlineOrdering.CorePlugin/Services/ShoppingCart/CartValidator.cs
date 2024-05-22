using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.ShoppingCart
{
    public class CartValidator : ICartValidator
    {
        #region Ctor

        public CartValidator()
        {         
        }

        #endregion

        #region Methods

        public async Task<bool> IsValidCustomerAttributes(Customer currentCustomer, int storeId)
        {            
            return await Task.FromResult(true);
        }

        public async Task<bool> IsValidFormAsync(IFormCollection form)
        {
            return await Task.FromResult(true);
        }

        #endregion
    }
}
