using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record DeliveryOperatorsCityModel :  BaseNopEntityModel
    {
        #region Properties

        public int LanguageId { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.DeliveryOperatorsCity.Models.CityName")]
        public string CityName { get; set; }

        public virtual DeliveryOperator_DeliveryType_DeliveryCity_Dependency DeliveryOperator_DeliveryType_DeliveryCity_Dependencies { get; set; }
        
        #endregion
    }
}
