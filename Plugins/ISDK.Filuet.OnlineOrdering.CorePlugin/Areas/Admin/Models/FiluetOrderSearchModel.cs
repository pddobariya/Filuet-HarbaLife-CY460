using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Areas.Admin.Models
{
    public record FiluetOrderSearchModel : OrderSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Orders.List.CustomOrderNumber")]
        public string CustomOrderNumber { get; set; }

        #endregion
    }
}
