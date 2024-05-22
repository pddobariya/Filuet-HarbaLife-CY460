using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks
{
    public class Get1SStockBalanceScheduleTask : IScheduleTask
    {
        #region Fields

        public const string Name = "Gets StockBalance of products";
        public static string TaskType => typeof(Get1SStockBalanceScheduleTask).AssemblyQualifiedName;

        private readonly ILogger _logger;
        private readonly _1SServiceWrapper _1sServiceWrapper;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public Get1SStockBalanceScheduleTask(
            ILogger logger,
            _1SServiceWrapper _1SServiceWrapper,
            IProductService productService)
        {
            _logger = logger;
            _1sServiceWrapper = _1SServiceWrapper;
            _productService = productService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            try
            {
                await _logger.InformationAsync("Starting Get1SStockBalanceScheduleTask schedule task...");

                foreach (var stockBalanceItemDto in await _1sServiceWrapper.GetStocks())
                {
                    var productBySku = await _productService.GetProductBySkuAsync(stockBalanceItemDto.Sku);
                    if (productBySku != null)
                    {
                        int quantity = 0;
                        try
                        {
                            quantity = int.Parse(stockBalanceItemDto.Quantity);
                        }
                        catch (Exception e)
                        {
                            await _logger.ErrorAsync("Error occurred during executing Get1SStockBalanceScheduleTask schedule task", e);
                        }

                        productBySku.StockQuantity = quantity >= 0 ? quantity : 0;
                        productBySku.ManageInventoryMethodId = 1;
                        await _productService.UpdateProductAsync(productBySku);
                    }
                }
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Error occurred during executing Get1SStockBalanceScheduleTask schedule task", e);
                throw;
            }
        }

        #endregion
    }
}
