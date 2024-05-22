using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.PRODUCT_REVIEWS)]
    public class ProductReviewsViewComponents : NopViewComponent
    {
        #region Fields

        public readonly IProductService _productService;
        public readonly IProductModelFactory _productModelFactory;

        #endregion

        #region Ctor

        public ProductReviewsViewComponents(IProductService productService, IProductModelFactory productModelFactory)
        {
            _productService = productService;
            _productModelFactory = productModelFactory;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);

            var model = new ProductReviewsModel();
            model = await _productModelFactory.PrepareProductReviewsModelAsync(model, product);

            return View(model);
        }

        #endregion
    }
}
