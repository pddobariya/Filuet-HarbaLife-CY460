using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.MOB_MENU)]
    public class MobMenuViewComponent : NopViewComponent
    {
        #region Field

        private readonly ICatalogModelFactory _catalogModelFactory;

        #endregion

        #region Ctor

        public MobMenuViewComponent(ICatalogModelFactory catalogModelFactory)
        {
            _catalogModelFactory = catalogModelFactory;
        }

        #endregion

        #region Method

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(int? productThumbPictureSize)
        {
            var model = await _catalogModelFactory.PrepareTopMenuModelAsync();
            return View(model);
        }

        #endregion

    }
}
