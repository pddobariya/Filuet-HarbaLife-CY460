using ISDK.Filuet.ExternalSSOAuthPlugin.Enum;
using ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using Nop.Services.Logging;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Services
{
    public class DistributorRestrictionService : IDistributorRestrictionService
    {
        #region Fields
        private readonly ILogger _logger;
        private readonly SSOAuthPluginSettings _settings;
        #endregion

        #region Ctor

        public DistributorRestrictionService(ILogger logger, 
            SSOAuthPluginSettings settings)
        {
            _logger = logger;
            _settings = settings;
        }

        #endregion

        #region Methods
        //public async Task<DsRestriction> CheckDistributorRestrictionAsync(DistributorDetailedProfileResponse distributorDetailed)
        //{
        //    var dsRestriction = DsRestriction.None;

        //    if (distributorDetailed == null ||
        //        string.IsNullOrWhiteSpace(distributorDetailed.Email) ||
        //        string.IsNullOrWhiteSpace(distributorDetailed.Phone))
        //    {
        //        return DsRestriction.WithoutEmail;
        //    }

        //    if (distributorDetailed.Flags.DistributorStatus != "Completed" && distributorDetailed.Flags.DistributorStatus != "Complete")
        //    {
        //        return DsRestriction.UnCompleted;
        //    }

        //    if (distributorDetailed.Flags != null && distributorDetailed.Flags.CantBuy)
        //    {
        //        return DsRestriction.CantBuy;
        //    }

        //    if (distributorDetailed.Flags != null && distributorDetailed.Flags.IsDeleted)
        //    {
        //        return DsRestriction.IsDeleted;
        //    }

        //    if (distributorDetailed.DsType.ToUpper() == _settings.AllowedDsTypes && !_settings.AllowedResidenceCountry.Contains(distributorDetailed.ResidenceCountryCode))
        //    {
        //        return DsRestriction.DsTypeCountry;
        //    }

        //    if (distributorDetailed.ResidenceCountryCode == _settings.DeniedResidenceCountry && _settings.DeniedDsTypes.Contains(distributorDetailed.DsType))
        //    {
        //        return DsRestriction.DsTypeCountry;
        //    }

        //    return dsRestriction;
        //}

        public DsRestriction CheckDistributorRestrictionAsync(DistributorDetailedProfileResponse distributorDetailed)
        {
            var dsRestriction = DsRestriction.None;

            if (distributorDetailed == null ||
                string.IsNullOrWhiteSpace(distributorDetailed.Email) ||
                string.IsNullOrWhiteSpace(distributorDetailed.Phone))
            {
                return DsRestriction.WithoutEmail;
            }

            if (distributorDetailed.Flags.DistributorStatus != "Completed" && distributorDetailed.Flags.DistributorStatus != "Complete")
            {
                return DsRestriction.UnCompleted;
            }

            if (distributorDetailed.Flags != null && distributorDetailed.Flags.CantBuy)
            {
                return DsRestriction.CantBuy;
            }

            if (distributorDetailed.Flags != null && distributorDetailed.Flags.IsDeleted)
            {
                return DsRestriction.IsDeleted;
            }

            if (distributorDetailed.DsType.ToUpper() == _settings.AllowedDsTypes && !_settings.AllowedResidenceCountry.Contains(distributorDetailed.ResidenceCountryCode))
            {
                return DsRestriction.DsTypeCountry;
            }

            if (distributorDetailed.ResidenceCountryCode == _settings.DeniedResidenceCountry && _settings.DeniedDsTypes.Contains(distributorDetailed.DsType))
            {
                return DsRestriction.DsTypeCountry;
            }

            return dsRestriction;
        }

        #endregion
    }
}
