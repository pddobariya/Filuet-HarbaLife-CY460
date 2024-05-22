using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components
{
    [ViewComponent(Name = "OrderSummaryContentBeforeWidget")]
    public class OrderSummaryContentBeforeWidgetViewComponent : NopViewComponent
    {
        #region Method

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
