using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class DistributorInfoResponse
    {
        #region Properties

        [JsonProperty("MemberId")]
        public string Id { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("ProcessingCountryCode")]
        public string ProcessingCountryCode { get; set; }

        [JsonProperty("ResidenceCountryCode")]
        public string ResidenceCountryCode { get; set; }

        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("SponsorId")]
        public string SponsorId { get; set; }

        #endregion
    }
}
