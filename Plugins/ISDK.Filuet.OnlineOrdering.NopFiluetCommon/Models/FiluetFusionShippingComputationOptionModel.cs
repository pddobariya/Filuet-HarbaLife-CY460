namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public class FiluetFusionShippingComputationOptionModel
    {
        #region Properties

        public int Id { get; set; }

        public string CountryCode { get; set; }

        public string WarehouseCode { get; set; }

        public string ProcessingLocationCode { get; set; }

        public bool IsSalesCenter { get; set; }

        public string FreightCode { get; set; }

        public string City { get; set; }

        public string ShippingZipCode { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        #endregion
    }
}
