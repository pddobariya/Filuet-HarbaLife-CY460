using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using System;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        #region Method

        protected ObjectResult SaveError(Exception exception)
        {
            var logger = EngineContext.Current.Resolve<ILogger>();
            var log = logger.InsertLog(LogLevel.Error,
                $"[{nameof(ApiControllerBase)}.{nameof(SaveError)}]. Exception.",
                exception.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error #{log.Id}.");
        }
       
        #endregion
    }
}