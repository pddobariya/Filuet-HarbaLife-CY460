using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Nop.Core.Infrastructure;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure
{
    public class SSOAuthOptions : OAuthOptions
    {
        #region Methods

        public SSOAuthOptions()
        {
            SSOAuthPluginSettings settings = EngineContext.Current.Resolve<SSOAuthPluginSettings>();
            IHttpContextAccessor _httpContextAccessor = EngineContext.Current.Resolve<IHttpContextAccessor>();
            CallbackPath = new PathString(SSOAuthHerbalifeDefaults.CallbackPath);
            AuthorizationEndpoint = settings.AuthorizationEndpoint;
            TokenEndpoint = settings.TokenEndpoint;
            ClientId = settings.ClientId;
            ClientSecret = settings.ClientSecret;
            //string returnUrl = string.Empty;
            SaveTokens = true;
            Scope.Add("openid");

            Events = new  OAuthEvents
            {

                OnRemoteFailure = context =>
                {
                    context.HandleResponse();

                    var errorUrl = new PathString(SSOAuthHerbalifeDefaults.CallbackPath);

                    string url = _httpContextAccessor.HttpContext.Request.GetDisplayUrl();

                    if (url.Contains("LoginCallback"))
                    {
                        url = url.Replace("LoginCallback", "LoginCallback1");

                    }
                    // Fetch the return URL from TempData                    
                    context.Response.Redirect(url);

                    return Task.FromResult(0);
                }
            };            


        }

        #endregion
    }
}
