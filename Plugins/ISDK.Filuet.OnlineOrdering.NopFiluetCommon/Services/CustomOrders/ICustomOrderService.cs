using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders
{
    public interface ICustomOrderService
    {
        #region Methods

        Task<string> GetCustomFreightCodeAsync(Order order, Customer customer);

        Task<bool> ShowCustomOrderCardMessageAsync(Customer customer = null);

        #endregion
    }
}