using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure.Attributes;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    [TypeScriptModel]
    public class ShippingCalculationMonthModel
    {
        #region Properties

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

        [JsonProperty(PropertyName = "isSelected")]
        public bool IsSelected { get; set; }

        #endregion
    }
}
