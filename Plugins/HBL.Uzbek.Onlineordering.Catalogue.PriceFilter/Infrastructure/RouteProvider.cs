using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority => -1;

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "PriceFilter",
                pattern: "filuetcatalog/pricefilter/{priceFilterId}",
                new { controller = "FiluetCatalog", action = "PriceFilter" });
        }
    }
}
