using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Factories;
using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Services;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Validators;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    public class DeliveryAdminController : BasePluginController
    {
        #region Fields

        private readonly IDeliveryPriceService _deliveryPriceService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ISalesCenterModelFactory _salesCenterModelFactory;
        private readonly IDeliveryTypeModelFactory _deliveryTypeModelFactory;
        private readonly ISalesCentersService _salesCenterService;
        private readonly INotificationService _notificationService;
        private readonly IDeliveryTypeService _deliveryTypeService;
        private readonly IAutoPostOfficeService _autoPostOfficeService;
        private readonly IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;

        #endregion

        #region Ctor

        public DeliveryAdminController(IDeliveryPriceService deliveryPriceService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ISalesCenterModelFactory salesCenterModelFactory,
            IDeliveryTypeModelFactory deliveryTypeModelFactory,
            ISalesCentersService salesCenterService,
            IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository,
            INotificationService notificationService,
            IDeliveryTypeService deliveryTypeService,
            IAutoPostOfficeService autoPostOfficeService)
        {
            _deliveryPriceService = deliveryPriceService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _salesCenterModelFactory = salesCenterModelFactory;
            _deliveryTypeModelFactory = deliveryTypeModelFactory;
            _salesCenterService = salesCenterService;
            _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository = deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;
            _notificationService = notificationService;
            _deliveryTypeService = deliveryTypeService;
            _autoPostOfficeService = autoPostOfficeService;
        }
        #endregion

        #region Methods

        #region SalesCenterRegion

        public virtual async Task<IActionResult> SalesCenterCreate(int languageId)
        {
            var model = await _salesCenterModelFactory.PrepareSalesCenterDtoModelAsync(new SalesCenterDtoModel(), null);
            model.LanguageId = languageId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SalesCentersList(SalesCenterSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

           var model = await _salesCenterModelFactory.PrepareSalesCentersListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> SalesCenterUpdate(SalesCenterDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            await _deliveryPriceService.UpdateSalesCenterAsync(model, languageId);
            
            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> SalesCenterDelete(SalesCenterDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _deliveryPriceService.DeleteSalesCenterAsync(model, languageId);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> SalesCenterCreate(SalesCenterDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _deliveryPriceService.CreateSalesCenterAsync(model, languageId);

            return RedirectToAction("Configure", "Config", new { area = AreaNames.Admin });
        }

        #endregion

        #region DeliveryTypeRegion

        [HttpPost]
        public async Task<IActionResult> GetDeliveryTypes(DeliveryTypeDtoSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            var model = await _deliveryTypeModelFactory.PrepareDeliveryTypesListModelAsync(searchModel);
           
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryTypeUpdate(DeliveryTypeDto model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            await _deliveryPriceService.DeliveryTypeUpdateAsync(model, languageId);

            return new NullJsonResult();
        }

        #endregion

        #region DeliveryOperatorRegion

        public virtual async Task<IActionResult> DeliveryOperatorCreate(int languageId)
        {
            var model = await _deliveryTypeModelFactory.PrepareDeliveryOperatorDtoModelAsync(new DeliveryOperatorDtoModel(), null);
            model.LanguageId = languageId;
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> GetDeliveryOperatorsAsync(DeliveryOperatorDtoSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            var model = await _deliveryTypeModelFactory.PrepareDeliveryOperatorListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryOperatorUpdateAsync(DeliveryOperatorDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            await _deliveryPriceService.UpdateDeliveryOperatorAsync(model, languageId);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryOperatorDelete(DeliveryOperatorDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            await _deliveryPriceService.DeleteDeliveryOperatorAsync(model, languageId);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryOperatorCreate(DeliveryOperatorDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            await _deliveryPriceService.CreateDeliveryOperatorAsync(model, languageId);

            return RedirectToAction("Configure", "Config", new { area = AreaNames.Admin });

        }

        #endregion

        #region DeliveryCityRegion

        public async Task<IActionResult> DeliveryCityCreate(int languageId)
        {
            var model = await _deliveryTypeModelFactory.PrepareDeliveryOperatorsCityModelAsync(new DeliveryOperatorsCityModel(), null);
            model.LanguageId = languageId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetDeliveryCities(DeliveryOperatorsCitySearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));
            
            var model = await _deliveryTypeModelFactory.PrepareDeliveryCityRegionListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryCityUpdate(DeliveryOperatorsCityModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            await _deliveryPriceService.UpdateDeliveryCityAsync(model, languageId);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryCityDelete(DeliveryOperatorsCityModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            await _deliveryPriceService.DeleteDeliveryCityAsync(model, languageId);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> DeliveryCityCreate(DeliveryOperatorsCityModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            await _deliveryPriceService.CreateDeliveryCityAsync(model);

            return RedirectToAction("Configure", "Config", new { area = AreaNames.Admin });
        }

        #endregion

        #region PriceRegion

        public virtual async Task<IActionResult> PriceCreate(int languageId)
        {
            var model = await _deliveryTypeModelFactory.PreparePriceDtoModelAsync(new PriceDtoAddModel(), languageId);
           
            model.LanguageId = languageId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetPricesAsync(PriceDtoSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            var model = await _deliveryTypeModelFactory.PreparePriceListModelAsync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> PriceUpdate(int Id, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var priceDto =  await _deliveryPriceService.GetPriceDtoByIdDropdownAsync(Id, languageId);
            var model = await _deliveryTypeModelFactory.PreparePriceEditDtoModelAsync(priceDto, languageId);
            model.LanguageId = languageId;
            return View(model);
            
        }
        [HttpPost]
        public async Task<IActionResult> PriceUpdate(PriceDtoAddModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _deliveryPriceService.UpdatePriceAsync(model);
            ViewBag.RefreshPage = true;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PriceDelete(PriceDtoAddModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _deliveryPriceService.DeletePriceAsync(model);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> PriceCreate(PriceDtoAddModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _deliveryPriceService.CreatePriceAsync(model);

            return RedirectToAction("Configure", "Config", new { area = AreaNames.Admin });
        }

        #endregion

        #region AutoPostOfficeRegion

        public virtual async Task<IActionResult> AutoPostOfficeCreate(int languageId)
        {
            var model = await _deliveryTypeModelFactory.PrepareAutoPostOfficeDtoModelAsync(new AutoPostOfficeDtoModel(), languageId);
            model.LanguageId = languageId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetAutoPostOffices(AutoPostOfficeDtoSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return ErrorJson(await _localizationService.GetResourceAsync("Admin.AccessDenied.Description"));

            var model = await _deliveryTypeModelFactory.PrepareAutoPostOfficeListModelAsync(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> AutoPostOfficeUpdate(int Id, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var autopostOfficeDto = await _deliveryPriceService.GetAutoPostOfficeDtoByIdAsync(Id, languageId);

            var model = await _deliveryTypeModelFactory.PrepareAutoPostOfficeEditDtoModelAsync(autopostOfficeDto,languageId);
            model.LanguageId = languageId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AutoPostOfficeUpdate([Validate] AutoPostOfficeDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");
            await _deliveryPriceService.UpdateAutoPostOfficeAsync(model, languageId);
            ViewBag.RefreshPage = true;
            return View(model);
        }

        public async Task<IActionResult> AutoPostOfficeDelete(AutoPostOfficeDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            await _deliveryPriceService.DeleteAutoPostOfficeAsync(model, languageId);

            return new NullJsonResult();
        }

        [HttpPost]
        public async Task<IActionResult> AutoPostOfficeCreate(AutoPostOfficeDtoModel model, int languageId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTaxSettings))
                return Content("Access denied");

            var deliveryOperatorDeliveryTypeDeliveryCityDependency = _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table.FirstOrDefault(dodtdc =>
               dodtdc.DeliveryOperatorId == model.DeliveryOperatorId && dodtdc.DeliveryTypeId == model.DeliveryTypeId && dodtdc.DeliveryCityId == model.DeliveryCityId
           );
            var models = await _deliveryTypeModelFactory.PrepareAutoPostOfficeDtoModelAsync(model, languageId);
            if (deliveryOperatorDeliveryTypeDeliveryCityDependency == null)
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Shipping.Delivery.Fields.Price.Error"));
                return View(models);
            }

            if ((await _deliveryTypeService.GetDeliveryTypeByIdAsync(deliveryOperatorDeliveryTypeDeliveryCityDependency.DeliveryTypeId)).SystemType != "PickPoint")
            {
                _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Shipping.Delivery.Fields.DeliveryType.Error"));
                return View(models);
            }

            var autoPostOffice = new AutoPostOffice
            {
                DeliveryOperator_DeliveryType_DeliveryCity_DependencyId = deliveryOperatorDeliveryTypeDeliveryCityDependency.Id,
                Blocked = model.Blocked,
                PointId = model.PointId
            };
            await _autoPostOfficeService.InsertAutoPostOfficeAsync(autoPostOffice);
            var autoPostOfficeLanguage = new AutoPostOfficeLanguage
            {
                Address = model.Address,
                Comment = model.Comment,
                LanguageId = languageId,
                AutoPostOfficeId = autoPostOffice.Id
            };
            await _autoPostOfficeService.InsertAutoPostOfficeLanguageAsync(autoPostOfficeLanguage);

            return RedirectToAction("Configure", "Config", new { area = AreaNames.Admin });
        }

        #endregion

        #endregion

    }
}
