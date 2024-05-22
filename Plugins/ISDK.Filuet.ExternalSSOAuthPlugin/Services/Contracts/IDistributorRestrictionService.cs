using ISDK.Filuet.ExternalSSOAuthPlugin.Enum;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts
{
    public interface IDistributorRestrictionService
    {
        #region Methods

        // Task<DsRestriction> CheckDistributorRestrictionAsync( DistributorDetailedProfileResponse distributorDetailed);
        DsRestriction CheckDistributorRestrictionAsync(DistributorDetailedProfileResponse distributorDetailed);

        #endregion
    }
}
