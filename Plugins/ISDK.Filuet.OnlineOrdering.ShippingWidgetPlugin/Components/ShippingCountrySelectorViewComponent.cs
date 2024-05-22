using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components
{
    [ViewComponent(Name = "ShippingCountrySelector")]
    public class ShippingCountrySelectorViewComponent : NopViewComponent
    {
        #region Field

        private readonly IShippingWidgetService _shippingWidgetService;

        #endregion

        #region Ctor

        public ShippingCountrySelectorViewComponent(IShippingWidgetService shippingWidgetService)
        {
            _shippingWidgetService = shippingWidgetService;
        }

        #endregion

        #region Method

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _shippingWidgetService.PrepareSippingCountrySelectorModelAsync();
            
            return View(model);
        }

        #endregion
    }
}
