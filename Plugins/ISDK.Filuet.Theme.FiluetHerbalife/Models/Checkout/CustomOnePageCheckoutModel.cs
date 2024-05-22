using Nop.Web.Framework.Models;
using Nop.Web.Models.Checkout;
using System.Collections.Generic;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models.Checkout
{
    public partial record CustomOnePageCheckoutModel : BaseNopModel
    {
        #region Properties

        public bool ShippingRequired { get; set; }
        public bool DisableBillingAddressCheckoutStep { get; set; }
        public bool DisplayCaptcha { get; set; }
        public bool IsReCaptchaV3 { get; set; }
        public string ReCaptchaPublicKey { get; set; }
        public decimal TotalOrderWeight { get; set; }
        public CheckoutBillingAddressModel BillingAddress { get; set; }
        public string PhonePrefix { get; set; }
        public string PhoneMask { get; set; }
        public IEnumerable<string> Phones { get; set; } = new List<string>();
        public IList<CheckoutPaymentMethodModel.PaymentMethodModel> PaymentMethods { get; set; }

        #endregion
        
    }
}