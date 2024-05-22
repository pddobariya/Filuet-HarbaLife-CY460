﻿using Filuet.OnlineOrdering.BltExtendedFunctions.ViewEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Filuet.OnlineOrdering.BltExtendedFunctions.Infrastructure
{
    public class PluginNopStartup : INopStartup
    {
        #region Method

        public int Order => 3000;

        public void Configure(IApplicationBuilder application)
        {
        }
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new BltFunctionsViewEngine());
            });
        }

        #endregion
    }
}
