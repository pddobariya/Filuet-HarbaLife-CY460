using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Factories;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Controllers
{
    public class PriceFilterController : BasePluginController
    {
        #region Fields

        private readonly PriceFilterModelFactory _priceFilterModelFactory;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public PriceFilterController(PriceFilterModelFactory priceFilterModelFactory, ISettingService settingService)
        {
            _priceFilterModelFactory = priceFilterModelFactory;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = await _priceFilterModelFactory.PreparePriceRangeListSearchModelAsync(new PriceRangeSearchModel());
            await _settingService.ClearCacheAsync();
            return View("~/Plugins/Catalogue.PriceFilter/Views/PriceFilter/Configure.cshtml", model);
        }

        #endregion
    }
}
