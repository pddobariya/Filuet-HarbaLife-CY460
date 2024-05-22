using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Framework.Controllers;
using System;
using System.Threading.Tasks;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Controllers
{
    public class PaymentSaferpayController : BasePaymentController
    {
        #region Fields

        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;

        #endregion

        #region Ctor
        public PaymentSaferpayController(
            IStoreContext storeContext,
            ISettingService settingService,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
        }

        #endregion

        #region Methods

        [HttpGet]
        public async Task<IActionResult> NotifyAsync(string guid)
        {
            var processor = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.Saferpay") as SaferpayPaymentProcessor;
            var result = await processor.CaptureAsync(new CapturePaymentRequest() { Order = new Order() { OrderGuid = new Guid(guid) } });
            if (result.CaptureTransactionResult.Contains("ERROR"))
                return BadRequest();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> PaySuccessAsync(string orderId)
        {
            var processor = await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.Saferpay") as SaferpayPaymentProcessor;
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentSettings =await _settingService.LoadSettingAsync<SaferpayPaymentSettings>(storeScope);
            if (paymentSettings.Bypass)
            {
                var result =await processor.CaptureAsync(new CapturePaymentRequest() { Order = new Order() { CustomOrderNumber = orderId } });
                if (result.CaptureTransactionResult.Contains("ERROR"))
                    return await PayFailureAsync(orderId);
            }
            else
            {
                await Task.Delay(4000);
                var order = await _orderService.GetOrderByCustomOrderNumberAsync(orderId);

                //if (!await _genericAttributeService.GetAttributeAsync<bool>(order, OrderAttributeNames.IsFusionSubmitOrderSuccess))
                //    return RedirectToAction("PaySuccess", new { orderId = orderId });

                //Note:- After paying for the order, the user receives an error for temporary comment this code
            }
            return RedirectToRoute("CheckoutCompleted", new { orderId = orderId });
        }

        [HttpGet]
        public async Task<IActionResult> PayFailureAsync(string orderId)
        {
            return await Task.FromResult(RedirectToRoute("ReOrder", new { orderId = orderId }));
        }

        #endregion
    }
}
