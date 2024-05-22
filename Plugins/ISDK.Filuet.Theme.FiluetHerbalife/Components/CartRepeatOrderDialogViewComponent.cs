using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.CART_REPEAT_ORDER_DIALOG)]
    public class CartRepeatOrderDialogViewComponent : NopViewComponent
    {
        #region Field

        private readonly IOrderService _orderService;

        #endregion

        #region Ctor

        public CartRepeatOrderDialogViewComponent(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #endregion

        #region Method

        public async Task<IViewComponentResult> InvokeAsync(int? orderId)
        {
            var model = new CartRepeatOrderDialogModel
            {
                OrderId = orderId
            };

            if (orderId.HasValue)
            {
                var order = await _orderService.GetOrderByIdAsync(orderId.Value);
                model.FusionOrderNumber =await order.GetFusionOrderNumberAsync();
                
            }

            return View(model);
        }

        #endregion

    }
}
