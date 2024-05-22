using ISDK.Filuet.OnlineOrdering.FiluetCommon.Enums;
using Nop.Core;
using Nop.Core.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services
{
    public interface IFiluetOrderService
    {
        #region Methods

        IPagedList<Order> SearchOrders(int storeId = 0,
            int vendorId = 0, int customerId = 0,
            int productId = 0, int affiliateId = 0, int warehouseId = 0,
            int billingCountryId = 0, string paymentMethodSystemName = null,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            string billingPhone = null, string billingEmail = null, string billingLastName = "",
            string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, 
            OrderSortingEnum? orderBy = OrderSortingEnum.Position, string fusionOrderId = null);

        Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0,
                    int vendorId = 0, int customerId = 0,
                    int productId = 0, int affiliateId = 0, int warehouseId = 0,
                    int billingCountryId = 0, string paymentMethodSystemName = null,
                    DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
                    List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
                    string billingPhone = null, string billingEmail = null, string billingLastName = "",
                    string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, string containOrderNumber = null);

        Task<Order> GetOrderByAuthorizationTransactionIdAsync(string authorizationTransactionId);

        Task<Order> GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, string paymentMethodSystemName);
        
        #endregion
    }
}
