using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion
{
    public class FusionServiceParams
    {
        #region Properties

        public string DistributorId { get; set; }

        public string ProcessingLocationCode { get; set; }

        public string WarehouseCode { get; set; }

        public string FreightCode { get; set; }

        public string OrderMonth { get; set; }

        public string OrderNumber { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }

        public string Zipcode { get; set; }

        public string Address { get; set; }

        public string PostamatId { get; set; }

        public string TimeSlot { get; set; }

        public string PhoneNumber { get; set; }

        public string Fullname { get; set; }

        public double VolumePoints { get; set; }

        public OrderPaymentModel OrderPaymentData { get; set; }

        public List<OrderItemModel> OrderItems { get; set; }

        public List<ShoppingCartLineModel> ShoppingCartLines { get; set; }

        public string OrderCategory {get;set;}

        public string OrderType { get; set; }

        public string OrderTypeId { get; set; }

        public bool? InvoiceWithShipment { get; set; }

        #endregion
    }
}
