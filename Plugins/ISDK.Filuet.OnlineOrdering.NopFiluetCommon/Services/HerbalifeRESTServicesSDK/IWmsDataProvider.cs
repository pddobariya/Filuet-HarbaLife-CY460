using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK
{

    /// <summary>
    /// Access to WMS (Warehouse Management System)
    /// </summary>
    public interface IWmsDataProvider
    {
        #region Methods

        /// <summary>
        /// The get sku availability.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="SkuAvailabilityResponse"/>.
        /// </returns>
        Task<SkuAvailabilityResponse> GetSkuAvailabilityAsync(SkuAvailabilityRequest request);

        /// <summary>
        /// The get product catalog.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ProductCatalogResponse"/>.
        /// </returns>
        Task<ProductCatalogResponse> GetProductCatalogAsync(ProductCatalogRequest request);

        /// <summary>
        /// The get postamats.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="PostamatsResponse"/>.
        /// </returns>
        Task<PostamatsResponse> GetPostamatsAsync(PostamatsRequest request);

        /// <summary>
        /// The get warehouse and freight codes.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="WarehouseAndFreightCodesResponse"/>.
        /// </returns>
       Task<WarehouseAndFreightCodesResponse> GetWarehouseAndFreightCodesAsync(WarehouseAndFreightCodesRequest request);

        /// <summary>
        /// The get product inventory.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ProductInventoryResponse"/>.
        /// </returns>
        Task<ProductInventoryResponse> GetProductInventoryAsync(ProductInventoryRequest request);

        #endregion
    }
}