using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System;
using System.Net;
using System.ServiceModel;
using System.Text;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class PluginExceptionAttribute : TypeFilterAttribute
    {
        #region Fields

        private const string ErrorShortMessage = "AuthorizationServiceError";

        #endregion

        #region Ctor

        public PluginExceptionAttribute() : base(typeof(PluginExceptionFilter))
        {
        }

        #endregion

        #region Methods

        private class PluginExceptionFilter : IExceptionFilter
        {
            public async void OnException(ExceptionContext context)
            {
                ILogger logger = EngineContext.Current.Resolve<ILogger>();
                ILocalizationService localizationService = EngineContext.Current.Resolve<ILocalizationService>();

                var ex = context.Exception;

                if (ex is ProtocolException)
                {
                    HandleProtocolException(logger, (ProtocolException)ex);
                }
                else
                {
                    HandleException(logger, ex);
                }

                var items = context.HttpContext.Items;

                context.Result = new ViewResult
                {
                    ViewName = context.RouteData.Values["action"].ToString(),                                        
                    TempData = items.ContainsKey("TempData") ? (ITempDataDictionary)items["TempData"] : null,
                    ViewData = items.ContainsKey("ViewData") ? (ViewDataDictionary)items["ViewData"] : null                     
                };

                context.ExceptionHandled = true;

                // Administrator must resolve the problem                
                context.ModelState.AddModelError(string.Empty,await localizationService.GetResourceAsync("Account.Login.AuthorizationServiceError"));
            }

            private async void HandleProtocolException(ILogger logger, ProtocolException ex)
            {
                var webException = ex.InnerException as WebException;
                if (webException != null)
                {
                    var response = webException.Response;
                    if (response != null)
                    {
                        string requestUri = response.ResponseUri.ToString();
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            using (var reader = new System.IO.StreamReader(responseStream, Encoding.UTF8))
                            {
                                // Fault exception
                                string responseText = reader.ReadToEnd();
                                await logger.ErrorAsync(ErrorShortMessage, new Exception(responseText));
                            }
                        }
                    }
                }
                else
                {
                    await logger.ErrorAsync(ErrorShortMessage, ex);
                }
            }

            private async void HandleException(ILogger logger, Exception ex)
            {
               await logger.ErrorAsync(ErrorShortMessage, ex);
            }
        }

        #endregion
    }
}
