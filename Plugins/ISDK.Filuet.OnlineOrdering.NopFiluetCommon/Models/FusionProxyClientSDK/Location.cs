using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FusionProxyClientSDK
{
    public class Location
    {
        #region Properties

        public string Name { get; set; }
        public string WarehouseNumber { get; set; }
        public string ProcessingLocation { get; set; }
        public int OrgId { get; set; }
        public string OrderTypeId { get; set; }
        public List<string> FreightCodes { get; set; }

        #endregion
    }
}
