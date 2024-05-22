using ISDK.Filuet.Theme.FiluetHerbalife.Constants;
using ISDK.Filuet.Theme.FiluetHerbalife.Factories;
using ISDK.Filuet.Theme.FiluetHerbalife.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Components
{
    [ViewComponent(Name = ComponentNames.CART_SUMMARY_BAR)]
    public class CartSummaryBarViewComponent : NopViewComponent
    {
        #region Field

        private readonly IFiluetCartModelFactory _cartModelFactory;

        #endregion

        #region Ctor

        public CartSummaryBarViewComponent(IFiluetCartModelFactory cartModelFactory)
        {
            _cartModelFactory = cartModelFactory;
        }

        #endregion

        #region Method

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            CartSummaryBarModel model = await _cartModelFactory.PrepareCartSummaryBarModel();
            return View(model);
        }

        #endregion

    }
}
