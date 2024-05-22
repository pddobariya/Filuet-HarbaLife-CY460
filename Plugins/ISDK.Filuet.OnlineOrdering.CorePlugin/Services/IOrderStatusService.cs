using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public interface IOrderStatusService
    {
        #region Methods

        Task InsertOrderStatusAsync(FiluetOrderStatus filuetOrderStatus);

        Task UpdateOrderStatusAsync(FiluetOrderStatus filuetOrderStatus);

        Task<IEnumerable<FiluetOrderStatus>> GetOrderStatusesByStatusIdsAsync(int[] statusIds);

        Task<IEnumerable<FiluetOrderStatus>> GetOrderStatusesByOrderIdsAsync(int[] orderIds);

        Task<IEnumerable<FiluetStatusLocaleString>> GetStatusLocaleStringsAsync(int languageId);

        Task<IEnumerable<FiluetOrderStatus>> GetOrderStatusesByOrderIdAsync(int orderId);

        #endregion
    }
}
