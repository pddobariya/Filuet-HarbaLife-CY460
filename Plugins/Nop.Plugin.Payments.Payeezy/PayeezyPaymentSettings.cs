using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Payeezy
{
    public class PayeezyPaymentSettings: ISettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether API is in sandbox (test) or production mode
        /// </summary>
        public bool IsSandbox { get; set; }

        /// <summary>
        /// Gets or sets a value indicating base URL for sandbox API requests
        /// </summary>
        public string APISandboxEndpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating base URL for production API requests
        /// </summary>
        public string APIProductionEndpoint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating thumbprint of API client certificate in Windows key store. Should not be modified unless certificate is changed in key store and person is aware what provided value is
        /// </summary>
        public string SandboxClientCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating thumbprint of API client certificate in Windows key store. Should not be modified unless certificate is changed in key store and person is aware what provided value is
        /// </summary>
        public string ProductionClientCertificateThumbprint { get; set; }

        /// <summary>
        /// Gets or sets a value of URL to which user is redirected to enter card data and perform payment
        /// </summary>
        public string SandboxPaymentRedirectUrl { get; set; }

        /// <summary>
        /// Gets or sets a value of URL to which user is redirected to enter card data and perform payment
        /// </summary>
        public string ProductionPaymentRedirectUrl { get; set; }


        /// <summary>
        /// Gets or sets a value indicating plugin controller name
        /// </summary>
        public string PluginControllerName { get; set; }


        /// <summary>
        /// Gets or sets a value indicating return ok URL in sandbox
        /// </summary>
        public string APISandboxReturnOkAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating return ok URL in production
        /// </summary>
        public string APIProductionReturnOkAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating return fail URL in sandbox
        /// </summary>
        public string APISandboxReturnFailAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating return fail URL in production
        /// </summary>
        public string APIProductionReturnFailAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating mobile payment action in sandbox
        /// </summary>
        public string APISandboxMobilePaymentAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating mobile payment action in production
        /// </summary>
        public string APIProductionMobilePaymentAction { get; set; }

        
        /// <summary>
        /// Gets or sets a value indicating mobile payment query string
        /// </summary>
        public string MobilePaymentQuerystring { get; set; }

        #endregion
    }
}
