using Nop.Web.Models.ShoppingCart;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.ShoppingCart
{
    /// <summary>
    /// ExtendedShoppingCartModel
    /// </summary>
    public record FiluetOrderTotalsModel : OrderTotalsModel
    {
        #region Properties

        public bool IsShowExtraData { get; set; }

        public string OrderMonth { get; set; }

        public string OrderMonthVolume { get; set; }

        public string DiscountRate { get; set; }

        public string OrderVolumePoints { get; set; }

        public string ProductEarnBase { get; set; }

        public string DiscountedBasePrice { get; set; }

        public string DeliveryPrice { get; set; }

        public string FlatFee { get; set; }

        public bool ShowDeliveryPrice { get; set; }

        #endregion
    }
}