using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.Payeezy.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //payment ok callback
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Payeezy.PaymentComplete",
                 "Plugins/PaymentPayeezy/PaymentComplete",
                 new { controller = "PaymentPayeezy", action = "PaymentComplete" }
            );

            //test reversal
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Payeezy.TestReverseTransaction",
                 "Plugins/PaymentPayeezy/testreversal",
                 new { controller = "PaymentPayeezy", action = "TestReverseTransaction" }
            );

            //payment fail callback
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Payeezy.PaymentError",
                 "Plugins/PaymentPayeezy/PaymentError",
                 new { controller = "PaymentPayeezy", action = "PaymentError" }
            );

            //mobile payment page
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Payeezy.PaymentMobile",
                 "Plugins/PaymentPayeezy/PaymentMobile",
                 new { controller = "PaymentPayeezy", action = "PaymentMobile" }
            );

            //test page
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Payeezy.TestNewOrder",
                 "Plugins/PaymentPayeezy/TestNewOrder",
                 new { controller = "PaymentPayeezy", action = "TestNewOrder" }
            );
        }

        public int Priority => -1;
    }
}
