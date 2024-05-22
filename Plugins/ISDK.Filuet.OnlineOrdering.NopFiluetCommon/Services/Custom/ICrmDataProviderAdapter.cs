using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface ICrmDataProviderAdapter
    {
        #region Methods

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileSSO(string accessToken);

        Task<DistributorProfileResponse> GetDistributorProfile(Customer customer);

        Task<DistributorFullProfile> GetDistributorFullProfileAsync(Customer customer);

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(Customer customer);
        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfile(string accessToken);
        Task<DistributorVolumeResponse> GetDistributorVolume(Customer customer);

        void InvalidateDistributorInfo(Customer customer);

        Task<DistributorFopLimitsModel> GetDistributorVPLimitsSSO(string memberId, string processingCountryCode);

        #endregion
    }
}
