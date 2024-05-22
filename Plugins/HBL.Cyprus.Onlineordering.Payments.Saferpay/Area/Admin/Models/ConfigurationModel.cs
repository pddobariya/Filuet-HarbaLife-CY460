using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;


namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Area.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.Bypass")]
        public bool Bypass { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.APIUrl")]
        public string APIUrl { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.APIUsername")]
        public string APIUsername { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.APIPassword")]
        public string APIPassword { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.APISpecVersion")]
        public string APISpecVersion { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.CustomerId")]
        public string CustomerId { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.TerminalId")]
        public string TerminalId { get; set; }

        [Required]
        [NopResourceDisplayName("Plugins.Payments.Saferpay.Fields.CurrencyCode")]
        public string CurrencyCode { get; set; }

        #endregion
    }
}
