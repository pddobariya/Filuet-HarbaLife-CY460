using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models
{
    public partial record PriceRangeModel : BaseNopEntityModel
    {
        #region Properties

        [NopResourceDisplayName("Plugins.PriceFilter.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.PriceFilter.Fields.MinPrice")]
        public decimal MinPrice { get; set; }
        [NopResourceDisplayName("Plugins.PriceFilter.Fields.MaxPrice")]
        public decimal MaxPrice { get; set; }
        [NopResourceDisplayName("Plugins.PriceFilter.Fields.IsActive")]
        public bool IsActive { get; set; }
        [NopResourceDisplayName("Plugins.PriceFilter.Fields.OrderNumber")]
        public int OrderNumber { get; set; }

        #endregion
    }
}
