using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OrderStatusPlugin.Constants;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Events;
using Nop.Services.Logging;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OrderStatusPlugin.Services
{
    public class OrderPaidEventConsumer : IConsumer<OrderPaidEvent>
    {
        #region Fields

        private readonly IOrderStatusService _orderStatusService;
        private readonly IStatusService _statusService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public OrderPaidEventConsumer(
            IOrderStatusService orderStatusService, 
            IStatusService statusService,
            ILogger logger)
        {
            _orderStatusService = orderStatusService;
            _statusService = statusService;
            _logger = logger;
        }

        #endregion

        #region Method

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            var order = eventMessage.Order;
            if (order.PaymentStatus != PaymentStatus.Paid)
            {
                return;
            }

            try
            {
                var paidStatus = await _statusService.GetStatusByNameAsync(FiluetStatusContants.Paid);
                if (paidStatus == null)
                {
                    return;
                }

                var newOrderSatus = new FiluetOrderStatus
                {
                    OrderId = order.Id,
                    StatusId = paidStatus.Id,
                    StatusDate = order.PaidDateUtc
                };

                await _orderStatusService.InsertOrderStatusAsync(newOrderSatus);
            }
            catch (Exception exception)
            {
               await _logger.ErrorAsync($"[OrderStatusPlugin].[OrderPaidEventConsumer] Error. OrderId: {order.Id}", exception);
            }
        }

        #endregion
    }
}
