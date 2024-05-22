using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.DualMonth;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth
{
    public interface IDualMonthsService
    {
        #region Methods

        Task<bool> GetDualMonthAllowedAsync();

        Task<DateTime> GetOrderMonthDatetimeOfCustomerAsync(Customer customer);

        Task UpdateSelectedLimitsAsync(Customer customer, DateTime monthDate);

        Task<List<MonthModel>> GetAvailableMonthsAsync();

        Task<string> GetOrderMonthOfCustomerAsync(Customer customer);

        Task<string> GetOrderMonthOfOrderAsync(Order order);
        //add for 4.4
        Task<bool> IsMonthSelectedAsync(Customer customer);

        #endregion
    }
}
