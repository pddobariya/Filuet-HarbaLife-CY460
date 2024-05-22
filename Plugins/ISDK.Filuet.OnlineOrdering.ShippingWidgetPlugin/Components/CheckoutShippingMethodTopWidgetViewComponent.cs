using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components
{
    [ViewComponent(Name = "CheckoutShippingMethodTopWidget")]
    public class CheckoutShippingMethodTopWidgetViewComponent : NopViewComponent
    {
        #region Method

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
