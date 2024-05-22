using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.ShoppingCart
{
    /// <summary>
    /// ExtendedShoppingCartModel
    /// </summary>
    public record FiluetShoppingCartModel : ShoppingCartModel
    {
        #region Ctor

        public FiluetShoppingCartModel()
        {
            CartIsValid = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Errors
        /// </summary>
        public IList<string> Errors { get; set; }
        public bool CartIsValid { get; set; }
        public bool ShowApfPayMessage { get; internal set; }
        public bool IsNotResident { get; internal set; }
        public string LandingToken { get; internal set; }
        public string[] IsCartValid { get; internal set; }

        public bool ShowRepeatOrderDialog { get; internal set; }

        public int LastOrderId { get; internal set; }

        #endregion
    }
}