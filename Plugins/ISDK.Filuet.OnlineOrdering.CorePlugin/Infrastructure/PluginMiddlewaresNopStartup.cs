using ISDK.Filuet.OnlineOrdering.CorePlugin.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    public class PluginMiddlewaresNopStartup : INopStartup
    {
        public int Order => 1;

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseMiddleware<ExceptionMiddleware>();
            application.UseMiddleware<RequestBodyLoggingMiddleware>();
            application.UseMiddleware<ResponseBodyLoggingMiddleware>();
        }
    }
}