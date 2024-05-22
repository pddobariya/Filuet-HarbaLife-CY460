using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Attributes;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    [TypeScriptModel]
    public class ShippingComputationOptionModel
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "warehouseCode")]
        public string WarehouseCode { get; set; }

        [JsonProperty(PropertyName = "processingLocationCode")]
        public string ProcessingLocationCode { get; set; }

        [JsonProperty(PropertyName = "isSalesCenter")]
        public bool IsSalesCenter { get; set; }

        [JsonProperty(PropertyName = "salesCenterId")]
        public int? SalesCenterId { get; set; }

        [JsonProperty(PropertyName = "isSelected")]
        public bool IsSelected { get; set; }

        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }

        public string FlagImageFileName { get; set; }

        #endregion
    }
}
