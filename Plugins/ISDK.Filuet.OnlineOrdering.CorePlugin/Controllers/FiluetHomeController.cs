using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Controllers;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    public class FiluetHomeController : HomeController
    {

        #region Ctor

        public FiluetHomeController()
        {
        }

        #endregion

        #region Methods

        public override IActionResult Index()
        {
            var currentRouteData = new RouteData(ControllerContext.RouteData);
            currentRouteData.Values["controller"] = "Home";
            ControllerContext.RouteData = currentRouteData;
            return base.Index();
        }

        #endregion

    }
}
