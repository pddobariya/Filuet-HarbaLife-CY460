using Newtonsoft.Json;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Models
{
    public class FiluetCountryModel
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty(PropertyName = "iso_code")]
        public string IsoCode { get; internal set; }

        #endregion
    }
}
