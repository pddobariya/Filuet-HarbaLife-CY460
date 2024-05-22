using ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace ISDK.Filuet.Theme.FiluetHerbalife.Areas.Admin.Controllers
{
    public class FiluetHerbalifeController : BasePluginController
    {
        #region Fields

        private readonly ThemeFiluetHerbalifeSettings _themeFiluetHerbalifeSettings;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public FiluetHerbalifeController(
            ThemeFiluetHerbalifeSettings themeFiluetHerbalifeSettings,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ISettingService settingService)
        {
            _themeFiluetHerbalifeSettings = themeFiluetHerbalifeSettings;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel
            {
                CategoryIdForCatalogue = _themeFiluetHerbalifeSettings.CategoryIdForCatalogue,
                CategoryIdForProgramm = _themeFiluetHerbalifeSettings.CategoryIdForProgramm,
                OmnivaCarrierUrl = _themeFiluetHerbalifeSettings.OmnivaCarrierUrl,
                DPDLatviaCarrierUrl = _themeFiluetHerbalifeSettings.DPDLatviaCarrierUrl,
                DPDLithuaniaCarrierUrl = _themeFiluetHerbalifeSettings.DPDLithuaniaCarrierUrl,
                DPDEstoniaCarrierUrl = _themeFiluetHerbalifeSettings.DPDEstoniaCarrierUrl
            };
            return View(model);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            _themeFiluetHerbalifeSettings.CategoryIdForCatalogue = model.CategoryIdForCatalogue;
            _themeFiluetHerbalifeSettings.CategoryIdForProgramm = model.CategoryIdForProgramm;
            _themeFiluetHerbalifeSettings.OmnivaCarrierUrl = model.OmnivaCarrierUrl;
            _themeFiluetHerbalifeSettings.DPDLatviaCarrierUrl = model.DPDLatviaCarrierUrl;
            _themeFiluetHerbalifeSettings.DPDLithuaniaCarrierUrl = model.DPDLithuaniaCarrierUrl;
            _themeFiluetHerbalifeSettings.DPDEstoniaCarrierUrl = model.DPDEstoniaCarrierUrl;
            await _settingService.SaveSettingAsync(_themeFiluetHerbalifeSettings);
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return Configure();
        }

        #endregion
    }
}
