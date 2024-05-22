using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Filuet.OnlineOrdering.BltExtendedFunctions.Components
{
    public class AfterTotalInfoOrderDetailsMessageViewComponent : NopViewComponent
    {
        #region Method

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
