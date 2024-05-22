using Nop.Web.Models.Catalog;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Catalog
{
    public record FiluetProductDetailsModel : ProductDetailsModel
    {
        #region Properties

        public string EarnBasePrice { get; set; }

        public string BasicRetailPrice { get; set; }

        public string ProductForOrderCategory { get; set; }

        #endregion
    }
}
