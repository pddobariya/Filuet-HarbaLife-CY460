using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class RequestBodyLoggingMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        public RequestBodyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;

            // Ensure the request body can be read multiple times
            context.Request.EnableBuffering();
            var requestTelemetry = context.Features.Get<RequestTelemetry>();
            requestTelemetry?.Properties.Add("Headers",
                string.Join(";", context.Request.Headers.Select(x => x.Key + ":" + x.Value)));
            
            // Only if we are dealing with POST or PUT, GET and others shouldn't have a body
            if (context.Request.Body.CanRead && (method == HttpMethods.Post || method == HttpMethods.Put))
            {
                // Leave stream open so next middleware can read it
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

                // Reset stream position, so next middleware can read it
                context.Request.Body.Position = 0;

                // Write request body to App Insights
                
                requestTelemetry?.Properties.Add("RequestBody", requestBody);
            }

            // Call next middleware in the pipeline
            await _next(context);
        }

        #endregion
    }
}