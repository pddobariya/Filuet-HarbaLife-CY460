using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts
{
    public interface IDistributorService
    {
        #region Methods

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileSSOAsync(string accessToken);

        Task<DistributorProfileResponse> GetDistributorProfileAsync(Customer customer);

        Task<DistributorFullProfile> GetDistributorFullProfileAsync(Customer customer);

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(Customer customer);

        Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(string accessToken);

        Task<DistributorVolumeResponse> GetDistributorVolumeAsync(Customer customer);

        void InvalidateDistributorInfoAsync(Customer customer);
        Task<DistributorFopLimitsModel> GetDistributorVPLimitsSSOAsync(string memberId, string processingCountryCode);
        Task SetDistributorRegionalRole(Customer customer, DistributorDetailedProfileResponse profile);
        Task<CustomerLimits> PutDistributorVpLimits(Customer customer, string memberId, string processingCountryCode);
        Task SaveCustomerDataAsync(Customer customer, DistributorDetailedProfileResponse profile);
        Task PutAddressesFromOracleAsync(Customer customer, DistributorDetailedProfileResponse profile);
        Task SetDistributorRegionalRole(Customer customer, int? discount, string countryCode);

        #endregion

    }
}