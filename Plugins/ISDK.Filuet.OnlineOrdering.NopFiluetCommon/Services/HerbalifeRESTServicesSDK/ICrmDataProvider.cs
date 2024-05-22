using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK
{

    /// <summary>
    /// Access to CRM (Customer Relationship Management)
    /// </summary>
    public partial interface ICrmDataProvider
    {
        #region Methods

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileSSOAsync(string accessToken);

        Task<DistributorProfileResponse> GetDistributorProfileAsync(string accessToken);

        Task<DistributorFullProfile> GetDistributorFullProfileAsync(string accessToken);

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(string accessToken);

        Task<DistributorVolumeResponse> GetDistributorVolumeAsync(string accessToken);

        /// <summary>
        /// The get distributor fop purchasing limits.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="DistributorFOPPurchasingLimitsResponse"/>.
        /// </returns>
        Task<DistributorFOPPurchasingLimitsResponse> GetDistributorFOPPurchasingLimitsAsync(DistributorFOPPurchasingLimitsRequest request);

        /// <summary>
        /// The get shopping cart totals.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="ShoppingCartTotalsResponse"/>.
        /// </returns>
        Task<ShoppingCartTotalsResponse> GetShoppingCartTotalsAsync(ShoppingCartTotalsRequest request);

        /// <summary>
        /// The submit order.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="SubmitOrderResponse"/>.
        /// </returns>
        Task<SubmitOrderResponse> SubmitOrderAsync(SubmitOrderRequest request);

        /// <summary>
        /// The get dual month status.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="DualMonthStatusResponse"/>.
        /// </returns>
        Task<DualMonthStatusResponse> GetDualMonthStatusAsync(DualMonthStatusRequest request);

        #endregion
    }
}