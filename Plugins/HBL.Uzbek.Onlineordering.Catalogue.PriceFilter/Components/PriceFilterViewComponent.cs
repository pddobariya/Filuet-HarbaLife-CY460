using System.Linq;
using System.Threading.Tasks;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Components
{
    public class PriceFilterViewComponent : NopViewComponent
    {
        #region Fields

        private readonly PriceRangeFilterService _priceRangeFilterService;

        #endregion

        #region Ctor

        public PriceFilterViewComponent(
            PriceRangeFilterService priceRangeFilterService)
        {
            _priceRangeFilterService = priceRangeFilterService;
        }

        #endregion

        #region Methods

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var priceRanges = await _priceRangeFilterService.GetPriceRangesAsync();

            if (!priceRanges.Any())
                return Content(string.Empty);

            var modelTasks = priceRanges.Select(async pr => new PriceRangeModel
            {
                Id = pr.Id,
                Name = string.Format(await pr.Name.ToLocalizedStringAsync(), pr.MinPrice, pr.MaxPrice),
                MinPrice = pr.MinPrice,
                MaxPrice = pr.MaxPrice,
                OrderNumber = pr.OrderNumber
            });

            var models = await Task.WhenAll(modelTasks);

            return View(models.OrderBy(pr => pr.OrderNumber).ToList());
        }

        #endregion
    }
}
