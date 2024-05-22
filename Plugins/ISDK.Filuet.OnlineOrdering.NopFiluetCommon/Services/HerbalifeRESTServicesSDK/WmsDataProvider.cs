using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.HerbalifeRESTServicesSDK;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK
{
    public class WmsDataProvider : IWmsDataProvider
    {
        #region Fields

        private HerbalifeRESTServicesClient _client;

        #endregion

        #region Ctor

        public WmsDataProvider(string apiUrl)
        {
            _client = new HerbalifeRESTServicesClient(apiUrl);
        }

        #endregion

        #region Methods

        public Task<PostamatsResponse> GetPostamatsAsync(PostamatsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ProductCatalogResponse> GetProductCatalogAsync(ProductCatalogRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ProductInventoryResponse> GetProductInventoryAsync(ProductInventoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SkuAvailabilityResponse> GetSkuAvailabilityAsync(SkuAvailabilityRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<WarehouseAndFreightCodesResponse> GetWarehouseAndFreightCodesAsync(WarehouseAndFreightCodesRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
