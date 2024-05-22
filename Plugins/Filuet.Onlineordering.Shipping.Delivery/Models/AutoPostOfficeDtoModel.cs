using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record AutoPostOfficeDtoModel : BaseNopEntityModel
    {
        #region Ctor

        public AutoPostOfficeDtoModel()
        {
            AvailableDeliveryCityId = new List<SelectListItem>();
            AvailableOperatorId = new List<SelectListItem>();
            AvailableTypeId = new List<SelectListItem>();
            AutoPostOfficeDtoSearchModel = new AutoPostOfficeDtoSearchModel();
        }

        #endregion

        #region Properties

        public AutoPostOfficeDtoSearchModel AutoPostOfficeDtoSearchModel { get; set; }
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.PointId")]
        public string PointId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.Blocked")]
        public bool Blocked { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryCityId")]
        public int DeliveryCityId { get; set; }
        public IList<SelectListItem> AvailableDeliveryCityId { get; set; }
        public string DeliveryCityName { get; set; }
        public string DeliveryOperatorName { get; set; }
        public IList<SelectListItem> AvailableOperatorId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryOperatorId")]
        public int DeliveryOperatorId { get; set; }
        public IList<SelectListItem> AvailableTypeId { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.DeliveryTypeId")]
        public int DeliveryTypeId { get; set; }
        public string TypeName { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.Address")]
        public string Address { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.Comment")]
        public string Comment { get; set; }

        [NopResourceDisplayName("Plugins.ShippingMethods.Delivery.Fields.PriceIsAbsent")]
        public bool PriceIsAbsent { get; set; }

        public int DeliveryOperator_DeliveryType_DeliveryCity_DependencyId { get; set; }

        #endregion
    }
}
