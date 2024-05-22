using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure
{
    /// <inheritdoc/>
    public class RouteProvider : IRouteProvider
    {
        #region Methods

        public int Priority => 1;

        /// <inheritdoc/>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Sitemap", "sitemap",
                new { controller = "FiluetCommon", action = "Sitemap" });

            //routeBuilder.Routes.RemoveRouteByName("ContactUs");
            endpointRouteBuilder.MapControllerRoute("ContactUs", "contactus",
                new { controller = "FiluetCommon", action = "ContactUs" });

            endpointRouteBuilder.MapControllerRoute(name: "areaRoute", pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            //routeBuilder.Routes.RemoveRouteByName("Login");
            endpointRouteBuilder.MapControllerRoute("Login",
                "login/",
                new { controller = "FiluetCustomer", action = "UserLogin" });

            //routeBuilder.Routes.RemoveRouteByName("Logout");
            endpointRouteBuilder.MapControllerRoute("Logout",
                "logout/",
                new { controller = "FiluetCustomer", action = "Logout" });

            //home page
            //routeBuilder.Routes.RemoveRouteByName("HomePage");
            endpointRouteBuilder.MapControllerRoute("HomePage", "",
                new { controller = "FiluetHome", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(
                "AdminLogin",
                "adminlogin/",
                new { controller = "Customer", action = "Login" });

            //routeBuilder.Routes.RemoveRouteByName("CheckoutConfirm");
            endpointRouteBuilder.MapControllerRoute(
                "CheckoutConfirm",
                "checkout/confirm",
                new { controller = "FiluetCheckout", action = "Confirm" });

            //routeBuilder.Routes.RemoveRouteByName("CustomerInfo");
            endpointRouteBuilder.MapControllerRoute(
                "CustomerInfo",
                "customer/info/",
                new { controller = "FiluetCustomer", action = "Info" });
            
            //routeBuilder.Routes.RemoveRouteByName("CustomerOrders");
            endpointRouteBuilder.MapControllerRoute("CustomerOrders", "order/history",
                new { controller = "FiluetOrder", action = "CustomerOrders" });

            //routeBuilder.Routes.RemoveRouteByName("ShoppingCart");
            endpointRouteBuilder.MapControllerRoute(
                "ShoppingCart",
                "cart/",
                new { controller = "FiluetShoppingCart", action = "Cart" });

            //routeBuilder.Routes.RemoveRouteByName("OrderDetails");
            endpointRouteBuilder.MapControllerRoute(
                "OrderDetails",
                "orderdetails/{orderId}",
                new { controller = "FiluetOrder", action = "Details" });

            //routeBuilder.Routes.RemoveRouteByName("CheckoutCompleted");
            endpointRouteBuilder.MapControllerRoute(
                "CheckoutCompleted",
                "checkout/completed/{orderId}",
                new { controller = "FiluetCheckout", action = "Completed" });

            //routeBuilder.Routes.RemoveRouteByName("OpcConfirmOrder");
            endpointRouteBuilder.MapControllerRoute(
                "OpcConfirmOrder",
                "checkout/opcconfirmorder",
                new { controller = "FiluetCheckout", action = "OpcConfirmOrder" });

            //routeBuilder.Routes.RemoveRouteByName("CheckoutOnePage");
            endpointRouteBuilder.MapControllerRoute("CheckoutOnePage", "onepagecheckout/",
                new { controller = "FiluetCheckout", action = "OnePageCheckout" });

            
            endpointRouteBuilder.MapControllerRoute("OpcSavePaymentInfo", "checkout/OpcSavePaymentInfo/",
                new { controller = "FiluetCheckout", action = "OpcSavePaymentInfo" });

            //add product to cart (without any attributes and options). used on catalog pages. (AJAX)
            endpointRouteBuilder.MapControllerRoute(name: "AddProductToCart-Catalog",
                pattern: $"addproducttocart/catalog/{{productId:min(0)}}/{{shoppingCartTypeId:min(0)}}/{{quantity:min(0)}}",
                defaults: new { controller = "FiluetShoppingCart", action = "AddProductToCart_Catalog" });

            EnsureTablesExist();
        }

        private void EnsureTablesExist()
        {
            //this.DbContext.EnsureTablesExist();
        }

        #endregion
    }
}