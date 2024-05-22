using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;
using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface ISpRoleChecker
    {
        #region Methods

        Task<bool> CheckAndUpdateAsync(Customer customer);
        Task<bool> SetupAsync(Customer customer, DistributorTypes distributorTypes);

        #endregion
    }
}