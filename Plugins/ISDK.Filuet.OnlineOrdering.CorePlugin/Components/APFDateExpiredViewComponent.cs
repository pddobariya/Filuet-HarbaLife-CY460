using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Components;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Components
{
    [ViewComponent(Name = CommonConstants.APFDateExpiredComponentName)]
    public class APFDateExpiredViewComponent : NopViewComponent
    {
        #region Fields
        
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public APFDateExpiredViewComponent(
            IWorkContext workContext)
        {
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IApfExtendedFunctionsHelper apfExtendedFunctionsHelper = null;
            try
            {
                apfExtendedFunctionsHelper = EngineContext.Current.Resolve<IApfExtendedFunctionsHelper>();
            }
            catch { }

            if (apfExtendedFunctionsHelper == null || !apfExtendedFunctionsHelper.IsApfAdded())
                return Content(string.Empty);

            Customer customer = await _workContext.GetCurrentCustomerAsync();
            var apfDate = await customer.GetApfDueDateAsync();
            apfDate = apfDate?.Date <= DateTime.Now.Date ? apfDate : null;
            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Views/Shared/Components/APFDateExpired/Default.cshtml", apfDate);
        }

        #endregion
    }
}
