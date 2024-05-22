using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components
{
    public class ShippingCountryPopupViewComponent : NopViewComponent
    {
        #region Field

        private readonly IShippingWidgetService _shippingWidgetService;

        #endregion

        #region Ctor

        public ShippingCountryPopupViewComponent(IShippingWidgetService shippingWidgetService)
        {
            _shippingWidgetService = shippingWidgetService;
        }

        #endregion

        #region Method

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ShippingCountryPopupModel model = await _shippingWidgetService.PrepareSippingCountryPopupModelAsync();

            return View(model);
        }

        #endregion

    }
}
