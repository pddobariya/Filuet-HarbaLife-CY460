using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class OrderLines
    {
        #region Properties

        public string ProcessingLocation { get; set; }

        public List<OrderLine> Lines { get; set; }

        #endregion
    }
}
