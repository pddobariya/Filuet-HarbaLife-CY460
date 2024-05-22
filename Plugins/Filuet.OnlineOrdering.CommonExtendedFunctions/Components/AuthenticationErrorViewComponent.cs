using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions.Components
{
    public class AuthenticationErrorViewComponent : NopViewComponent
    {
        #region Method

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
