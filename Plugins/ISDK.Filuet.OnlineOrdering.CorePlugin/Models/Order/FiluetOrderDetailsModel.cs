using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.ShoppingCart;
using Nop.Web.Models.Order;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order
{
    public record FiluetOrderDetailsModel: OrderDetailsModel
    {
        #region Ctor

        public FiluetOrderDetailsModel()
        {
            OrderTotals = new FiluetOrderTotalsModel();
        }

        #endregion

        #region Properties

        public string FusionOrderId { get; set; }
        public string CustomerName { get; set; }
        
        public string CustomerEmail { get; set; }

        //shipping data
        public string DeliveryOperator { get; set; }
        public string ShipToZipcode { get; set; }
        public string ShipToCountryCode { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToAddress { get; set; }
        public string ShipToPostamatId { get; set; }
        public string ShipToFullname { get; set; }
        public string ShipToPhone { get; set; }
        public string ShipToTimeslot { get; set; }
        public bool IsShipInvoiceWithOrder { get; set; }

        public string WelcomeTemplate { get; set; }
        public string ThankYouTemplate { get; set; }

        public FiluetOrderTotalsModel OrderTotals { get; set; }
        
        public string Comment { get; set; }

        public IEnumerable<OrderStatusDto> OrderStatusDtos { get; set; }

        public new IList<FiluetOrderItemModel> Items { get; set; }

        #endregion
    }
}
