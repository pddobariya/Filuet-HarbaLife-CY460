using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Fusion
{
    public class FusionValidationService
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly IShoppingCartProxyService _shoppingCartProxyService;
        private readonly IRepository<CustomerLimits> _customerLimitsRepository;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FusionValidationService(
            ILogger logger,
            IProductService productService,
            IDualMonthsService dualMonthsService,
            IShoppingCartProxyService shoppingCartProxyService,
            IRepository<CustomerLimits> customerLimitsRepository,
            IGenericAttributeService genericAttributeService)
        {
            _logger = logger;
            _productService = productService;
            _dualMonthsService = dualMonthsService;
            _shoppingCartProxyService = shoppingCartProxyService;
            _customerLimitsRepository = customerLimitsRepository;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task<DistributorLimitsModel> ValidateDistributorLimitsAsync(Customer customer, IList<ShoppingCartItem> shoppingCart, double volumePoints = 0)
        {
            if (volumePoints == 0 && shoppingCart != null && shoppingCart.Any())
            {
                volumePoints = (double)await shoppingCart.SumAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<decimal>(await _productService.GetProductByIdAsync(x.ProductId), ProductAttributeNames.VolumePoints));
            }
            DistributorLimitsModel distributorLimits = new DistributorLimitsModel 
            {
                DistributorLimit = DistributorLimitEnum.NotExceed
            };

            DistributorFopLimitsModel userLimits =await GetDistributorLimitsAsync(customer, true);
            if(userLimits == null)
            {
                distributorLimits.DistributorLimit = DistributorLimitEnum.Exceed;
                return distributorLimits;
            }

            double limitsDelta = 0;
            if (userLimits.InFopPeriod)
            {
                if ((volumePoints > userLimits.FopLimit && userLimits.FopLimit != -1) || (volumePoints > userLimits.PcLimit && userLimits.PcLimit != -1))
                {
                    distributorLimits.DistributorLimit = DistributorLimitEnum.FirstOrderLimitExceed;
                    if (volumePoints > userLimits.FopLimit && userLimits.FopLimit != -1)
                    {
                        limitsDelta = volumePoints - userLimits.FopLimit;
                    }
                }
            }
            else
            {
                if (volumePoints > userLimits.PcLimit && userLimits.PcLimit != -1)
                {
                    distributorLimits.DistributorLimit = DistributorLimitEnum.MonthlyLimitExceed;
                    limitsDelta = volumePoints - userLimits.PcLimit;
                }
            }

            if (!distributorLimits.IsValid)
            {
                distributorLimits.ExceedanceAmount = limitsDelta;
            }

            return distributorLimits;
        }

        public async Task<DistributorFopLimitsModel> GetDistributorLimitsAsync(Customer customer, bool fromCache = false)
        {
            var customerLimits = _customerLimitsRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);
            DistributorFopLimitsModel distributorFopLimitsModel = null;
            if ((customerLimits != null && !customerLimits.IsValidInfo) || customerLimits == null)
            {
                string cp =await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.CountryOfProcessing);
                string orderMonth =await _dualMonthsService.GetOrderMonthOfCustomerAsync(customer);
                string distributorId =await customer.GetDistributorIdAsync();

                FusionHelpers.LogFusionCall("[PERF DEBUG] GetDistributorLimits - request", new
                {
                    DistributorId = distributorId,
                    CountryOfProcessing = cp,
                    OrderMonth = orderMonth
                });

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                try 
                { 
                    distributorFopLimitsModel = _shoppingCartProxyService.GetDistributorLimits(distributorId, cp, orderMonth);
                    stopWatch.Stop();
                    FusionHelpers.LogFusionCall($"[PERF DEBUG] GetDistributorLimits - response. Process time: {stopWatch.ElapsedMilliseconds} ms.", distributorFopLimitsModel);
                }
                catch (Exception exc)
                {
                    stopWatch.Stop();
                    await _logger.ErrorAsync($"[PERF DEBUG] GetDistributorLimits error. Process time: {stopWatch.ElapsedMilliseconds} ms.", exc);
                }
                
                if (distributorFopLimitsModel == null)
                    return null;
                var res = AutoMapperConfiguration.Mapper.Map<CustomerLimits>(distributorFopLimitsModel);
                res.CustomerId = customer.Id;
                res.IsValidInfo = true;
                if (customerLimits == null)
                {
                   await _customerLimitsRepository.InsertAsync(res);
                }
                else
                {
                    res.Id = customerLimits.Id;
                    await _customerLimitsRepository.UpdateAsync(res);
                }
            }
            else
            {
                distributorFopLimitsModel =
                    AutoMapperConfiguration.Mapper.Map<DistributorFopLimitsModel>(customerLimits);
            }

            return distributorFopLimitsModel;
        }

        #endregion
    }
}
