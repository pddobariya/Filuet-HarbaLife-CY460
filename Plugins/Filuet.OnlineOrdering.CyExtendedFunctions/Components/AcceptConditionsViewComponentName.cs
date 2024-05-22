using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CyExtendedFunctions.Components
{
    public class AcceptConditionsViewComponent : NopViewComponent
    {
        #region Methods
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View();
        }
        #endregion
    }
}
