using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ISDK.Filuet.ExternalSSOAuthPlugin
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("CountryRestrictions",
                "SSOAuth/CountryRestrictions",
                new { controller = "SSOAuth", action = "CountryRestrictions" });

            endpointRouteBuilder.MapControllerRoute("APFRestrictions",
                "SSOAuth/APFRestrictions",
                new { controller = "SSOAuth", action = "APFRestrictions" });
            
            endpointRouteBuilder.MapControllerRoute("DistributorRestrictions",
                "SSOAuth/DistributorRestrictions",
                new { controller = "SSOAuth", action = "DistributorRestrictions" });
        }
    }
}
