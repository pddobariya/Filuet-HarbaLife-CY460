using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Controllers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Controllers
{
    public class FiluetShippingWidgetCheckoutController : FiluetCheckoutController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly OrderSettings _orderSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;


        #endregion

        #region Ctor

        public FiluetShippingWidgetCheckoutController(
            AddressSettings addressSettings, 
            CaptchaSettings captchaSettings, 
            CustomerSettings customerSettings, 
            IAddressAttributeParser addressAttributeParser, 
            IAddressModelFactory addressModelFactory, 
            IAddressService addressService, 
            ICheckoutModelFactory checkoutModelFactory, 
            ICountryService countryService, 
            ICustomerService customerService, 
            IGenericAttributeService genericAttributeService, 
            ILocalizationService localizationService, 
            ILogger logger, 
            IOrderProcessingService orderProcessingService,
            IOrderService orderService, 
            IPaymentPluginManager paymentPluginManager, 
            IPaymentService paymentService, 
            IProductService productService, 
            IShippingService shippingService, 
            IShoppingCartService shoppingCartService, 
            IStoreContext storeContext, 
            ITaxService taxService, 
            IWebHelper webHelper, 
            IWorkContext workContext, 
            OrderSettings orderSettings, 
            PaymentSettings paymentSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            TaxSettings taxSettings,
            IFiluetShippingCartService filuetShippingCartService,
            IDualMonthsService dualMonthsService, 
            ICategoryService categoryService,
            IHttpContextAccessor httpContextAccessor, 
            IPluginService pluginService) : base(addressSettings, captchaSettings, customerSettings, addressAttributeParser, addressModelFactory, addressService, checkoutModelFactory, countryService, customerService, genericAttributeService, localizationService, logger, orderProcessingService, orderService, paymentPluginManager, paymentService, productService, shippingService, shoppingCartService, storeContext, taxService, webHelper, workContext, orderSettings, paymentSettings, rewardPointsSettings, shippingSettings, taxSettings, filuetShippingCartService, dualMonthsService, categoryService, httpContextAccessor, pluginService)
        {
            _customerService = customerService;
            _logger = logger;
            _shoppingCartService = shoppingCartService;
            _storeContext = storeContext;
            _workContext = workContext;
            _orderSettings = orderSettings;
        }




        #endregion

        #region Methods

        #region OpcSaveBilling

        public override async Task<IActionResult> OpcSaveBilling(CheckoutBillingAddressModel model, IFormCollection form)
        {
            var shoppingCartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(shoppingCartItems))
            {
                var cart = shoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .ToList();
                return await base.OpcLoadStepAfterShippingMethod(cart);
            }
            return Json(new
            {
                update_section = new UpdateSectionJsonModel
                {
                    name = "shipping-method",
                    html = await RenderPartialViewToStringAsync("~/Plugins/ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin/Views" + "/FiluetCheckout/FiluetOpcShippingMethods.cshtml", null)
                },
                goto_section = "shipping_method"
            });
        }

        #endregion

        #region OpcSaveShippingMethod
       
        public override async Task<IActionResult> OpcSaveShippingMethod(string shippingoption, IFormCollection form)
        {
            try
            {
                var shoppingCartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
                var cart = shoppingCartItems
                   .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                   .ToList();

                if (!cart.Any())
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");
                
                if (await _customerService.IsGuestAsync(await _workContext.GetCurrentCustomerAsync()) && !_orderSettings.AnonymousCheckoutAllowed)
                    throw new Exception("Anonymous checkout is not allowed");

                if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
                    throw new Exception("Shipping is not required");

                return await base.OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc,await _workContext.GetCurrentCustomerAsync());
                return Json(new { error = 1, message = exc.Message });
            }
        }

        #endregion

        #endregion
    }
}
