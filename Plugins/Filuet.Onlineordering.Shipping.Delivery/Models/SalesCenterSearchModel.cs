using Nop.Web.Framework.Models;

namespace Filuet.Onlineordering.Shipping.Delivery.Models
{
    public partial record SalesCenterSearchModel : BaseSearchModel
    {
        #region Properties

        public int LanguageId { get; set; }

        #endregion
    }
}
