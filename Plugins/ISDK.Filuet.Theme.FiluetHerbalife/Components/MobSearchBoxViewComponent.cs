using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    public class MobSearchBoxViewComponent : NopViewComponent
    {
        #region Fields

        private readonly ICatalogModelFactory _catalogModelFactory;

        #endregion

        #region Ctor

        public MobSearchBoxViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            _catalogModelFactory = catalogModelFactory;
        }

        #endregion

        #region Method

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _catalogModelFactory.PrepareSearchBoxModelAsync();
            return View(model);
        }

        #endregion
    }
}
