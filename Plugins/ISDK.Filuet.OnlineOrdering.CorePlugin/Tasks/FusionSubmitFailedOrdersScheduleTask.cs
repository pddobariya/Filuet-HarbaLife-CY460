using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks
{
    public class FusionSubmitFailedOrdersScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IGenericAttributeService _genericAttributeService;        
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly IPdfService _pdfService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly ICustomerService _customerService;
        private readonly OrderSettings _orderSettings;
        private readonly IWorkContext _workContext;

        public const string Name = "Resubmits orders that failed submission to Fusion";

        public static string TaskType => typeof(FusionSubmitFailedOrdersScheduleTask).AssemblyQualifiedName;

        #endregion

        #region Ctor

        public FusionSubmitFailedOrdersScheduleTask(
            IOrderService orderService,
            IGenericAttributeService genericAttributeService,
            ILogger logger, 
            IOrderProcessingService orderProcessingService,
            IFusionIntegrationService fusionIntegrationService,
            IPdfService pdfService,
            IWorkflowMessageService workflowMessageService, 
            IRepository<GenericAttribute> genericAttributeRepository, 
            ICustomerService customerService,
            OrderSettings orderSettings,
            IWorkContext workContext)
        {
            _logger = logger;
            _orderService = orderService;
            _genericAttributeService = genericAttributeService;
            _orderProcessingService = orderProcessingService;
            _fusionIntegrationService = fusionIntegrationService;
            _pdfService = pdfService;
            _workflowMessageService = workflowMessageService;
            _genericAttributeRepository = genericAttributeRepository;
            _customerService = customerService;
            _orderSettings = orderSettings;
            _workContext = workContext;
        }

        #endregion

        #region Methods


        public async Task ExecuteAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                await _logger.InformationAsync("[PERF DEBUG] FusionSubmitFailedOrdersScheduleTask schedule task start...");

                IList<GenericAttribute> genericAttributes = await
                    _genericAttributeRepository.GetAllAsync(q => q.Where(ga => ga.KeyGroup == nameof(Order) && ga.Key == OrderAttributeNames.IsFusionSubmitOrderSuccess && ga.Value == "false"));
                   
                var failedOrders = await _orderService.GetOrdersByIdsAsync(genericAttributes.Select(p => p.EntityId).ToArray());

                var failedOrderIdsAfterSubmit = new List<int>();
                var processedOrderIds = new List<int>();

                foreach (Order failedOrder in failedOrders)
                {
                    var orderNumber = failedOrder.CustomOrderNumber.Split(".");
                    if (orderNumber.Length < 2 || string.IsNullOrWhiteSpace(orderNumber[1]))
                        continue;

                    var fusionFalse = _genericAttributeRepository.Table.Where(ga => ga.KeyGroup == nameof(Order) &&
                            ga.Key == OrderAttributeNames.IsFusionSubmitOrderSuccess && ga.Value == "true"
                            && ga.EntityId == failedOrder.Id).FirstOrDefault();

                    if (fusionFalse != null)
                    {
                        var gaRemove = _genericAttributeRepository.Table.Where(ga => ga.KeyGroup == nameof(Order) &&
                              ga.Key == OrderAttributeNames.IsFusionSubmitOrderSuccess && ga.Value == "false"
                              && ga.EntityId == failedOrder.Id).FirstOrDefault();

                        await _genericAttributeRepository.DeleteAsync(gaRemove);

                        continue;
                    }

                    if (_orderProcessingService.CanMarkOrderAsPaid(failedOrder))
                    {
                        await _orderProcessingService.MarkOrderAsPaidAsync(failedOrder);
                        processedOrderIds.Add(failedOrder.Id);
                    }
                    else
                    {
                        
                        var cartTotal = await _fusionIntegrationService.GetShoppingCartTotalAsync(await _customerService.GetCustomerByIdAsync(failedOrder.CustomerId), failedOrder);
                        await _fusionIntegrationService.SaveOrderToFusionAsync(failedOrder, cartTotal);
                        await _orderProcessingService.CheckOrderStatusAsync(failedOrder);
                        await using var stream = new MemoryStream();
                         await _pdfService.PrintOrderToPdfAsync(stream, failedOrder, _orderSettings.GeneratePdfInvoiceInCustomerLanguage ? null : await _workContext.GetWorkingLanguageAsync(), store: null, vendor: await _workContext.GetCurrentVendorAsync());
                        var orderCompletedAttachmentFilePath = _orderSettings.AttachPdfInvoiceToOrderCompletedEmail ?
                        await _pdfService.SaveOrderPdfToDiskAsync(failedOrder) : null;
                        var orderCompletedAttachmentFileName = "order.pdf";
                        await _workflowMessageService.SendOrderCompletedCustomerNotificationAsync(failedOrder, failedOrder.CustomerLanguageId, orderCompletedAttachmentFilePath,
                                orderCompletedAttachmentFileName);
                    }
                }

                stopwatch.Stop();
                if (!failedOrderIdsAfterSubmit.Any())
                {
                    await _logger.InsertLogAsync(LogLevel.Information, 
                        $"[PERF DEBUG] FusionSubmitFailedOrdersScheduleTask schedule task successfully finished for the following orders. " +
                        $"Number of orders: {failedOrders.Count}. Processed order ids: {string.Join(",", processedOrderIds)}. Task processing time: {stopwatch.ElapsedMilliseconds} ms.");
                }
                else
                {
                    await _logger.InsertLogAsync(LogLevel.Information,
                        $"[PERF DEBUG] FusionSubmitFailedOrdersScheduleTask schedule task has some orders failed. " +
                        $"Number of orders: {failedOrders.Count}. Failed order ids: {string.Join(",", failedOrderIdsAfterSubmit)}. Task processing time: {stopwatch.ElapsedMilliseconds} ms.");
                }
            }
            catch (Exception exc)
            {
                stopwatch.Stop();
                await _logger.ErrorAsync("[PERF DEBUG] Error occurred during executing FusionSubmitFailedOrdersScheduleTask schedule task. " +
                    $"Task processing time: {stopwatch.ElapsedMilliseconds} ms.", exc);
                throw;
            }
        }

        #endregion
    }
}
