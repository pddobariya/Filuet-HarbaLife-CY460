using Nop.Core;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain
{
    public class PriceRange : BaseEntity
    {
        #region Properties

        public string Name { get; set; }

        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }

        public int OrderNumber { get; set; }

        #endregion
    }
}
