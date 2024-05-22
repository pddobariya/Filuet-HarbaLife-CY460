using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        #region RegisterRoutes/Priority

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("OpcSaveBilling", $"checkout/OpcSaveBilling/",
                new { controller = "FiluetShippingWidgetCheckout", action = "OpcSaveBilling" });

            endpointRouteBuilder.MapControllerRoute(name: "ShippingCountrySelectorUpdate",
                pattern: "ShippingCountrySelector/Update",
                defaults: new { controller = "FiluetShippingCountrySelector", action = "Update" });

            endpointRouteBuilder.MapControllerRoute(name: "ShippingCountryPopupConfirm",
                pattern: "ShippingCountrySelector/PopupConfirm",
                defaults: new { controller = "FiluetShippingCountrySelector", action = "PopupConfirm" });

            endpointRouteBuilder.MapControllerRoute(name: "ShippingCountryDefaultCountry",
               pattern: "ShippingCountrySelector/DefaultCountry/{clientTimeZone}",
               defaults: new { controller = "FiluetShippingCountrySelector", action = "DefaultCountry" });

        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;

        #endregion
    }
}
