using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class ProductCatalogResponse
    {
        #region Properties
        
        public List<ProductCatalogItem> Items { get; set; }

        public List<Error> Errors { get; set; }

        #endregion
    }
}
