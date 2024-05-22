using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK
{
    public class ProductCatalogItem
    {
        #region Properties

        public Enums.HerbalifeRESTServicesSDK.CountryCodes CountryCode { get; set; }

        public string SellingSku { get; set; }

        public decimal? EarnBase { get; set; }

        public decimal? UnitTaxBase { get; set; }

        public string ProductUOMCode { get; set; }

        public decimal? ListPrice { get; set; }

        public decimal? VolumePoints { get; set; }

        public CurrencyCodes CurrencyCode { get; set; }

        public decimal? FreightExFlag { get; set; }

        public bool IsEventItem { get; set; }

        #endregion
    }
}
