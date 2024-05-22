using Filuet.Onlineordering.Shipping.Delivery.Services;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Services.Common;
using Nop.Web.Framework.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.ORDER_INFO)]
    public class OrderInfoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ThemeFiluetHerbalifeSettings _themeFiluetHerbalifeSettings;
        private readonly IDeliveryOperatorService _deliveryOperatorService;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor


        public OrderInfoViewComponent(IWorkContext workContext,
            IStoreContext storeContext, 
            ThemeFiluetHerbalifeSettings themeFiluetHerbalifeSettings, 
            IDeliveryOperatorService deliveryOperatorService,
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _storeContext = storeContext;
            _themeFiluetHerbalifeSettings = themeFiluetHerbalifeSettings;
            _deliveryOperatorService = deliveryOperatorService;
            _genericAttributeService = genericAttributeService;
        }


        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var model = new FiluetOrderDetailsModel();
            model.CustomerName = await customer.GetFullNameAsync();

            //init shipping and invoice data
           
            model.ShipToAddress = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.Address);
            model.ShipToCity = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.City);
            model.ShipToCountryCode = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.CountryOrderOfProcessing) ?? await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.CountryOfProcessing);
            model.ShipToZipcode = await _genericAttributeService.GetAttributeAsync<string>(customer, OrderAttributeNames.SelectedShippingZipCode);
            model.ShipToFullname = await _genericAttributeService.GetAttributeAsync<string>(customer, OrderAttributeNames.SelectedShippingFullname);
            model.ShipToPhone = await _genericAttributeService.GetAttributeAsync<string>(customer, OrderAttributeNames.SelectedShippingPhoneNumber);
            model.ShipToPostamatId = await _genericAttributeService.GetAttributeAsync<string>(customer, OrderAttributeNames.SelectedShippingPostamatId);
            model.IsShipInvoiceWithOrder = await _genericAttributeService.GetAttributeAsync<bool>(customer, OrderAttributeNames.IsShipInvoiceWithOrder);
            model.Comment = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.ShippingNotes);

            string deliveryOperatorId = await _genericAttributeService.GetAttributeAsync<string>(customer, FiluetThemePluginDefaults.SelectedDeliveryOperatorId);

            if (!string.IsNullOrEmpty(deliveryOperatorId) && int.TryParse(deliveryOperatorId, out int operatorId))
            {
                var deliveryOperatorLanguage = await _deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(operatorId);
                model.DeliveryOperator = deliveryOperatorLanguage.First(dol => dol.LanguageId == language.Id).OperatorName;
                model.Comment = $"DeliveryOperator: {model.DeliveryOperator}{await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.ShippingNotes)}";

                if (model.DeliveryOperator.ToUpper().Contains("OMNIVA"))
                {
                    ViewBag.ShipToCarrierUrl = _themeFiluetHerbalifeSettings.OmnivaCarrierUrl;
                }
                else if (model.DeliveryOperator.ToUpper().Contains("DPD"))
                {
                    if (model.ShipToCountryCode == "LV")
                        ViewBag.ShipToCarrierUrl = _themeFiluetHerbalifeSettings.DPDLatviaCarrierUrl;
                    else if (model.ShipToCountryCode == "LT")
                        ViewBag.ShipToCarrierUrl = _themeFiluetHerbalifeSettings.DPDLithuaniaCarrierUrl;
                    else if (model.ShipToCountryCode == "EE")
                        ViewBag.ShipToCarrierUrl = _themeFiluetHerbalifeSettings.DPDEstoniaCarrierUrl;
                }
            }
            else
            {
                model.DeliveryOperator = deliveryOperatorId;
            }

            var shippingDetailsComment = await _genericAttributeService.GetAttributeAsync<string>(customer,ShippingDetailsAttributes.ShippingDetailsCommentAttribute, currentStore.Id);
            if (!string.IsNullOrEmpty(shippingDetailsComment))
            {
                try
                {
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<int, string>>(shippingDetailsComment);
                    ViewBag.autoPostOfficeLanguageComment = dictionary[language.Id];
                }
                catch { }
            }

            return View(model);
        }

        #endregion
    }
}
