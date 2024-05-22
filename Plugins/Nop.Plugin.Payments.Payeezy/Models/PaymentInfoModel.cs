using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.Payeezy.Models
{
    public record PaymentInfoModel: BaseNopModel
    {
        #region Properties

        [NopResourceDisplayName("Plugins.Payments.Payeezy.Fields.InvoiceEmail")]
        public string InvoiceEmail { get; set; }
        
        public bool? IsShipInvoiceWithOrder { get; set; }

        #endregion
    }
}
