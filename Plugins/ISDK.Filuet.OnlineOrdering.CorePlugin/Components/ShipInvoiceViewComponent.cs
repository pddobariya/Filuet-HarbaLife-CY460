using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Components
{
    public class ShipInvoiceViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ShipInvoiceViewComponent(
            IGenericAttributeService genericAttributeService, 
            IWorkContext workContext)
        {
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new ShipInvoiceModel();

            if(await _genericAttributeService.HasCustomAttributeAsync(await _workContext.GetCurrentCustomerAsync(), CustomerAttributeNames.IsShipInvoiceWithOrder))
            {
                //streetAddressAttribute = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.StreetAddressAttribute);
               // model.IsShipInvoiceWithOrder = await (await _workContext.GetCurrentCustomerAsync()).GetAttributeAsync<bool>(CustomerAttributeNames.IsShipInvoiceWithOrder);
                model.IsShipInvoiceWithOrder = (await _genericAttributeService.GetAttributeAsync<bool>(await _workContext.GetCurrentCustomerAsync(), CustomerAttributeNames.IsShipInvoiceWithOrder));
            }
            return View(model);
        }

        #endregion
    }
}
