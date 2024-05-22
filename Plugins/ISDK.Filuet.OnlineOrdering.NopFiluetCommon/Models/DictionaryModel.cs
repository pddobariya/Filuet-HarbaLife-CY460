using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Attributes;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    [TypeScriptModel]
    public class DictionaryModel
    {
        #region Properties

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonIgnore]
        public int IdInt
        {
            get
            {
                return int.TryParse(Id, out var id) ? id : default(int);
            }
        }

        #endregion
    }
}
