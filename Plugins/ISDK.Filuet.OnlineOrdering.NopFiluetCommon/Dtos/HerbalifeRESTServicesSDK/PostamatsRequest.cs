using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class PostamatsRequest
    {
        #region Properties

        public CountryCodes Country { get; set; }

        public string PostamatType { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string ZipCode { get; set; }

        #endregion
    }
}
