using Filuet.Hrbl.Ordering.Abstractions;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    public interface IRestApiClient
    {
        #region Methods

       Task<DistributorProfile> GetProfile(string distributorId);
        Task<FOPPurchasingLimitsResult> GetDSFOPPurchasingLimits(string memberId, string processingCountryCode);
        
        #endregion
    }
}
