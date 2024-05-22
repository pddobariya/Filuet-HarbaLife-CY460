using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Gdpr;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Framework.Controllers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    [NameControllerModelConvention("Setting")]
    public class FiluetSettingController : SettingController
    {
        #region Fields

        public const string InstagramLinkAttribute = "InstagramLink";
        public const string InstagramLink_OverrideForStoreAttribute = "InstagramLink_OverrideForStore";
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public FiluetSettingController(
            AppSettings appSettings,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            INopDataProvider dataProvider,
            IEncryptionService encryptionService, 
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService, 
            IGdprService gdprService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager, 
            INopFileProvider fileProvider,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingModelFactory settingModelFactory, 
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext, 
            IUploadService uploadService) 
            : base(appSettings,
                  addressService, 
                  customerActivityService,
                  customerService,
                  dataProvider,
                  encryptionService,
                  eventPublisher,
                  genericAttributeService,
                  gdprService,
                  localizedEntityService, 
                  localizationService,
                  multiFactorAuthenticationPluginManager,
                  fileProvider,
                  notificationService,
                  orderService, 
                  permissionService, 
                  pictureService,
                  settingModelFactory, 
                  settingService,
                  storeContext,
                  storeService,
                  workContext, 
                  uploadService)
        {
            _genericAttributeService = genericAttributeService;
            _permissionService = permissionService;
            _settingModelFactory = settingModelFactory;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        [JetBrains.Annotations.AspMvcSuppressViewError]
        public override async Task<IActionResult> GeneralCommon(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareGeneralCommonSettingsModelAsync();

            var filuetGeneralCommonSettingsModel = AutoMapperConfiguration.Mapper.Map<FiluetGeneralCommonSettingsModel>(
                model);
            ((FiluetStoreInformationSettingsModel)filuetGeneralCommonSettingsModel.StoreInformationSettings).InstagramLink =
                await _genericAttributeService.GetAttributeAsync<string>(_storeContext.GetCurrentStore(), InstagramLinkAttribute);
            ((FiluetStoreInformationSettingsModel)filuetGeneralCommonSettingsModel.StoreInformationSettings).InstagramLink_OverrideForStore =
                await _genericAttributeService.GetAttributeAsync<bool>(_storeContext.GetCurrentStore(), InstagramLink_OverrideForStoreAttribute);
            

            return View(filuetGeneralCommonSettingsModel);
        }

        [OrderActionConstraint(1)]
        public override async Task<IActionResult> GeneralCommon(GeneralCommonSettingsModel model)
        {
            return await base.GeneralCommon(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        [OrderActionConstraint(2)]
        public async Task<IActionResult> GeneralCommon(FiluetGeneralCommonSettingsModel model)
        {
            await _genericAttributeService.SaveAttributeAsync(_storeContext.GetCurrentStore(), InstagramLinkAttribute,
                ((FiluetStoreInformationSettingsModel)model.StoreInformationSettings).InstagramLink);
            await _genericAttributeService.SaveAttributeAsync(_storeContext.GetCurrentStore(), InstagramLink_OverrideForStoreAttribute,
                ((FiluetStoreInformationSettingsModel)model.StoreInformationSettings).InstagramLink_OverrideForStore);
            var baseModel = model;
            baseModel.StoreInformationSettings = model.StoreInformationSettings;
            return await base.GeneralCommon(baseModel);
        }

        #endregion
    }
}
