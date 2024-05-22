using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class ShoppingCartTotalsResponse
    {
        #region Properties

        public OrderPriceHeader OrderPriceHeader { get; set; }

        public OrderPriceLines OrderPriceLines { get; set; }

        public List<ErrorMessage> Errors { get; set; }

        #endregion
    }
}
