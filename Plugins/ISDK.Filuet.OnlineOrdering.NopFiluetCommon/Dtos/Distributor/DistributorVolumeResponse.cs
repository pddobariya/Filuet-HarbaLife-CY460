using System.Globalization;
using Newtonsoft.Json;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class DistributorVolumeResponse
    {
        #region Properties

        [JsonProperty("TV")]
        public string Tv { get; set; }

        [JsonProperty("PV")]
        public string Pv { get; set; }

        [JsonProperty("PPV")]
        public string Ppv { get; set; }

        [JsonIgnore]
        public decimal? TvValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Tv))
                {
                    return null;
                }
                var tv = Tv.Replace(',', '.');
                if (decimal.TryParse(tv, NumberStyles.Any, new CultureInfo("en-En"), out var value))
                {
                    return value;
                }
                return null;
            }
        }

        [JsonIgnore]
        public decimal? PvValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Pv))
                {
                    return null;
                }
                var pv = Pv.Replace(',', '.');
                if (decimal.TryParse(pv, NumberStyles.Any, new CultureInfo("en-En"), out var value))
                {
                    return value;
                }
                return null;
            }
        }

        [JsonIgnore]
        public decimal? PpvValue
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Ppv))
                {
                    return null;
                }
                var ppv = Ppv.Replace(',', '.');
                if (decimal.TryParse(ppv, NumberStyles.Any, new CultureInfo("en-En"), out var value))
                {
                    return value;
                }
                return null;
            }
        }

        #endregion
    }
}
