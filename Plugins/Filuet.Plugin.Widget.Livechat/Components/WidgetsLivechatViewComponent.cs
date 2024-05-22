using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using System;
using System.Threading.Tasks;

namespace Filuet.Plugin.Widget.Livechat.Components
{
    [ViewComponent(Name = "WidgetsLivechat")]
    public class WidgetsLivechatViewComponent : NopViewComponent
    {
        #region Fields

        private readonly LivechatSettings _livechatSettings;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public WidgetsLivechatViewComponent(
            LivechatSettings livechatSettings,
            ILogger logger)
        {
            _livechatSettings = livechatSettings;
            _logger = logger;
        }

        #endregion

        #region Methods

        private string GetPreparedScriptAsync()
        {
            return _livechatSettings.TrackingScript;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var script = "";
            var routeData = Url.ActionContext.RouteData;

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");
                                
                script +=  GetPreparedScriptAsync();
            }
            catch (Exception ex)
            {
                await _logger.InsertLogAsync(Nop.Core.Domain.Logging.LogLevel.Error, "Process were raised error into creating Livechat scripts!", ex.ToString());
            }
            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}