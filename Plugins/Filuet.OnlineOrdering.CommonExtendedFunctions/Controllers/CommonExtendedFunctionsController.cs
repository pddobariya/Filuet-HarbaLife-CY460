using Microsoft.AspNetCore.Mvc;
using Nop.Web.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions.Controllers
{
    public class CommonExtendedFunctionsController : BasePublicController
    {
        #region Methods

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public IActionResult Index()
        {
            return View();
        }

        #endregion
    }
}
