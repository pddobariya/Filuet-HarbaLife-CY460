using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class MapToFiluetModelExtensions
    {
        #region Methods

        public async static Task<List<OrderItemModel>> ToOrderItemModelListAsync(this IEnumerable<ShoppingCartItem> cartItems, string warehouseCode)
        {
            if (cartItems == null)
            {
                return new List<OrderItemModel>();
            }

            var orderItems = new List<OrderItemModel>();
            IProductAttributeService attrServ = EngineContext.Current.Resolve<IProductAttributeService>();            

            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            IProductService productService = null;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                productService = serviceScope.ServiceProvider.GetService<IProductService>();
            }


            foreach (ShoppingCartItem sci in cartItems)
            {
                OrderItemModel orderItem = await FusionHelpers.GetOrderItemAsync(sci.Quantity, await productService.GetProductByIdAsync(sci.ProductId), sci.AttributesXml, attrServ, warehouseCode);
                orderItems.Add(orderItem);
            }

            return orderItems;
        }

        public async static Task<List<OrderItemModel>> ToOrderItemModelListAsync(this IEnumerable<OrderItem> orderItems, string warehouse)
        {
            if (orderItems == null)
            {
                return new List<OrderItemModel>();
            }
            List<OrderItemModel> fusionOrderItems = new List<OrderItemModel>();
            IProductAttributeService attrServ = EngineContext.Current.Resolve<IProductAttributeService>();
            var productService = EngineContext.Current.Resolve<IProductService>();
            foreach (Nop.Core.Domain.Orders.OrderItem oi in orderItems)
            {
                OrderItemModel orderItem = await FusionHelpers.GetOrderItemAsync(oi.Quantity, await productService.GetProductByIdAsync(oi.ProductId), oi.AttributesXml, attrServ, warehouse);
                fusionOrderItems.Add(orderItem);
            }

            return fusionOrderItems;
        }

        #endregion
    }
}
