using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.DualMonth;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth
{
    public class DualMonthsService : IDualMonthsService
    {
        #region Fields 

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public DualMonthsService(
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            ISettingService settingService,
            IStoreContext storeContext,
            ICustomerService customerService)
        {
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _settingService = settingService;
            _storeContext = storeContext;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public virtual async Task<bool> GetDualMonthAllowedAsync()
        {
            var currentDate = await GetCurrentDateTimeAsync();
            var settings = _settingService.LoadSetting<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());

            return settings.StartDate <= currentDate.Date && currentDate.Date < settings.EndDate.Value.AddDays(1).Date;
        }

        public virtual async Task<DateTime> GetOrderMonthDatetimeOfCustomerAsync(Customer customer)
        {
            var currentDate = new DateTime((await GetCurrentDateTimeAsync()).Year, (await GetCurrentDateTimeAsync()).Month, 1);
            bool allowMonthSelect = await GetDualMonthAllowedAsync();

            if (!allowMonthSelect)
            {
                await UpdateSelectedLimitsAsync(customer, currentDate);
                return currentDate;
            }

             int? selectedLimitsMonth = await _genericAttributeService.GetAttributeAsync<int?>(customer, CustomerAttributeNames.SelectedLimitsMonth);
            int? selectedLimitsYear = await _genericAttributeService.GetAttributeAsync<int?>(customer, CustomerAttributeNames.SelectedLimitsYear);

            DateTime selectedMonthDate = DateTime.MinValue;

            if (selectedLimitsMonth.HasValue && selectedLimitsYear.HasValue)
            {
                selectedMonthDate = new DateTime(selectedLimitsYear.Value, selectedLimitsMonth.Value, 1);
                if (selectedMonthDate < currentDate.AddMonths(-1) || selectedMonthDate > currentDate.AddMonths(1))
                {
                    selectedMonthDate = DateTime.MinValue;
                    RemoveSelectedLimits(customer);
                }
            }

            if (selectedMonthDate == DateTime.MinValue)
            {
                int currentDayOfMonth = (await GetCurrentDateTimeAsync()).Day;
                int lastDayOfThisMonth = currentDate.AddMonths(1).AddDays(-1).Day;
                DateTime prevMonth = currentDate.AddMonths(-1);
                selectedMonthDate = (lastDayOfThisMonth - currentDayOfMonth < currentDayOfMonth) ? currentDate : prevMonth;

                await UpdateSelectedLimitsAsync(customer, selectedMonthDate);
            }

            return new DateTime(selectedMonthDate.Year, selectedMonthDate.Month, 1);
        }

        public async Task<string> GetOrderMonthOfOrderAsync(Order order)
        {
            string orderMonthStr = await _genericAttributeService.GetAttributeAsync<string>(order,OrderAttributeNames.OrderMonth);
            if (string.IsNullOrEmpty(orderMonthStr))
            {
                orderMonthStr = await GetOrderMonthOfCustomerAsync(await _customerService.GetCustomerByIdAsync(order.CustomerId));
            }
            return orderMonthStr;
        }

        public async Task<string> GetOrderMonthOfCustomerAsync(Customer customer)
        {
            return (await GetOrderMonthDatetimeOfCustomerAsync(customer)).ToString("yy.MM");
        }

        public async Task UpdateSelectedLimitsAsync(Customer customer, DateTime monthDate)
        {
            await _genericAttributeService.SaveAttributeAsync(customer, FiluetCoreDefault.SelectedLimitsMonth, monthDate.Month);
            await _genericAttributeService.SaveAttributeAsync(customer, FiluetCoreDefault.SelectedLimitsYear, monthDate.Year);
        }

        public async Task<List<MonthModel>> GetAvailableMonthsAsync()
        {
            var availableMonths = new List<MonthModel>();
            var orderMonth = await GetOrderMonthDatetimeOfCustomerAsync(await _workContext.GetCurrentCustomerAsync());

            // currentMonth
            DateTime currentMonthDate = new DateTime((await GetCurrentDateTimeAsync()).Year, (await GetCurrentDateTimeAsync()).Month, 1);
            var currentMonth = new MonthModel()
            {
                Timestamp = currentMonthDate.ToUnixTimestamp(),
                DisplayName = currentMonthDate.ToString("MMMM", await LocalizationHelpers.GetCurrentCultureDataAsync()).ToCapitalCase(),
                IsSelected = orderMonth.Year == currentMonthDate.Year && orderMonth.Month == currentMonthDate.Month
            };
            availableMonths.Add(currentMonth);

            // prevMonth
            DateTime prevMonthDate = currentMonthDate.AddMonths(-1);
            var prevMonth = new MonthModel()
            {
                Timestamp = prevMonthDate.ToUnixTimestamp(),
                DisplayName = prevMonthDate.ToString("MMMM",await LocalizationHelpers.GetCurrentCultureDataAsync()).ToCapitalCase(),
                IsSelected = orderMonth.Year == prevMonthDate.Year && orderMonth.Month == prevMonthDate.Month
            };
            availableMonths.Add(prevMonth);

            return availableMonths;
        }

        private async void RemoveSelectedLimits(Customer customer)
        {
            var attr = (await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, nameof(Customer))).FirstOrDefault(attr => attr.Key == CustomerAttributeNames.SelectedLimitsMonth);
            if (attr != null)
            {
                await _genericAttributeService.DeleteAttributeAsync(attr);
            }
         
            attr = (await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, nameof(Customer))).FirstOrDefault(attr => attr.Key == CustomerAttributeNames.SelectedLimitsYear);
            if (attr != null)
            {
                await _genericAttributeService.DeleteAttributeAsync(attr);
            }
        }

        private async Task<DateTime> GetCurrentDateTimeAsync()
        {
            var settings = _settingService.LoadSetting<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            return DateTime.UtcNow.AddHours(settings.HoursShift);
        }

        //add for 4.4

        public async Task<bool> IsMonthSelectedAsync(Customer customer)
        {
            if (!await GetDualMonthAllowedAsync())
                return true;
            var availableMonths =(await GetAvailableMonthsAsync()).First(m => m.IsSelected).DisplayName;
            var month = DateTime.ParseExact(availableMonths, "MMMM",await LocalizationHelpers.GetCurrentCultureDataAsync()).Month;
            var selesctedMonth =await _genericAttributeService.GetAttributeAsync<int>(customer,CustomerAttributeNames.SelectedLimitsMonth);
            return month == selesctedMonth;
        }

        #endregion
    }
}
