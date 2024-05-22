using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using Nito.AsyncEx;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks
{
    public class Order1SSenderScheduleTask : IScheduleTask
    {
        #region Fields

        public const string Name = "Send order to 1S";
        public static string TaskType => typeof(Order1SSenderScheduleTask).AssemblyQualifiedName;

        private readonly ILogger _logger;
        private readonly _1SServiceWrapper _1sServiceWrapper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IProductService _productService;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private object sync = new object();

        #endregion

        #region Ctor

        public Order1SSenderScheduleTask(
            ILogger logger, 
            _1SServiceWrapper _1SServiceWrapper,
            IGenericAttributeService genericAttributeService, 
            IProductService productService,
            IRepository<GenericAttribute> genericAttributeRepository)
        {
            _logger = logger;
            _1sServiceWrapper = _1SServiceWrapper;
            _genericAttributeService = genericAttributeService;
            _productService = productService;
            _genericAttributeRepository = genericAttributeRepository;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    await _logger.InformationAsync("Starting Order1SSenderScheduleTask schedule task...");

                    lock (sync)
                    {
                        var genericAttributes = _genericAttributeRepository.Table.Where(ga =>
                                ga.KeyGroup == nameof(Order) &&
                                ga.Key == CoreGenericAttributes.OrderJsonStringAttribute)
                            .ToArray();
                     
                        foreach (var attribute in genericAttributes)
                        {
                            var sent = _1sServiceWrapper.SendOrder(attribute.Value).Result;
                            if (sent)
                            {
                                continue;
                            }
                            _genericAttributeService.DeleteAttributeAsync(attribute).Wait();
                        }
                    }
                }
                catch (Exception e)
                {
                    await _logger.ErrorAsync("Error occurred during executing Order1SSenderScheduleTask schedule task", e);
                    throw;
                }
            });
        }

        #endregion
    }
}
