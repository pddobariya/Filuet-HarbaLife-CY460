using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Nop.Web.Framework.Models;
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record  ShipingMethodViewModel : BaseNopEntityModel
    {
        #region Properties

        public IEnumerable<DeliveryTypeDto> DeliveryTypes { get; set; }
        public IEnumerable<DeliveryOperatorDto> DeliveryOperatorDtos { get; set; }
        public IEnumerable<DeliveryOperatorDto> PickupPointsOperatorDtos { get; set; }
        public IEnumerable<DeliveryCity> Cities { get; set; }
        public CityViewModel[] DeliveryCities { get; set; }
        public CityViewModel[] PickPointCities { get; set; }
        public IEnumerable<AutoPostOffice> AutoPostOffices { get; set; }
        public IEnumerable<SalesCenterDto> SalesCenterCities { get; set; }
        public List<string> Addresses { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Phones { get; set; }
        public string PhonePrefix { get; set; }
        public string PhoneMask { get; set; }
        public bool RequirePostCode { get; set; }
        public bool SelfPickupActive { get; set; }
        public decimal CriterionValue { get; set; }
        public string NotificationHtmlAboveModule { get; set; }

        #endregion
    }
}
