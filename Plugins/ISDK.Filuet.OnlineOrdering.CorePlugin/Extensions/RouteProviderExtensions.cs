using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Extensions
{
    public static class RouteProviderExtensions
    {
        public static void RemoveRouteByName(this IList<IRouter> routes, string nameRoute)
        {
            var routeItem = routes.FirstOrDefault(p => ((Route)p).Name == nameRoute);
            if (routeItem != null)
            {
                routes.Remove(routeItem);
            }
        }
    }
}
