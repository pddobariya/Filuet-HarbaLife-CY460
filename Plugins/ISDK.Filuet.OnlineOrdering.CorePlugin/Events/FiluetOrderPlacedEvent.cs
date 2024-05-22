using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Events
{
    public class FiluetOrderPlacedEvent : IConsumer<OrderPlacedEvent>
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService; 
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FiluetOrderPlacedEvent(
            ICustomerService customerService,
            ILogger logger,
            IOrderService orderService,
            IGenericAttributeService genericAttributeService)
        {
            _customerService = customerService;
            _logger = logger;
            _orderService = orderService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            var order = eventMessage.Order;

            try
            {                
                var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                order.OrderShippingInclTax =await _genericAttributeService.GetAttributeAsync<decimal>(customer,CustomerAttributeNames.DeliveryPrice);
                await _orderService.UpdateOrderAsync(order);
            }
            catch (Exception exp)
            {
                await _logger.ErrorAsync($"[FiluetOrderPlacedEvent] orderId={order.Id}", exp);
            }
        }

        #endregion
    }
}
