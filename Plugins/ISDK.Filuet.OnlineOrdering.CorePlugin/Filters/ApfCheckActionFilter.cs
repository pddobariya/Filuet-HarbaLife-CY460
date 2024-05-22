using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Filters
{
    public class ApfCheckActionFilter : IActionFilter
    {
        public const string ShowApfMessage = "ShowApfMessage";

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ViewResult vr)
            {
                vr.ViewData[ShowApfMessage] = true;
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
