using Nop.Web.Models.Catalog;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Models.Catalog
{
    public record FiluetProductDetailsModel : ProductDetailsModel
    {
        #region Ctor

        public FiluetProductDetailsModel() : base()
        {
            ProductDescription = new ProductDescription();
        }

        #endregion

        #region Properties

        public string EarnBasePrice { get; set; }

        public string BasicRetailPrice { get; set; }

        public string ProductForOrderCategory { get; set; }

        public ProductDescription ProductDescription { get; set; }

        #endregion
        
    }
}
