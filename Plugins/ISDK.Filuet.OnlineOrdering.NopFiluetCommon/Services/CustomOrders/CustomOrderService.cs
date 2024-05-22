using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders
{
    public class CustomOrderService : ICustomOrderService
    {
        #region Fields 

        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        #endregion

        #region Ctor

        public CustomOrderService(
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IOrderService orderService,
            IProductService productService)
        {
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _productService = productService;
        }

        #endregion

        #region Methods

        public async Task<string> GetCustomFreightCodeAsync(Order order, Customer customer)
        {

            if (order != null && await (await _orderService.GetOrderItemsAsync(order.Id)).AllAwaitAsync(async oi => new[] { "R909", "5451", "R809" }.Contains((await _productService.GetProductByIdAsync(oi.ProductId)).Sku)) || order == null && await ShowCustomOrderCardMessageAsync(customer))
            {
                return FreightCodes.BLH;
            }

            return null;
        }

        public async Task<bool> ShowCustomOrderCardMessageAsync(Customer customer = null)
        {
            return await (await _shoppingCartService.GetShoppingCartAsync(customer ?? await _workContext.GetCurrentCustomerAsync())).AllAwaitAsync(async oi => new[] { "R909", "5451", "R809" }.Contains((await _productService.GetProductByIdAsync(oi.ProductId)).Sku));
        }

        #endregion
    }
}