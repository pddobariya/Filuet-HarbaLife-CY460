using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Catalog;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models
{
    public record FiluetSearchModel : SearchModel
    {
        #region Properties

        [NopResourceDisplayName("Search.PriceRange.From")]
        public string SearchPriceRangeFrom { get; set; }
        
        [NopResourceDisplayName("Search.PriceRange.To")]
        public string SearchPriceRangeTo { get; set; }

        #endregion
    }
}
