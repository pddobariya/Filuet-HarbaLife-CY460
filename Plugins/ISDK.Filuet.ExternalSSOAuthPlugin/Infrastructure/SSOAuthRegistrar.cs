using System;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure
{
    public class SSOAuthRegistrar : IExternalAuthenticationRegistrar
    {
        #region Methods

        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddOAuth<SSOAuthOptions, SSOAuthHandler>(
                SSOAuthHerbalifeDefaults.AuthenticationScheme,
                SSOAuthHerbalifeDefaults.AuthenticationScheme,
                options => { });

            builder.Services.Configure<CookieAuthenticationOptions>(NopAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ExpireTimeSpan = new TimeSpan(24, 0, 0);
                options.SlidingExpiration = false;
            });
        }

        #endregion
    }
}
