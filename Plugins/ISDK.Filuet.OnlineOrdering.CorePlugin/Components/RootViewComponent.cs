using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Components
{
    [ViewComponent(Name = CommonConstants.RootViewComponentName)]
    public class RootViewComponent : NopViewComponent
    {
        #region Methods

        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/Components/Root/Default.cshtml");
        }

        #endregion
    }
}
