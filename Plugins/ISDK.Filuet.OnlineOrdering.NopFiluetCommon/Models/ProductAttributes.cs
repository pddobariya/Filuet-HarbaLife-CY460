namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    /// <summary>
    /// Product attributes
    /// </summary>
    public class ProductAttributeModel
    {
        #region Properties

        public string Sku { get; set; }

        public string ManufacturerPartNumber { get; set; }

        public string Gtin { get; set; }

        public decimal? Vp { get; set; }

        #endregion
    }
}
