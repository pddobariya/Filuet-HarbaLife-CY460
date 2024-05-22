using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OrderStatusPlugin.Constants;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.OrderStatusPlugin.Tasks
{
    public class LoadOrderStatusesTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly _1SServiceWrapper __1sServiceWrapper;
        private readonly IStatusService _statusService;
        public const string Name = "Load order statuses from 1C";
        public static string TaskType => typeof(LoadOrderStatusesTask).AssemblyQualifiedName;

        #endregion

        #region Ctor

        public LoadOrderStatusesTask(
            ILogger logger,
            IOrderStatusService orderStatusService,
            IRepository<GenericAttribute> genericAttributeRepository,
            _1SServiceWrapper _1sServiceWrapper,
            IStatusService statusService)
        {
            _logger = logger;
            _orderStatusService = orderStatusService;
            _genericAttributeRepository = genericAttributeRepository;
            __1sServiceWrapper = _1sServiceWrapper;
            _statusService = statusService;
        }

        #endregion

        #region Method

        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            try
            {
                var statuses = await _statusService.GetStatusesAsync(query =>
                {
                    query = query.Where(p => p.ExternalStatusName == FiluetStatusContants.Paid
                        || p.ExternalStatusName == FiluetStatusContants.TransferredToProcessing
                        || p.ExternalStatusName == FiluetStatusContants.TransferredToDelivery);

                    return query;
                });

                var orderStatuses = await _orderStatusService.GetOrderStatusesByStatusIdsAsync(statuses.Select(p => p.Id).ToArray());
                var orderIds = orderStatuses.Select(p => p.OrderId).ToArray();
                var fusionOrderNumberAttributes = await _genericAttributeRepository.Table
                    .Where(p => p.KeyGroup == nameof(Order)
                        && p.Key == OrderAttributeNames.FusionOrderNumber
                        && orderIds.Contains(p.EntityId))
                    .ToListAsync();

                var orderIdsMapping = new Dictionary<string, int>();
                var fusionOrderNumbers = new List<string>();
                foreach (var fusionOrderNumberAttribute in fusionOrderNumberAttributes)
                {
                    fusionOrderNumbers.Add(fusionOrderNumberAttribute.Value);
                    orderIdsMapping.Add(fusionOrderNumberAttribute.Value, fusionOrderNumberAttribute.EntityId);
                }
                 var orderStatusDtos = await __1sServiceWrapper.GetOrderStatuses(string.Join(',', fusionOrderNumbers));
                foreach (var orderStatusDto in orderStatusDtos)
                {
                    if (string.IsNullOrWhiteSpace(orderStatusDto.OrderNum))
                    {
                        continue;
                    }

                    if (!orderIdsMapping.ContainsKey(orderStatusDto.OrderNum))
                    {
                        continue;
                    }

                    var statuse = statuses.FirstOrDefault(p => p.ExternalStatusName == orderStatusDto.Status);
                    if (statuse == null)
                    {
                        continue;
                    }

                    var newOrderStatuse = new FiluetOrderStatus
                    {
                        OrderId = orderIdsMapping[orderStatusDto.OrderNum],
                        StatusDate = orderStatusDto.StatusDate
                    };

                    await _orderStatusService.InsertOrderStatusAsync(newOrderStatuse);
                }
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync("Error occurred during executing LoadOrderStatusesTask schedule task", exc);
                throw;
            }
        }

        #endregion
    }
}
