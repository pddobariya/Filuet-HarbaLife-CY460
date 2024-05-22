using Nop.Web.Models.Media;
using Nop.Web.Models.Order;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order
{
    public record FiluetOrderItemModel : OrderDetailsModel.OrderItemModel
    {
        #region Properties

        public PictureModel PictureModel { get; set; }
        public string ShortDescription { get; set; }

        #endregion
    }
}
