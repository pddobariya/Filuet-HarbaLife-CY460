using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor
{
    public class DistributorFOPPurchasingLimitsResponse
    {
        #region Properties

        public DSFOPLimits DSFOPLimits { get; set; }

        public List<DSPurchasingLimitItem> DSPurchasingLimits { get; set; }

        public bool InFopPeriod { get; set; }

        public List<Error> Errors { get; set; }


        #endregion
    }
}
