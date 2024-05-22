using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public class IpApiResponseModel
    {
        #region Properties

        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("regionName")]
        public string RegionName { get; set; }

        #endregion
    }
}
