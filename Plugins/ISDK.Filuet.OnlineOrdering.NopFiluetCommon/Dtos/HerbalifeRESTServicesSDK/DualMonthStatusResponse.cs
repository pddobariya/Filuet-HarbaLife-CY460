using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class DualMonthStatusResponse
    {
        #region Properties

        public bool IsDualMonthAllowed { get; set; }

        public List<Error> Errors { get; set; }

        #endregion
    }
}
