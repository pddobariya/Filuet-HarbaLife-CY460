using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        {
            SalesCenterSearchModel = new SalesCenterSearchModel();
            DeliveryTypeDtoSearchModel = new DeliveryTypeDtoSearchModel();
            DeliveryOperatorDtoModel = new DeliveryOperatorDtoModel();
            DeliveryOperatorDtoSearchModel = new DeliveryOperatorDtoSearchModel();
            DeliveryOperatorsCitySearchModel = new DeliveryOperatorsCitySearchModel();
            PriceDtoSearchModel = new PriceDtoSearchModel();
            PriceDtoAddModel = new PriceDtoAddModel();
            AutoPostOfficeDtoSearchModel = new AutoPostOfficeDtoSearchModel();
            AutoPostOfficeDtoModel = new AutoPostOfficeDtoModel();
            SalesCenterDtoModel = new SalesCenterDtoModel();
            SalesCenterLanguageModel = new SalesCenterLanguageModel();
            DeliveryOperatorsCityModel = new DeliveryOperatorsCityModel();
            DeliveryTypeDtoModel = new DeliveryTypeDtoModel();
        }

        #endregion

        #region Properties

        public DeliveryTypeDtoModel DeliveryTypeDtoModel { get; set; }
        public SalesCenterSearchModel SalesCenterSearchModel { get; set; }
        public DeliveryTypeDtoSearchModel DeliveryTypeDtoSearchModel { get; set; }
        public DeliveryOperatorDtoModel DeliveryOperatorDtoModel { get; set; }
        public PriceDtoAddModel PriceDtoAddModel { get; set; }
        public DeliveryOperatorDtoSearchModel DeliveryOperatorDtoSearchModel { get; set; }
        public DeliveryOperatorsCityModel DeliveryOperatorsCityModel { get; set; }
        public DeliveryOperatorsCitySearchModel DeliveryOperatorsCitySearchModel { get; set; }
        public PriceDtoSearchModel PriceDtoSearchModel { get; set; }
        public AutoPostOfficeDtoSearchModel AutoPostOfficeDtoSearchModel { get; set; }
        public AutoPostOfficeDtoModel AutoPostOfficeDtoModel { get; set; }
        public SalesCenterDtoModel SalesCenterDtoModel { get; set; }
        public SalesCenterLanguageModel SalesCenterLanguageModel { get; set; }
        public IList<Nop.Core.Domain.Localization.Language> Languages { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.PhonePrefix")]
        public string PhonePrefix { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.PhoneMask")]
        public string PhoneMask { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.SelfPickupActive")]
        public bool SelfPickupActive { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.AddAddressToComment")]
        public bool AddAddressToComment { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.RequirePostCode")]
        public bool RequirePostCode { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.SalesCenterElectronicQueueInvitation")]
        public string SalesCenterElectronicQueueInvitation { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.NotificationHtmlAboveModule")]
        public string NotificationHtmlAboveModule { get; set; }
        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.ShowInvitation")]
        public bool ShowInvitation { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.MinCriterion")]
        public int MinCriterion { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.PickPoint")]
        public decimal PickPoint { get; set; }

        [NopResourceDisplayName("Filuet.Onlineordering.Shipping.Delivery.HomeDelivery")]
        public decimal HomeDelivery { get; set; }

        #endregion
    }
}
