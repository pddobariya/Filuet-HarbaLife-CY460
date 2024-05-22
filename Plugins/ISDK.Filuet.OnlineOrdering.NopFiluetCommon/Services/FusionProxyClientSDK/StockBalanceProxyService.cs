//#define LOCAL_DEBUG

using Filuet.Hrbl.Ordering.Abstractions;
using Filuet.Hrbl.Ordering.Adapter;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Configuration;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using ProxyServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    public class StockBalanceProxyService : ClientBase<IFusionServiceProxy>, IStockBalanceProxyService
    {
        #region Fields

        private IHerbalifeEnvironment _herbalifeEnvironment;
        private static readonly IHrblOrderingAdapter _hrblOrderingAdapter;

        #endregion

        #region Ctor

        static StockBalanceProxyService()
        {
            _hrblOrderingAdapter = ConnectionBuilder.GetRestApiAdapter();
        }

        public StockBalanceProxyService(FiluetConfig filuetConfig)
            : base(ConnectionBuilder.GetBinding(filuetConfig.FusionProxyConnectionProtocol),
                  ConnectionBuilder.GetEndpointAddress(filuetConfig.FusionProxyConnectionAddress))
        {
            this.ClientCredentials.ServiceCertificate.SslCertificateAuthentication =
                ConnectionBuilder.GetSslCertificateAuthentication(false);

            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                _herbalifeEnvironment = serviceScope.ServiceProvider.GetService<IHerbalifeEnvironment>();
            }
        }

        #endregion

        #region Methods

        public async Task<StockBalanceModel> GetStockBalance(IEnumerable<OrderItemModel> skuList)
        {
            if (!skuList.Any())
            {
                return new StockBalanceModel { StockBalances = Array.Empty<StockBalanceItemModel>() };
            }

            return new StockBalanceModel
            {
                StockBalances =(await _hrblOrderingAdapter.GetSkuAvailability(skuList.First().Sku.Warehouse,
                    skuList.ToDictionary(oim => oim.Sku.Name, oim => oim.Count))).Select(si => new StockBalanceItemModel
                    {
                        Sku = new SkuItemModel { Name = si.Sku, Warehouse = skuList.FirstOrDefault()?.Sku.Warehouse },
                        IsAvailable = si.IsSkuAvailable,
                        IsBlocked = !si.IsSkuValid,
                        IsValid = si.IsSkuValid,
                        StockBalanceItemAvailability = Enums.StockBalanceItemAvailabilityEnum.Unknown,
                        StockQty = (int)si.AvailableQuantity
                    }).ToArray()
            };
        }

        #endregion
    }
}
