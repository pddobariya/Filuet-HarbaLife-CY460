using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class ProductInventoryRequest
    {
        #region Properties

        public string OrderType { get; set; }

        public CountryCodes CountryCode { get; set; }

        #endregion
    }
}
