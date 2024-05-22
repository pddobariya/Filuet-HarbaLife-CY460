using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Components
{
    [ViewComponent(Name = SSOAuthHerbalifeDefaults.ViewComponentName)]
    public class SSOAuthViewComponent : NopViewComponent
    {
        #region Method

        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Views/PublicInfo.cshtml");
        }

        #endregion
    }
}