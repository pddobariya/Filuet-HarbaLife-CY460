using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks
{
    public class UpdateStockBalanceScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IStockBalanceProxyService _stockBalanceProxyService;
        private readonly IRepository<Product> _productRepository;
        private readonly ICustomProductService _customProductService;
        private readonly IRepository<StockQuantityHistory> _stockQuantityHistoryRepository;
        public const string Name = "Update Stock Balance from Fusion";
        public static string TaskType => typeof(UpdateStockBalanceScheduleTask).AssemblyQualifiedName;

        #endregion

        #region Ctor

        public UpdateStockBalanceScheduleTask(
            ILogger logger,
            IStockBalanceProxyService stockBalanceProxyService,
            IRepository<Product> productRepository,
            ICustomProductService customProductService, 
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository)
        {
            _logger = logger;
            _stockBalanceProxyService = stockBalanceProxyService;
            _productRepository = productRepository;
            _customProductService = customProductService;
            _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var fusionTime = new Stopwatch();
            var dbReadTime = new Stopwatch();
            var dbWriteTime = new Stopwatch();

            try
            {
                await _logger.InformationAsync("[PERF DEBUG] UpdateStockBalanceScheduleTask schedule task start...");

                dbReadTime.Start();

                var products = _productRepository.Table.Where(p => !p.Deleted && p.Published && p.VisibleIndividually).ToArray();
                var orderItemModels =await _customProductService.GetProductWarehousePairs(products).SelectAwait( async x =>await FusionHelpers.GetOrderItemAsync(1, x.Item1, null, null, x.Item2)).ToArrayAsync();

                dbReadTime.Stop();


                fusionTime.Start();

                StockBalanceModel stockBalanceModel = null;
                var stringBuilder = new StringBuilder("Fusion calls!" + Environment.NewLine + Environment.NewLine);
                var FusionCallsCount = 0;
                var fusionRequestTime = new Stopwatch();
                foreach (var warehouse in orderItemModels.Select(x => x.Sku.Warehouse).Distinct())
                {
                    var oim = orderItemModels.Where(x => x.Sku.Warehouse == warehouse).ToArray();
                    var initialPortion = 15;

                    var portion = oim.Length > initialPortion ? initialPortion : oim.Length;
                    var skip = 0;
                    var iteration = 0;
                    do
                    {
                        var taken =oim.Skip(skip).Take(portion);
                        fusionRequestTime.Restart();
                        var temp = await _stockBalanceProxyService.GetStockBalance(taken);
                        fusionRequestTime.Stop();
                        stringBuilder = stringBuilder.AppendLine($"Iteration[{++iteration}] for warehouse: {warehouse}. Fusion GetStockBalance batch size: {portion}. Fusion request time: {fusionRequestTime.ElapsedMilliseconds} ms.");
                        if (stockBalanceModel == null)
                        {
                            stockBalanceModel = temp;
                        }
                        else
                        {
                            stockBalanceModel.StockBalances = stockBalanceModel.StockBalances.Union(temp.StockBalances);
                        }

                        skip += portion;
                        portion = oim.Length > (initialPortion + skip) ? initialPortion : oim.Length - skip;
                    } while (portion > 0);
                    FusionCallsCount += iteration;
                }

                fusionTime.Stop();
                stringBuilder.AppendLine(Environment.NewLine + Environment.NewLine + $"Fusion calls count: {FusionCallsCount}. Fusion request time: {fusionTime.ElapsedMilliseconds} ms.");
                

                dbWriteTime.Start();
                int updatedProductsCount = 0;
                List<StockQuantityHistory> stockHistoryEntries = new List<StockQuantityHistory>();
                foreach (var product in products)
                {
                    var stockBalanceItemModel = stockBalanceModel.StockBalances.FirstOrDefault(x => x.Sku.Name == product.Sku);
                    var quantityToChange = (stockBalanceItemModel?.StockQty ?? 0) - product.StockQuantity;
                    if (quantityToChange != 0)
                    {
                        product.ManageInventoryMethod = ManageInventoryMethod.ManageStock;
                        product.StockQuantity += quantityToChange;

                        StockQuantityHistory stockHistoryEntry = new StockQuantityHistory
                        {
                            ProductId = product.Id,
                            CombinationId = null,
                            WarehouseId = (int)product.WarehouseId,
                            QuantityAdjustment = quantityToChange,
                            StockQuantity = product.StockQuantity,
                            Message = "Synced from Fusion",
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        stockHistoryEntries.Add(stockHistoryEntry);
                        updatedProductsCount++;
                    }
                }
                await _productRepository.UpdateAsync(products);
                await _stockQuantityHistoryRepository.InsertAsync(stockHistoryEntries, false);
                dbWriteTime.Stop();


                await _logger.InsertLogAsync(LogLevel.Information,
                    $"[PERF DEBUG] UpdateStockBalanceScheduleTask schedule task successfully finished. " + 
                    $"Products count: {products.Length}. Updated products count: {updatedProductsCount}. Orders count: {orderItemModels.Length}. " + 
                    $"DB read time: {dbReadTime.ElapsedMilliseconds} ms. Fusion access time: {fusionTime.ElapsedMilliseconds} ms. DB write time: {dbWriteTime.ElapsedMilliseconds} ms.",
                    stringBuilder.ToString());
            }
            catch (Exception exc)
            {
                dbReadTime.Stop();
                fusionTime.Stop();
                dbWriteTime.Stop();

                await _logger.ErrorAsync("[PERF DEBUG] Error occurred during executing UpdateStockBalanceScheduleTask schedule task. " +
                    $"DB read time: {dbReadTime.ElapsedMilliseconds} ms. Fusion access time: {fusionTime.ElapsedMilliseconds} ms. DB write time: {dbWriteTime.ElapsedMilliseconds} ms.", exc);
                throw;
            }
        }

        #endregion
    }
}
