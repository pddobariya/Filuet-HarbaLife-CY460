using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using Microsoft.AspNetCore.Authentication;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders
{
    public interface IResidentChecker
    {
        #region Methods

        bool CheckIfAuthenticatedCustomerIsResident(AuthenticateResult authenticateResult);
        bool NeedToCheckIfAuthenticatedCustomerIsResident { get; }
        bool IsProhibitedForNotResident { get; }
        Task CheckAndUpdateAsync(Customer customer, AuthenticateResult authenticateResult, DistributorDetailedProfileResponse distibutorInfo);

        #endregion
    }
}
