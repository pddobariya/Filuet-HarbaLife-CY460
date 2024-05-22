using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components
{
    [ViewComponent(Name = "WidgetHead")]
    public class WidgetHeadViewComponent : NopViewComponent
    {
        #region Method

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
