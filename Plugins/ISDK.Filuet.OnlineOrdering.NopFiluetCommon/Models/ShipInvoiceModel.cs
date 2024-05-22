using Nop.Web.Framework.Models;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public record ShipInvoiceModel : BaseNopModel
    {
        #region Properties

        public bool? IsShipInvoiceWithOrder { get; set; }

        #endregion
    }
}
