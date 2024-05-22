using ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.ShoppingCart;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Components
{
    public class FiluetOrderTotalsViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IFusionIntegrationService _fusionIntegrationService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly IDistributorService _distributorService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger _logger;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FiluetOrderTotalsViewComponent(
            IShoppingCartModelFactory shoppingCartModelFactory,
            IWorkContext workContext,
            IFusionIntegrationService fusionIntegrationService,
            IDualMonthsService dualMonthsService,
            IDistributorService distributorService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            ILogger logger,
            IGenericAttributeService genericAttributeService)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _workContext = workContext;
            _fusionIntegrationService = fusionIntegrationService;
            _dualMonthsService = dualMonthsService;
            _distributorService = distributorService;
            _shoppingCartService = shoppingCartService;
            _logger = logger;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(bool isEditable)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            var cart = shoppingCart
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .ToList();

            var baseModel = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(cart, isEditable);
            var model = PluginMapper.Mapper.Map<FiluetOrderTotalsModel>(baseModel);

            try
            {
                //get shopping cart total and show extra details only on checkout
                string path = this.Url.ActionContext.HttpContext.Request.Path.Value;
                if (path.Equals("/onepagecheckout", StringComparison.InvariantCultureIgnoreCase)
                    || path.Equals("/checkout/opcsavepaymentinfo/", StringComparison.InvariantCultureIgnoreCase)
                    || path.Equals("/checkout/OpcSaveShippingMethod/", StringComparison.InvariantCultureIgnoreCase)
                    || path.Equals("/checkout/OpcConfirmOrderInfo", StringComparison.InvariantCultureIgnoreCase))
                {
                    model.IsShowExtraData = true;

                    var cartTotal = await _fusionIntegrationService.GetShoppingCartTotalAsync(customer);
                    var distributorVolume =await _distributorService.GetDistributorVolumeAsync(customer);

                    string orderMonth =await _dualMonthsService.GetOrderMonthOfCustomerAsync(customer);
                    var orderMonthVolume = distributorVolume?.TvValue ?? 0;
                    var discountRate = cartTotal.DiscountPercent;
                    var orderVolumePoints = cartTotal.VolumePoints;
                    decimal productEarnbase = (cartTotal.ShoppingCartLines == null || !cartTotal.ShoppingCartLines.Any()) ? 0 : cartTotal.ShoppingCartLines.Sum(x => x.TotalEarnbase.HasValue ? x.TotalEarnbase.Value : 0);
                    decimal basePrice = cartTotal.AmountBase; //as sub total
                    decimal totalDue = cartTotal.TotalDue; //as total
                    decimal taxAmount = cartTotal.TotalTaxAmount;
                    var shippingCost = cartTotal.FreightCharge;
                    decimal discountedBasePrice = cartTotal.DiscountedBasePrice;
                    var orderType =await cart.GetOrderTypeAsync();

                    ICustomOrderService customOrderService;
                    var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                    using (var serviceScope = serviceScopeFactory.CreateScope())
                    {
                        customOrderService = serviceScope.ServiceProvider.GetService<ICustomOrderService>();
                    }

                    var deliveryPrice = await customOrderService?.ShowCustomOrderCardMessageAsync(customer) == true || orderType == CategoryTypeEnum.Maintenance || orderType == CategoryTypeEnum.Ticket ? 0 : await _genericAttributeService.GetAttributeAsync<decimal>(customer, CustomerAttributeNames.DeliveryPrice);

                    var store = await _storeContext.GetCurrentStoreAsync();
                    var flatFee = cartTotal.DeliveryFlatFee;

                    if (flatFee > 0)
                        totalDue += deliveryPrice + flatFee;
                    
                    model.SubTotal =await basePrice.FormatPriceAsync();
                    model.OrderTotal =await totalDue.FormatPriceAsync();
                    model.Tax =await taxAmount.FormatPriceAsync();
                    model.DisplayTax = true;
                    model.ProductEarnBase =await productEarnbase.FormatPriceAsync();
                    model.OrderVolumePoints =await orderVolumePoints.FormatPriceAsync(true);
                    model.DiscountRate = discountRate.FormatPercent();
                    model.OrderMonthVolume =await orderMonthVolume.FormatPriceAsync(true);
                    model.OrderMonth = orderMonth;
                    model.Shipping =await shippingCost.FormatPriceAsync();
                    model.DiscountedBasePrice =await discountedBasePrice.FormatPriceAsync();
                    model.DeliveryPrice = string.IsNullOrWhiteSpace(model.SelectedShippingMethod) ? await 0M.FormatPriceAsync() :await deliveryPrice.FormatPriceAsync(); // if selfPickup are used
                    model.FlatFee = null; 

                    if (flatFee > 0)
                        model.FlatFee =await flatFee.FormatPriceAsync();
                }
                else
                {
                    var cartTotal = await _fusionIntegrationService.GetShoppingCartTotalOffline(customer);
                    model.OrderTotal =await cartTotal.TotalDue.FormatPriceAsync();
                    model.OrderVolumePoints =await cartTotal.VolumePoints.FormatPriceAsync(true);
                }
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(LogLevel.Error, "[PERF DEBUG] FiluetOrderTotalsViewComponent error.", ex.ToString(), customer:customer);
            }

            return View(model);
        }

        #endregion
    }
}
