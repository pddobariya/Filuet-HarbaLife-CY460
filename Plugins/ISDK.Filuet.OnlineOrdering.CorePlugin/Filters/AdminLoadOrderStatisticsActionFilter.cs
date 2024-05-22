using ISDK.Filuet.OnlineOrdering.CorePlugin.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using System;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Filters
{
    public class AdminLoadOrderStatisticsActionFilter : ActionFilterAttribute, IFilterProvider
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            FiluetOrderController ctrl = EngineContext.Current.Resolve<FiluetOrderController>();
            ILogger logger = EngineContext.Current.Resolve<ILogger>();
            string period = Convert.ToString(filterContext.ActionArguments["period"]);            
            filterContext.Result = ctrl.LoadOrderStatistics(period).Result;
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {            
        }

        public void OnProvidersExecuting(FilterProviderContext context)
        {            
        }
    }
}
