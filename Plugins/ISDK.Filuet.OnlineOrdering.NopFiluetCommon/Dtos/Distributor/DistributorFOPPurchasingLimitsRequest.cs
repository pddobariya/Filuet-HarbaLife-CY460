using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class DistributorFOPPurchasingLimitsRequest
    {
        #region Properties

        public string DistributorId { get; set; }

        public CountryCodes CountryCode { get; set; }

        public string OrderMonth { get; set; }

        #endregion
    }
}
