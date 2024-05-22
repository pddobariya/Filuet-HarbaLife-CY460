using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Payeezy.Models
{
    public record ConfigurationModel: BaseNopModel
    {
        #region Properties

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.IsSandbox")]
        public bool IsSandbox { get; set; }
        public bool IsSandbox_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APISandboxEndpoint")]
        public string APISandboxEndpoint { get; set; }
        public bool APISandboxEndpoint_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APIProductionEndpoint")]
        public string APIProductionEndpoint { get; set; }
        public bool APIProductionEndpoint_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.SandboxClientCertificateThumbprint")]
        public string SandboxClientCertificateThumbprint { get; set; }
        public bool SandboxClientCertificateThumbprint_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.ProductionClientCertificateThumbprint")]
        public string ProductionClientCertificateThumbprint { get; set; }
        public bool ProductionClientCertificateThumbprint_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.SandboxPaymentRedirectUrl")]
        public string SandboxPaymentRedirectUrl { get; set; }
        public bool SandboxPaymentRedirectUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.ProductionPaymentRedirectUrl")]
        public string ProductionPaymentRedirectUrl { get; set; }
        public bool ProductionPaymentRedirectUrl_OverrideForStore { get; set; }


        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.PluginControllerName")]
        public string PluginControllerName { get; set; }
        public bool PluginControllerName_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APISandboxReturnOkAction")]
        public string APISandboxReturnOkAction { get; set; }
        public bool APISandboxReturnOkAction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APIProductionReturnOkAction")]
        public string APIProductionReturnOkAction { get; set; }
        public bool APIProductionReturnOkAction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APISandboxReturnFailAction")]
        public string APISandboxReturnFailAction { get; set; }
        public bool APISandboxReturnFailAction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APIProductionReturnFailAction")]
        public string APIProductionReturnFailAction { get; set; }
        public bool APIProductionReturnFailAction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APISandboxMobilePaymentAction")]
        public string APISandboxMobilePaymentAction { get; set; }
        public bool APISandboxMobilePaymentAction_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.APIProductionMobilePaymentAction")]
        public string APIProductionMobilePaymentAction { get; set; }
        public bool APIProductionMobilePaymentAction_OverrideForStore { get; set; }

        

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.MobilePaymentQuerystring")]
        public string MobilePaymentQuerystring { get; set; }
        public bool MobilePaymentQuerystring_OverrideForStore { get; set; }

        #endregion
    }
}
