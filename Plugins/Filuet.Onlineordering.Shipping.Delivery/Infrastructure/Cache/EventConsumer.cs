using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Services.Common;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Infrastructure.Cache
{
    public class EventConsumer : IConsumer<OrderPlacedEvent>
    {
        #region Fileds

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public EventConsumer(IStoreContext storeContext,
            IWorkContext workContext = null, IGenericAttributeService
            genericAttributeService = null)
        {
            _storeContext = storeContext;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            if (eventMessage?.Order == null)
                return;

            var order = eventMessage?.Order;
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            var comment = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, ShippingDetailsAttributes.ShippingDetailsCommentAttribute, currentStore.Id);
            await _genericAttributeService.SaveAttributeAsync(order, ShippingDetailsAttributes.ShippingDetailsCommentAttribute, comment, currentStore.Id);
        }

        #endregion
    }
}
