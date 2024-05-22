using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure
{
    public class SSOAuthHandler : OAuthHandler<SSOAuthOptions>
    {
        public SSOAuthHandler(IOptionsMonitor<SSOAuthOptions> options, ILoggerFactory logger, 
            UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

    }
}
