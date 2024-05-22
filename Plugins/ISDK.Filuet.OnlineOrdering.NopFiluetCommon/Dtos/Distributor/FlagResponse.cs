using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class FlagResponse
    {
        #region  Properties

        [JsonProperty("cantBuy")]
        public string CantBuy { get; set; }

        [JsonIgnore]
        public bool? CantBuyValue
        {
            get
            {
                if (bool.TryParse(CantBuy, out var value))
                {
                    return value;
                }
                return null;
            }
        }

        #endregion
    }
}
