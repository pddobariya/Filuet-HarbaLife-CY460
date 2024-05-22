namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class SubmitOrderRequest
    {
        #region Properties

        public OrderHeader OrderHeader { get; set; }

        public OrderLines OrderLines { get; set; }

        public OrderPayment OrderPayment { get; set; }

        #endregion
    }
}
