using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using System.Collections.Generic;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models
{
    public record PriceFilterModel : BaseNopEntityModel
    {
        #region Ctor

        public PriceFilterModel()
        {
            PictureModel = new PictureModel();
            Products = new List<ProductOverviewModel>();
            CatalogProductsModel = new CatalogProductsModel();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Plugins.PriceFilter.Fields.Name")]
        public string? Name { get; set; }
        [NopResourceDisplayName("Plugins.PriceFilter.Fields.PictureModel")]
        public PictureModel PictureModel { get; set; }

        public CatalogProductsModel CatalogProductsModel { get; set; }
        public IList<ProductOverviewModel> Products { get; set; }

        #endregion
    }
}
