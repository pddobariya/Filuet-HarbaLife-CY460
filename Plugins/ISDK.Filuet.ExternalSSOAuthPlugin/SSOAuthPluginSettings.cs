using Nop.Core.Configuration;

namespace ISDK.Filuet.ExternalSSOAuthPlugin
{
    public class SSOAuthPluginSettings : ISettings
    {
        #region Properties

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ApiUrl { get; set; }

        public string AuthorizationEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string WhitelistedCountries { get; set; }

        public bool DenyNoResident { get; set; }

        public bool DenyEntryToUnpaidAPF { get; set; }

        public bool IsEnableCountryRestrictions { get; set; }

        public bool IsProhibitedForNotResident { get; set; }

        public bool UpdateCountryDisRole { get; set; } = true;

        public string AllowedDsTypes { get; set; }
        public string AllowedResidenceCountry { get; set; }
        public string DeniedDsTypes { get; set; }
        public string DeniedResidenceCountry { get; set; }

        #endregion

    }
}
