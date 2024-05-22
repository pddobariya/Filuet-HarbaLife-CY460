using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.ClientId")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.ClientSecret")]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.AuthorizationEndpoint")]
        public string AuthorizationEndpoint { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.TokenEndpoint")]
        public string TokenEndpoint { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.ApiUrl")]
        public string ApiUrl { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.WhitelistedCountries")]
        public string WhitelistedCountries { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.DenyNoResident")]
        public bool DenyNoResident { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.DenyEntryToUnpaidAPF")]
        public bool DenyEntryToUnpaidAPF { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.IsEnableCountryRestrictions")]
        public bool IsEnableCountryRestrictions { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.IsProhibitedForNotResident")]
        public bool IsProhibitedForNotResident { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.UpdateCountryDisRole")]
        public bool UpdateCountryDisRole { get; set; } = true;


        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.AllowedDsTypes")]
        public string AllowedDsTypes { get; set; }
        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.AllowedResidenceCountry")]
        public string AllowedResidenceCountry { get; set; }


        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.DeniedDsTypes")]
        public string DeniedDsTypes { get; set; }
        [NopResourceDisplayName("Plugins.ExternalAuth.SSO.DeniedResidenceCountry")]
        public string DeniedResidenceCountry { get; set; }

        #endregion
    }
}