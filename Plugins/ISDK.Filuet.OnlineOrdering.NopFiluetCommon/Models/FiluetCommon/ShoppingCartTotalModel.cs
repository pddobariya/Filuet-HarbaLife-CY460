using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Shopping cart recalculation result
    /// </summary>
    public class ShoppingCartTotalModel
    {
        #region Properties

        /// <summary>
        /// Order month
        /// </summary>
        public int OrderMonth { get; set; }
        
        /// <summary>
        /// Order year
        /// </summary>
        public int OrderYear { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Discount
        /// </summary>
        public double Discount { get; set; }

        /// <summary>
        /// Discount
        /// </summary>
        public double DiscountPercent { get; set; }

        /// <summary>
        /// Volume points (VP)
        /// </summary>
        public double VolumePoints { get; set; }

        /// <summary>
        /// VP for the current order
        /// </summary>
        public double CurrentOrderVolumePoints { get; set; }

        /// <summary>
        /// Amount base / Spec: База для расчета цены
        /// </summary>
        public decimal AmountBase { get; set; }

        /// <summary>
        /// Reward amount base / Spec: База для расчета вознаграждения
        /// </summary>
        public decimal RewardAmountBase { get; set; }

        /// <summary>
        /// Total amount / Spec: Сумма со скидкой без учета налогов
        /// </summary>
        public decimal TotalAmount { get; set; }

        public decimal TotalDue { get; set; }

        public decimal DeliveryFlatFee { get; set; }

        public decimal TotalTaxAmount { get; set; }

        public decimal DiscountedBasePrice { get; set; }

        public decimal FreightCharge { get; set; }

        /// <summary>
        /// Errors during the order
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        public IEnumerable<ShoppingCartLineModel> ShoppingCartLines { get; set; }

        #endregion
    }
}
