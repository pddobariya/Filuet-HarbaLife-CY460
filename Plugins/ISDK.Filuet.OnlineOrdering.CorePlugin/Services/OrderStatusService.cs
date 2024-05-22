using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using LinqToDB;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        #region Fields

        private readonly IRepository<FiluetOrderStatus> _filuetOrderStatusRepository;
        private readonly IRepository<FiluetStatusLocaleString> _filuetStatusLocaleStringRepository;

        #endregion

        #region Ctor

        public OrderStatusService(
            IRepository<FiluetOrderStatus> filuetOrderStatusRepository,
            IRepository<FiluetStatusLocaleString> filuetStatusLocaleStringRepository)
        {
            _filuetOrderStatusRepository = filuetOrderStatusRepository;
            _filuetStatusLocaleStringRepository = filuetStatusLocaleStringRepository;
        }

        #endregion

        #region Methods

        public async Task InsertOrderStatusAsync(FiluetOrderStatus filuetOrderStatus)
        {
            filuetOrderStatus.CreatedOnUtc = DateTime.UtcNow;
            filuetOrderStatus.UpdateOnUtc = DateTime.UtcNow;
            await _filuetOrderStatusRepository.InsertAsync(filuetOrderStatus);
        }

        public async Task UpdateOrderStatusAsync(FiluetOrderStatus filuetOrderStatus)
        {
            filuetOrderStatus.UpdateOnUtc = DateTime.UtcNow;
            await _filuetOrderStatusRepository.UpdateAsync(filuetOrderStatus);
        }

        public async Task<IEnumerable<FiluetOrderStatus>> GetOrderStatusesByStatusIdsAsync(int[] statusIds)
        {
            return await _filuetOrderStatusRepository.Table
                .Where(p => statusIds.Contains(p.StatusId))
                .ToArrayAsync();
        }

        public async Task<IEnumerable<FiluetOrderStatus>> GetOrderStatusesByOrderIdsAsync(int[] orderIds)
        {
            return await _filuetOrderStatusRepository.Table
                .Where(p => orderIds.Contains(p.OrderId))
                .ToArrayAsync();
        }

        public async Task<IEnumerable<FiluetStatusLocaleString>> GetStatusLocaleStringsAsync(int languageId)
        {
            return await _filuetStatusLocaleStringRepository.Table
                .Where(p => p.LanguageId == languageId)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<FiluetOrderStatus>> GetOrderStatusesByOrderIdAsync(int orderId)
        {
            return await _filuetOrderStatusRepository.Table
                .Where(p => p.OrderId == orderId)
                .ToArrayAsync();
        }

        #endregion
    }
}
