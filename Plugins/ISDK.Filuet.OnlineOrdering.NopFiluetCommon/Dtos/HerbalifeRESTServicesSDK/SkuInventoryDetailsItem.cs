using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class SkuInventoryDetailsItem
    {
        #region Properties

        /// <summary>
        /// SKU
        /// </summary>
        public string SkuName { get; set; }

        public bool IsValid { get; set; }

        /// <summary>
        /// Requested quantity is available
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Block state
        /// </summary>
        public bool IsBlocked { get; set; }

        public string CrossSellSku { get; set; }

        /// <summary>
        /// Stock quantity
        /// </summary>
        public int AvailableQuantity { get; set; }

        public ReasonCodes ReasonCode { get; set; }

        public string ReasonMessage { get; set; }

        #endregion
    }
}
