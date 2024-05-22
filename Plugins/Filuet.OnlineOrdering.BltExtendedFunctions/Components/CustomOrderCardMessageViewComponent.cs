using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.BltExtendedFunctions.Components
{
    public class CustomOrderCardMessageViewComponent : NopViewComponent
    {
        #region Fileds

        private readonly ICustomOrderService _customOrderService;

        #endregion

        #region Ctor

        public CustomOrderCardMessageViewComponent(
            ICustomOrderService customOrderService)
        {
            _customOrderService = customOrderService;
        }

        #endregion

        #region Method

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!await _customOrderService.ShowCustomOrderCardMessageAsync())
            {
                return Content(string.Empty);
            }
            return View();
        }

        #endregion
    }
}