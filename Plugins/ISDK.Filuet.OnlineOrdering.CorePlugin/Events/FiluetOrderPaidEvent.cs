using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Services.Events;
using Nop.Services.Logging;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Events
{
    public class FiluetOrderPaidEvent : IConsumer<OrderPaidEvent>
    {
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly ILogger _logger;

        public FiluetOrderPaidEvent(
            IFusionIntegrationService fusionIntegrationService,
            ILogger logger)
        {
            _fusionIntegrationService = fusionIntegrationService;
            _logger = logger;
        }

        public async Task HandleEventAsync(OrderPaidEvent eventMessage)
        {
            try
            {
                var order = eventMessage.Order;

                await _fusionIntegrationService.ReSubmitOrderAsync(order);
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, "[PERF DEBUG] FiluetOrderPaidEvent error.", ex.ToString());
            }
        }
    }
}
