using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class SubmitOrderResponse
    {
        #region Properties

        public OrderSubmitStatuses OrderStatus { get; set; }

        public string OrderNumber { get; set; }

        #endregion
    }
}
