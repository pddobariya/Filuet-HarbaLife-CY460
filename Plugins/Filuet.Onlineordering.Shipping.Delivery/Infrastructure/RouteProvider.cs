using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Filuet.Onlineordering.Shipping.Delivery.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        #region Methods

        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(name: "CheckoutDelivery",
                pattern: $"OpcSaveShippingDeliveryMethod",
                new { controller = "Delivery", action = "Costs" });
            endpointRouteBuilder.MapControllerRoute(name: "CheckoutDelivery",
                pattern: $"OpcSaveShippingDeliveryMethod",
                new { controller = "Delivery", action = "PickupCosts" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetDeliveryTyoes", "Plugins/Delivery/GetDeliveryTypes",
                new { controller = "Delivery", action = "GetDeliveryTypes" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetSalesCenters", "Plugins/Delivery/GetSalesCenters",
                new { controller = "Delivery", action = "GetSalesCenters" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetDeliveryOperatorsCities", "Plugins/Delivery/GetDeliveryOperatorsCities",
                new { controller = "Delivery", action = "GetDeliveryOperatorsCities" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetDeliveryOperators", "Plugins/Delivery/GetDeliveryOperators",
                new { controller = "Delivery", action = "GetDeliveryOperators" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetDeliveryAddresses", "Plugins/Delivery/GetDeliveryAddresses",
                new { controller = "Delivery", action = "GetDeliveryAddresses" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetAutoPostOffices", "Plugins/Delivery/GetAutoPostOffices",
                new { controller = "Delivery", action = "GetAutoPostOffices" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.LoadLocalization", "Plugins/Delivery/LoadLocalization",
                new { controller = "Delivery", action = "LoadLocalization" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.LoadMiscs", "Plugins/Delivery/LoadMiscs",
                new { controller = "Delivery", action = "LoadMiscs" });
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.Delivery.GetOperatorPrice", "Plugins/Delivery/GetOperatorPrice",
                new { controller = "Delivery", action = "GetOperatorPrice" });

            endpointRouteBuilder.MapControllerRoute("CheckoutOnePage", "onepagecheckout/",
               new { controller = "CheckoutDelivery", action = "OnePageCheckout" });
        }
        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1;

        #endregion
    }
}
