using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class PostamatsResponse
    {
        #region Properties

        public List<PostamatItem> Items { get; set; }

        public List<Error> Errors { get; set; }

        #endregion
    }
}
