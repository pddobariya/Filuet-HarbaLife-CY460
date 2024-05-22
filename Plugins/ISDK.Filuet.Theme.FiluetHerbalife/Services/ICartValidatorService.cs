using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Services
{
    public interface ICartValidatorService
    {
        #region Methods

        public Task<bool> IsValidFormAsync(IFormCollection form);
        public Task<bool> IsValidCustomerAttributes(Customer currentCustomer, int storeId);

        #endregion
    }
}
