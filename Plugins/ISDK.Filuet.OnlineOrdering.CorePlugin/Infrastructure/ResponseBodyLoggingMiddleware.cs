using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class ResponseBodyLoggingMiddleware
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        #region Ctor

        public ResponseBodyLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        #endregion

        #region Methods

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                // Swap out stream with one that is buffered and supports seeking
                var responseStream = new MemoryStream();
                context.Response.Body = responseStream;

                // hand over to the next middleware and wait for the call to return
                await _next(context);

                // Read response body from memory stream
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(context.Response.Body);
                var responseBody = await reader.ReadToEndAsync();
                // Copy body back to so its available to the user agent
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await responseStream.CopyToAsync(originalBodyStream);

                // Write response body to App Insights
                var requestTelemetry = context.Features.Get<RequestTelemetry>();
                requestTelemetry?.Properties.Add("ResponseHeaders",
                    string.Join(";", context.Response.Headers.Select(x => x.Key + ":" + x.Value)));
                requestTelemetry?.Properties.Add("ResponseBody", responseBody);
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        #endregion
    }
}