using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Factories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.HERBALIFE_NEWS)]
    public class HerbalifeNewsViewComponent : ViewComponent
    {
        #region Field

        private readonly INewsBlockModelFactory _newsBlockModelFactory;

        #endregion

        #region Ctor

        public HerbalifeNewsViewComponent(INewsBlockModelFactory newsBlockModelFactory)
        {
            _newsBlockModelFactory = newsBlockModelFactory;
        }

        #endregion

        #region Method

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _newsBlockModelFactory.PrepareNewsBlockModelAsync();

            return View(model);
        }

        #endregion
    }
}
