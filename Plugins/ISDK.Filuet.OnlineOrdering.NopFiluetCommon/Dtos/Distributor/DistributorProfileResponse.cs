using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class DistributorProfileResponse : DistributorInfoResponse
    {
        #region Properties

        [JsonProperty("MailingCountryCode")]
        public string MailingCountryCode { get; set; }
                
        [JsonProperty("TypeCode")]
        public string Type { get; set; }

        [JsonProperty("SubTypeCode")]
        public string SubType { get; set; }

        [JsonProperty("ResidenceCountry")]
        public string CountryOfResidence { get; set; }

        [JsonProperty("CountryOfProcessing")]
        public string CountryOfProcessing { get; set; }

        #endregion
    }
}
