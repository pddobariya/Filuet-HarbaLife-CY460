using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.Payeezy.Models;
using Nop.Services.Common;
using Nop.Web.Framework.Components;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.Payeezy.Components
{
    public class PaymentPayeezyViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public PaymentPayeezyViewComponent(
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            PaymentInfoModel model = new PaymentInfoModel();

            var currentCustomer =await _workContext.GetCurrentCustomerAsync();
            if (currentCustomer != null)
            {
                string email = currentCustomer.Email;
                if (email == NopFiluetCommonDefaults.EmptyDisplayPlaceholder)
                {
                    email = String.Empty;
                }
                model.InvoiceEmail = email;
            }
            if (await _genericAttributeService.HasCustomAttributeAsync(currentCustomer, CustomerAttributeNames.IsShipInvoiceWithOrder))
            {
                model.IsShipInvoiceWithOrder = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer,CustomerAttributeNames.IsShipInvoiceWithOrder);
            }
           
            return View(model);
        }

        #endregion
    }
}
