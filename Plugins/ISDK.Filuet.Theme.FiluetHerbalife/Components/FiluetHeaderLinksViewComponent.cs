using ISDK.Filuet.Theme.FiluetHerbalife.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    public class FiluetHeaderLinksViewComponent : NopViewComponent
    {
        #region Field

        private readonly ICommonModelFactory _commonModelFactory;

        #endregion

        #region Ctor

        public FiluetHeaderLinksViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        #endregion

        #region Method

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _commonModelFactory.PrepareHeaderLinksModelAsync();

            var filuetModel = model as FiluetHeaderLinksModel;

            return View(filuetModel);
        }

        #endregion

    }
}
