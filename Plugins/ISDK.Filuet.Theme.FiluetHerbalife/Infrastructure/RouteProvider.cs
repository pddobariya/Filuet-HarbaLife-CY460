using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.ExternalAuth.Facebook.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        #region Method

        #region RegisterRoutes

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //topic
            endpointRouteBuilder.MapControllerRoute(name: "Faq",
                pattern: $"/all-questions",
                defaults: new { controller = "FiluetTopic", action = "Faq" });

            endpointRouteBuilder.MapControllerRoute(name: "FaqDetails",
                pattern: $"faq/details/{{topicId?}}",
                defaults: new { controller = "FiluetTopic", action = "FaqDetails" });

            //product search
            endpointRouteBuilder.MapControllerRoute(name: "ProductSearch",
                pattern: $"/search/",
                defaults: new { controller = "FiluetCatalog", action = "FiluetSearch" });

            endpointRouteBuilder.MapControllerRoute(name: "SearchProducts",
                pattern: $"product/search",
                defaults: new { controller = "FiluetCatalog", action = "FiluetSearchProducts" });

            endpointRouteBuilder.MapControllerRoute("CheckoutOnePage",
                pattern: "onepagecheckout/",
                defaults: new { controller = "FiluetHerbalifeCheckout", action = "OnePageCheckout" });

            endpointRouteBuilder.MapControllerRoute(name: "OpcSaveShippingMethod",
                pattern: "checkout/OpcSaveShippingMethod/",
                defaults: new { controller = "FiluetHerbalifeCheckout", action = "OpcSaveShippingDelivery" });

            endpointRouteBuilder.MapControllerRoute(name: "OpcSavePaymentInfo",
                pattern: "checkout/OpcSavePaymentInfo/",
                defaults: new { controller = "FiluetHerbalifeCheckout", action = "OpcSavePaymentInfo" });

            endpointRouteBuilder.MapControllerRoute(name: "OpcConfirmOrderInfo",
                pattern: "checkout/OpcConfirmOrderInfo/",
                defaults: new { controller = "FiluetHerbalifeCheckout", action = "OpcConfirmOrderInfo" });

            endpointRouteBuilder.MapControllerRoute(name: "CheckoutCompleted",
                pattern: "checkout/completed/{orderId}",
                defaults: new { controller = "FiluetHerbalifeCheckout", action = "Completed" });
        }

        #endregion

        #region Priority

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 2;

        #endregion

        #endregion
    }
}
