using Filuet.Hrbl.Ordering.Abstractions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    public class RestApiClient : IRestApiClient
    {
        #region Fields

        private readonly IHrblOrderingAdapter _hrblOrderingAdapter;

        #endregion

        #region Ctor

        public RestApiClient()
        {
            _hrblOrderingAdapter = ConnectionBuilder.GetRestApiAdapter();
        }

        #endregion

        #region Methods

        public async Task<FOPPurchasingLimitsResult> GetDSFOPPurchasingLimits(string memberId, string processingCountryCode)
        {
            var fopLimits = await _hrblOrderingAdapter.GetDSFOPPurchasingLimits(memberId, processingCountryCode);
            return fopLimits;
        }

        public async Task<DistributorProfile> GetProfile(string distributorId)
        {
            return await _hrblOrderingAdapter.GetProfile(distributorId);
        }

        #endregion
    }
}
