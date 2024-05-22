using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Exceptions
{
    public class ExceptionMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;
        private readonly IHostApplicationLifetime _appLifetime;        
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ExceptionMiddleware(RequestDelegate next, 
            IHostApplicationLifetime appLifetime, 
            ILogger logger,
            ILocalizationService localizationService)
        {            
            _appLifetime = appLifetime;
            _next = next;
            _logger = logger;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (OutOfMemoryException ex)
            {
                Environment.FailFast($"ExceptionMiddleware OutOfMemory error. {ex.Message} {ex.StackTrace}");
                _appLifetime.StopApplication(); // Restart application (set azure appservice as always on)
            }
            catch (AuthenticationException exception)
            {
                httpContext.Response.ContentType = "application/json";

                await _logger.ErrorAsync($"ExceptionMiddleware Authentication error.", exception);

                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsync("Error. Trace Id: ");
            }
            catch (DistributorDetailedException exception)
            {
                httpContext.Response.ContentType = "application/json";

                await _logger.ErrorAsync($"ExceptionMiddleware DistributorDetailed error.", exception);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var text = await _localizationService.GetResourceAsync("Filuet.DistributorDetailed.Exception");
                await httpContext.Response.WriteAsync($"{text}. Trace Id: {httpContext.TraceIdentifier}");
            }
            catch (Exception exception)
            {
                httpContext.Response.ContentType = "application/json";

                await _logger.ErrorAsync($"ExceptionMiddleware Unknown error.", exception);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var sharedText = await _localizationService.GetResourceAsync("Filuet.Middleware.Exception");
                await httpContext.Response.WriteAsync($"{sharedText}. Trace Id: {httpContext.TraceIdentifier}<br> {exception}");
            }
        }

        #endregion
    }
}
