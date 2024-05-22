using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class OrderPriceLines
    {
        #region Properties

        public string ProcessingLocation { get; set; }

        public List<OrderPriceLine> PriceLines { get; set; }

        #endregion
    }
}
