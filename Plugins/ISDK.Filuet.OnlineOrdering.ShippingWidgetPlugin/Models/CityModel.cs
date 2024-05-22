using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure.Attributes;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    [TypeScriptModel]
    public class CityModel
    {
        #region Properties

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        #endregion
    }
}