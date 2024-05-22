using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class ProductInventoryItem
    {
        #region Properties

        public float? ConversionFactor { get; set; }

        public CountryCodes CountryCode { get; set; }

        public bool IsBlocked { get; set; }

        public int? QuantityAvailable { get; set; }

        public int? QuantityOnHand { get; set; }

        public int? QuantityReserved { get; set; }

        public string ReturnCode { get; set; }

        public string ReturnMessage { get; set; }

        public string Sku { get; set; }

        public string StockingInitOfMeasure { get; set; }

        public int Threshold { get; set; }

        public string WarehouseCode { get; set; }

        public string BlockedReason { get; set; }

        public bool SplitAllowed { get; set; }

        #endregion
    }
}
