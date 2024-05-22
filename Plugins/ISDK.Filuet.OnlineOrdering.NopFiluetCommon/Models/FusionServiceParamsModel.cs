namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public class FusionServiceParamsModel
    {
        #region Properties

        public string DistributorId { get; set; }

        public string DsType { get; set; }

        //ProcessingLocationCode
        public string ProcessingLocation { get; set; }

        public string WarehouseCode { get; set; }

        public string FreightCode { get; set; }

        public string OrderMonth { get; set; }

        public string OrderNumber { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }

        //Zipcode
        public string PostalCode { get; set; }

        public string Address { get; set; }

        public string PostamatId { get; set; }

        public string TimeSlot { get; set; }

        public string PhoneNumber { get; set; }

        public string CustomerName { get; set; }

        public double VolumePoints { get; set; }

        //OrderCategory
        public string OrderTypeCode { get; set; }

        public string OrderType { get; set; }

        public string OrderTypeId { get; set; }

        public bool? InvoiceWithShipment { get; set; }

        #endregion
    }
}
