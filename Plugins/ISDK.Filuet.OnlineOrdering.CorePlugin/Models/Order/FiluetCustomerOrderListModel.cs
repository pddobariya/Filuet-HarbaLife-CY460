using Nop.Web.Models.Order;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order
{
    public record FiluetCustomerOrderListModel
    {
        #region Ctor

        public FiluetCustomerOrderListModel()
        {
            Orders = new List<FiluetOrderDetailsModel>();
            PagingFilteringContext = new OrderPagingFilteringModel();
        }

        #endregion

        #region Properties

        public IList<FiluetOrderDetailsModel> Orders { get; set; }

        public OrderPagingFilteringModel PagingFilteringContext { get; set; }

        #endregion

        #region Nested classes

        public record FiluetOrderDetailsModel : CustomerOrderListModel.OrderDetailsModel
        {
            public string FusionOrderNumber { get; set; }

            public string VolumePoints { get; set; }

            public string OrderStatusClass { get; set; }
        }

        #endregion
    }
}
