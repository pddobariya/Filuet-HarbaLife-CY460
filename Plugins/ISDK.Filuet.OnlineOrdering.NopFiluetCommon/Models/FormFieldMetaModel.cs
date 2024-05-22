using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.Attributes;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    [TypeScriptModel]
    public class FormFieldMetaModel
    {
        #region Properties

        [JsonProperty(PropertyName = "nameResourceKey")]
        public string NameResourceKey { get; set; }

        [JsonProperty(PropertyName = "helptextResourceKey")]
        public string HelptextResourceKey { get; set; }

        [JsonProperty(PropertyName = "placeholderResourceKey")]
        public string PlaceholderResourceKey { get; set; }

        [JsonProperty(PropertyName = "controlType")]
        public int ControlType { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        #endregion
    }
}
