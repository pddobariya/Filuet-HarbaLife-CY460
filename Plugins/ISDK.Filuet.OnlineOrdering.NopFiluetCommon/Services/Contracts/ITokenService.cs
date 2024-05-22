using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Contracts
{
    public enum ValidateTokenResults
    {
        Undefined,
        Valid,
        InValid,
        Expired
    }

    public interface ITokenService
    {
        #region Methods

        Task<int> GetTokenSessionTimeoutMinutesAsync(Customer customer);

        Task<int> SetTokenAsync(Customer customer, string token);

        Task<RefreshToken> GetTokenAsync(int tokenId);

        Task<string> EncodeMobileTokenAsync(Customer customer, string token);

        Task AuthenticateCustomerByTokenAsync(string token);

        Task<MobileTokenModel> DecodeMobileTokenAsync(string encodedToken);

        Task<ValidateTokenResults> ValidateTokenAsync(int customerId, string token);

        #endregion

    }
}
