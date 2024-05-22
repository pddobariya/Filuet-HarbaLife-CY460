using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record PriceDtoAddModel : BaseNopEntityModel
    {
        #region Ctor

        public PriceDtoAddModel()
        {
            AvailableDeliveryCityId = new List<SelectListItem>();
            AvailableOperatorId = new List<SelectListItem>();
            AvailableTypeId = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public IList<SelectListItem> AvailableDeliveryCityId { get; set; }
        public IList<SelectListItem> AvailableOperatorId { get; set; }
        public IList<SelectListItem> AvailableTypeId { get; set; }
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryPrise")]
        public decimal DeliveryPrise { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryCityId")]
        public int DeliveryCityId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryOperatorId")]
        public int DeliveryOperatorId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryTypeId")]
        public int DeliveryTypeId { get; set; }

        public string DeliveryCityName { get; set; }
        public string DeliveryOperatorName { get; set; }
        public string TypeName { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.MinCriterionValue")]
        public decimal MinCriterionValue { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.MaxCriterionValue")]
        public decimal MaxCriterionValue { get; set; }

        #endregion
    }
}
