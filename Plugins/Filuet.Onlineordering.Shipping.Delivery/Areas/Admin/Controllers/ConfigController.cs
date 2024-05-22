using Filuet.Onlineordering.Shipping.Delivery.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Controllers
{
    public class ConfigController :BaseAdminController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILanguageService _languageService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ConfigController(
            ISettingService settingService,
            IStoreContext storeContext,
            ILanguageService languageService,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _languageService = languageService;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [AspMvcSuppressViewError]
        public async Task<IActionResult> Configure()
        {
            var deliveryPluginSettings = _settingService.LoadSetting<DeliveryPluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            var configurationModel = new ConfigurationModel
            {
                PhoneMask = deliveryPluginSettings.PhoneMask,
                PhonePrefix = deliveryPluginSettings.PhonePrefix,
                Languages = _languageService.GetAllLanguages(),
                SelfPickupActive = deliveryPluginSettings.SelfPickupActive,
                AddAddressToComment = deliveryPluginSettings.AddAddressToComment,
                RequirePostCode = deliveryPluginSettings.RequirePostCode,
                ShowInvitation = deliveryPluginSettings.ShowInvitation,
                SalesCenterElectronicQueueInvitation = deliveryPluginSettings.SalesCenterElectronicQueueInvitation,
                NotificationHtmlAboveModule = deliveryPluginSettings.NotificationHtmlAboveModule,
                MinCriterion = deliveryPluginSettings.MinCriterion,
                PickPoint = deliveryPluginSettings.PickPoint,
                HomeDelivery = deliveryPluginSettings.HomeDelivery
            };

            configurationModel.SalesCenterSearchModel.SetGridPageSize();
            configurationModel.DeliveryTypeDtoSearchModel.SetGridPageSize();
            configurationModel.DeliveryOperatorDtoSearchModel.SetGridPageSize();
            configurationModel.DeliveryOperatorsCitySearchModel.SetGridPageSize();
            configurationModel.PriceDtoSearchModel.SetGridPageSize();
            configurationModel.AutoPostOfficeDtoSearchModel.SetGridPageSize();

            return View(configurationModel);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return await Configure();
            }

            //save settings
            var deliveryPluginSettings = new DeliveryPluginSettings
            {
                PhonePrefix = model.PhonePrefix,
                PhoneMask = model.PhoneMask,
                SelfPickupActive = model.SelfPickupActive,
                AddAddressToComment = model.AddAddressToComment,
                RequirePostCode = model.RequirePostCode,
                ShowInvitation = model.ShowInvitation,
                SalesCenterElectronicQueueInvitation = model.SalesCenterElectronicQueueInvitation,
                NotificationHtmlAboveModule = model.NotificationHtmlAboveModule,
                MinCriterion = model.MinCriterion,
                PickPoint = model.PickPoint,
                HomeDelivery = model.HomeDelivery
            };
            await _settingService.SaveSettingAsync(deliveryPluginSettings, await _storeContext.GetActiveStoreScopeConfigurationAsync());

            //now clear settings cache
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }
        #endregion

    }
}
