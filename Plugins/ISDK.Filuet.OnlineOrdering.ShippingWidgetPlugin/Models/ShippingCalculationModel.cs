using System.Collections.Generic;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure.Attributes;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    [TypeScriptModel]
    public class ShippingCalculationModel 
    {
        #region Properties

        [JsonProperty(PropertyName = "options")]
        public List<ShippingComputationOptionModel> Options { get; set; }

        [JsonProperty(PropertyName = "isAllowMonthSelect")]
        public bool IsAllowMonthSelect { get; set; }

        [JsonProperty(PropertyName = "availableMonths")]
        public List<ShippingCalculationMonthModel> AvailableMonths { get; set; }

        public ShippingCalculationModel()
        {
            Options = new List<ShippingComputationOptionModel>();
            AvailableMonths = new List<ShippingCalculationMonthModel>();
        }

        #endregion
    }
}
