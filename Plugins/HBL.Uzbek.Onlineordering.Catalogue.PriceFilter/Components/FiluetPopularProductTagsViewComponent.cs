using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Components
{
    public class FiluetPopularProductTagsViewComponent : NopViewComponent
    {
        #region 

        private readonly ICatalogModelFactory _catalogModelFactory;

        #endregion

        #region Ctor

        public FiluetPopularProductTagsViewComponent(
            ICatalogModelFactory catalogModelFactory)
        {
            _catalogModelFactory = catalogModelFactory;
        }

        #endregion

        #region Methods
        public async Task<IViewComponentResult> InvokeAsync(int tagId)
        {
            var model = await _catalogModelFactory.PreparePopularProductTagsModelAsync();

            if (!model.Tags.Any())
                return Content(string.Empty);

            ViewBag.TagId = tagId;

            return View(model);
        }

        #endregion
    }
}
