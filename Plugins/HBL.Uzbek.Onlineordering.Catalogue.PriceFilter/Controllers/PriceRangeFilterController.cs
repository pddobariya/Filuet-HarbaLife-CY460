using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Factories;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Models;
using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class PriceRangeFilterController : BasePluginController
    {
        #region Fields

        private readonly PriceRangeFilterService _service;
        private readonly IPermissionService _permissionService;
        private readonly PriceFilterModelFactory _priceFilterModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public PriceRangeFilterController(PriceRangeFilterService service,
            IPermissionService permissionService,
            PriceFilterModelFactory priceFilterModelFactory,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _service = service;
            _permissionService = permissionService;
            _priceFilterModelFactory = priceFilterModelFactory;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods


        [HttpPost]
        public async Task<IActionResult> PriceRangeList(PriceRangeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            //prepare model
            var model = await _priceFilterModelFactory.PreparePriceRangeListModelAsync(searchModel);

            return Json(model);

        }
        [HttpGet]
        public async Task<IActionResult> PriceCreate()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            //prepare model
            var model = await _priceFilterModelFactory.PrepareRangeModelAsync(new PriceRangeModel(), null);

            return View("~/Plugins/Catalogue.PriceFilter/Views/PriceFilter/PriceCreate.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> PriceCreate(PriceRangeModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            var list = new PriceRange()
            {
                //Id = model.Id,
                Name = model.Name,
                MinPrice = model.MinPrice,
                MaxPrice = model.MaxPrice,
                OrderNumber = model.OrderNumber
            };
            //Insert Record
            await _service.CreatePriceRangeAsync(list);
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            //prepare model
            return RedirectToAction("Configure","PriceFilter", new { area = AreaNames.Admin });
        }


        [HttpPost]
        public async Task<IActionResult> PriceUpdate(PriceRange model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            await _service.UpdatePriceRangeAsync(model);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> PriceDelete(PriceRange model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel))
                return Content("Access denied");

            await _service.DeletePriceRangeAsync(model);

            return new NullJsonResult();
        }

        #endregion
    }
}

