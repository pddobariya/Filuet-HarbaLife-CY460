using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Catalog
{
    public record FiluetAdminProductModel : ProductModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.EarnBasePrice")]
        public decimal? EarnBasePrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.BasicRetailPrice")]
        public decimal? BasicRetailPrice { get; set; }

        [NopResourceDisplayName("Admin.Catalog.Products.Fields.ProductForOrderCategory")]
        public string ProductForOrderCategory { get; set; }

        #endregion
    }
}
