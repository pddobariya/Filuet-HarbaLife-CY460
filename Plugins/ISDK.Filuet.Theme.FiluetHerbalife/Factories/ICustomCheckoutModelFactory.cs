using ISDK.Filuet.Theme.FiluetHerbalife.Models.Checkout;
using Nop.Core.Domain.Orders;
using Nop.Web.Models.Checkout;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Factories
{
    public partial interface ICustomCheckoutModelFactory
    {
        #region Methods

        Task<CheckoutBillingAddressModel> PrepareBillingAddressModelAsync(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false,
            string overrideAttributesXml = "");
        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the one page checkout model
        /// </returns>
        Task<CustomOnePageCheckoutModel> PrepareOnePageCheckoutModelAsync(IList<ShoppingCartItem> cart);

        #endregion
        
    }
}
