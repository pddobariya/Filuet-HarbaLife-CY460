using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace HBL.Uzbek.Onlineordering.Payments.Payme.Components
{
    public class PaymentSaferpayViewComponent : NopViewComponent
    {
        #region Methods

        public IViewComponentResult Invoke()
        {
            return View();
        }

        #endregion
    }
}
