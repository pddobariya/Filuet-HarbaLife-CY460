using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class PostamatItem
    {
        #region Properties

        public string PostamatId { get; set; }

        public CountryCodes Country { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string ZipCode { get; set; }

        public string FreightCode { get; set; }

        public string WarehouseCode { get; set; }

        public string PlaceName { get; set; }

        public string MetroStation { get; set; }

        public string Street { get; set; }

        public string Building { get; set; }

        public string AdditionalInfo { get; set; }

        #endregion
    }
}
