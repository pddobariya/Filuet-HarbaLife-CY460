namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class WarehouseAndFreightCodesResponse
    {
        #region Properties

        public WarehouseAndFreightCodes WarehouseAndFreightCodes { get; set; }

        public bool IsExpressDelivery { get; set; }

        public string PostalCode { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }

        #endregion
    }
}
