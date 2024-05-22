using ISDK.Filuet.OnlineOrdering.CorePlugin.Components;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Plugin_Data;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Setting;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Tasks;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Task = System.Threading.Tasks.Task;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin
{
    /// <summary>
    /// ShoppingPlugin
    /// </summary>
    public class FiluetCorePlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IPluginSettingsService _pluginSettingsService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly INopFileProvider _fileProvider;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
        private readonly IWorkContext _workContext;
        private readonly ILanguageService _languageService;
        private readonly INopFileProvider _nopFileProvider;
        private readonly IGenericAttributeService _genericAttributeService;

        /// <summary>
        /// 12 hours
        /// </summary>
        private const int SecondsAtHours12 = 12 * 3600;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="scheduleTaskService">NOP Scheduler</param>
        public FiluetCorePlugin(
            IScheduleTaskService scheduleTaskService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IPluginSettingsService pluginSettingsService,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            INopFileProvider fileProvider,
            IPictureService pictureService, IWebHelper webHelper,
            IRepository<CustomerRole> customerRoleRepository,
            IWorkContext workContext,
            ILanguageService languageService,
            INopFileProvider nopFileProvider, IGenericAttributeService genericAttributeService)
        {
            _scheduleTaskService = scheduleTaskService;
            _localizationService = localizationService;
            _settingService = settingService;
            _pluginSettingsService = pluginSettingsService;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _fileProvider = fileProvider;
            _pictureService = pictureService;
            _webHelper = webHelper;
            _customerRoleRepository = customerRoleRepository;
            _workContext = workContext;
            _languageService = languageService;
            _nopFileProvider = nopFileProvider;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Utilities

        private async Task InstallLocalizationAsync()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _nopFileProvider.MapPath("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _nopFileProvider.EnumerateFiles(directoryPath, string.Format("language_{0}_pack.xml", language.UniqueSeoCode)))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }

            // TODO remove AddOrUpdateLocaleResourceAsync. Create folder with resources
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.VolumePoints", "{0} VP");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.SKU", "SKU: {0}");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.BasePrice", "Base for calculating prices:");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.BaseRenumeration", "Base for calculating renumerations:");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.RetailPrice", "Recommended retail price:");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Catalog.Categories.Fields.IsMeta", "Is Meta");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Catalog.Categories.Fields.CategoryType", "Category Type");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Admin.Customers.Customers.Fields.DistributorId", "Distributor Id");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.Aside", "Nutrition for a better life!");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.LoginMember", "Distributor & Members");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.OrderProducts", "Order products and more");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.Forgot", "I do not have or I have lost my PIN");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.OAuth", "Authorize as MyHerbalife member");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Account.Login.WrongCredentials.UnableGetAccessToken", "Unable to retrieve AccessToken");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Account.Login.WrongCredentials.InvalidIdOrPin", "Invalid ID or PIN");
            await _localizationService.AddOrUpdateLocaleResourceAsync("Account.Login.WrongCredentials.CallSupport", "Please call Support + 00 000 0000 to recover your PIN");
            await _localizationService.AddOrUpdateLocaleResourceAsync("ShoppingCart.ContainsDifferentCategory", "The shopping cart contains a product from a different category.");

            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.OverriddenVolumePoints", "Volume Points");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.OverriddenBasePrice", "Base for calculating prices");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.OverriddenBaseRenumeration", "Base for calculating renumerations");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Catalog.Products.ProductAttributes.AttributeCombinations.Fields.OverriddenRetailPrice", "Recommended retail price");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.VolumePoints", "{0} VP");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.SKU", "SKU: {0}");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.BasePrice", "Base for calculating prices:");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.BaseRenumeration", "Base for calculating renumerations:");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.RetailPrice", "Recommended retail price:");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.System.ProxyHealth", "The proxy heartbeat");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Common.Test", "Test");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Catalog.Categories.Fields.IsMeta", "Is Meta");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Catalog.Categories.Fields.CategoryType", "Category Type");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Admin.Customers.Customers.Fields.DistributorId", "Distributor Id");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.Aside", "Nutrition for a better life!");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.LoginMember", "Distributors & Members");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.OrderProducts", "Order products and more");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.Forgot", "I do not have or I have lost my PIN");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.OAuth", "Authorize as MyHerbalife member");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Login.WrongCredentials.UnableGetAccessToken", "Unable to retrieve AccessToken");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Login.WrongCredentials.InvalidIdOrPin", "Invalid ID or PIN");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Login.WrongCredentials.CallSupport", "Please call Support + 00 000 0000 to recover your PIN");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Login.WrongCredentials.GoldStandard",
                $"To proceed with order placement please visit &lt;a href='https://myHerbalife.com'&gt;myHerbalife.com&lt;/a&gt; to acknowledge our Gold Standard Guarantees");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Login.WrongCredentials.CantBuy", "Unable to proceed with order placement, please contact your local Member Services for further information.");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Login.WrongCredentials.ForeignPurchaseRestriction", "As a Privileged Member, you are not allowed to purchase products outside the Country you are registered in.");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ShoppingCart.ContainsDifferentCategory", "The shopping cart contains a product from a different category.");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Address.Fields.Address1", "Address");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ShoppingCart.ApfOnlyAllowed", "Only APF is allowed.");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Customer.ApfProductIsRequired", "Only APF is allowed.");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Fields.Ppv", "PPV");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Fields.Pv", "PV");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Fields.Tv", "TV");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.Fields.DistributorId", "Member Id");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.CustomerInfo.CallToChangePersonalDetails", "Please call Support + 00 000 0000 to change your personal details");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "Account.ChangePassword.Fields.NewPassword.IsWeak", "The new password shall be a combination of alpha, numeric, and special characters.");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                    "ISDK.Filuet.OnlineOrdering.CorePlugin.StartDate", "DualMonth StartDate");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                    "ISDK.Filuet.OnlineOrdering.CorePlugin.EndDate", "DualMonth EndDate");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.CountryCode", "CountryCode");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.ProcessingLocationCode", "ProcessingLocationCode");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.CurrencyCode", "CurrencyCode");
            await _localizationService.AddOrUpdateLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.HerbalifeEnvironment", "Herbalife Environment");

            await _localizationService.AddOrUpdateLocaleResourceAsync("HBL.Baltic.APFDueDateWarningPeriodDays.MessageTemplate", "Your APF is due on {0:dd MMMM yyyy}. You can pay it now.");
            await _localizationService.AddOrUpdateLocaleResourceAsync("HBL.Baltic.APFDueDateWarningPeriodDays.MessageTemplate", "Срок оплаты Вашего ежегодного взноса истекает {0:dd MMMM yyyy}. Оплатить сейчас.", "ru-RU");
        }

        private async Task UninstallLocalizationAsync()
        {
            //For EN
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _nopFileProvider.MapPath("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _nopFileProvider.EnumerateFiles(directoryPath, string.Format("language_{0}_pack.xml", language.UniqueSeoCode)))
                {
                    var languageResourceNames = from name in XDocument.Load(filePath).Document.Descendants("LocaleResource")
                                                select name.Attribute("Name").Value;

                    foreach (var item in languageResourceNames)
                    {
                        await _localizationService.DeleteLocaleResourcesAsync(item);
                    }
                }
            }


            // TODO remove DeleteLocaleResourceAsync. Create folder with resources
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.VolumePoints");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.SKU");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.BasePrice");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.BaseRenumeration");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.RetailPrice");
            await _localizationService.DeleteLocaleResourceAsync("Admin.Catalog.Categories.Fields.IsMeta");
            await _localizationService.DeleteLocaleResourceAsync("Admin.Catalog.Categories.Fields.CategoryType");
            await _localizationService.DeleteLocaleResourceAsync("Admin.Customers.Customers.Fields.DistributorId");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.Aside");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.LoginMember");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.OrderProducts");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.Forgot");
            await _localizationService.DeleteLocaleResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.UserLogin.OAuth");
            await _localizationService.DeleteLocaleResourceAsync("Account.Login.WrongCredentials.UnableGetAccessToken");
            await _localizationService.DeleteLocaleResourceAsync("Account.Login.WrongCredentials.InvalidIdOrPin");
            await _localizationService.DeleteLocaleResourceAsync("Account.Login.WrongCredentials.CallSupport");
            await _localizationService.DeleteLocaleResourceAsync("ShoppingCart.ContainsDifferentCategory");
            await _localizationService.DeleteLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.StartDate");
            await _localizationService.DeleteLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.EndDate");
            await _localizationService.DeleteLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.CountryCode");
            await _localizationService.DeleteLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.ProcessingLocationCode");
            await _localizationService.DeleteLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.CurrencyCode");
            await _localizationService.DeleteLocaleResourceAsync(
                "ISDK.Filuet.OnlineOrdering.CorePlugin.HerbalifeEnvironment");
        }

        private async Task InstallSettingsAsync()
        {
            await _settingService.SaveSettingAsync(new FiluetCorePluginSettings()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                ProductShowReview  = false
            });
            
            var defaultSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(DefaultSettingsData.ChangedSettings);
            _pluginSettingsService.Install(defaultSettings);

            // ISDK.Filuet.ExternalSSOAuthPlugin mark as active            
            _externalAuthenticationSettings.ActiveAuthenticationMethodSystemNames.Add(ISDKFiluetPluginNames.ExternalSSOAuthSystemName);
            await _settingService.SaveSettingAsync(_externalAuthenticationSettings);
        }

        private async Task UninstallSettingsAsync()
        {
            await _settingService.DeleteSettingAsync<FiluetCorePluginSettings>();
        }

        private async void InstallSampleData()
        {
            var logoImagesPath = _fileProvider.MapPath($"~/Plugins/{ISDKFiluetPluginNames.FiluetCorePluginSystemName}/Plugin_Data/Images/");
            var picture1Id = _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(logoImagesPath + "banner_herbalife_1.jpeg"),
                MimeTypes.ImagePJpeg, "banner_herbalife_1").Id;
            var picture2Id = _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(logoImagesPath + "banner_herbalife_2.jpeg"),
                MimeTypes.ImagePJpeg, "banner_herbalife_2").Id;
            var picture3Id = _pictureService.InsertPictureAsync(await _fileProvider.ReadAllBytesAsync(logoImagesPath + "banner_herbalife_3.jpeg"),
                MimeTypes.ImagePJpeg, "banner_herbalife_3").Id;

            //_dbContext.ExecuteSqlCommand($@"
            //    SET IDENTITY_INSERT [dbo].[SS_AS_AnywhereSlider] ON
            //    INSERT INTO [dbo].[SS_AS_AnywhereSlider]([Id], [SystemName], [SliderType], [LanguageId], [LimitedToStores])                                            
            //    VALUES(1, 'ENG', 10, 1, 0)
            //    SET IDENTITY_INSERT [dbo].[SS_AS_AnywhereSlider] OFF

            //    INSERT INTO dbo.SS_AS_SliderImage(DisplayText, Url, Alt, Visible, DisplayOrder, PictureId, SliderId)
            //                                VALUES (NULL, NULL, NULL, 1, 1, {picture1Id}, 1)

            //    INSERT INTO dbo.SS_AS_SliderImage(DisplayText, Url, Alt, Visible, DisplayOrder, PictureId, SliderId)
            //                                VALUES (NULL, NULL, NULL, 1, 1, {picture2Id}, 1)

            //    INSERT INTO dbo.SS_AS_SliderImage(DisplayText, Url, Alt, Visible, DisplayOrder, PictureId, SliderId)
            //                                VALUES (NULL, NULL, NULL, 1, 1, {picture3Id}, 1)
            //");

            //DatabaseHelper.ExecuteSqlFile(_fileProvider.MapPath("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Plugin_Data/Sql/create_sample_data.sql"));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Install
        /// </summary>
        public override async Task InstallAsync()
        {
            if (!await PluginHelper.EnsurePluginInstalledAsync(ISDKFiluetPluginNames.PickupInStorePluginSystemName))
            {
                await Task.CompletedTask;
            }

            if (!await PluginHelper.EnsurePluginInstalledAsync(ISDKFiluetPluginNames.ExternalSSOAuthSystemName))
            {
                await Task.CompletedTask;
            }

            // schedule task
            ScheduleTask task = await _scheduleTaskService.GetTaskByTypeAsync(FusionSubmitFailedOrdersScheduleTask.TaskType);
            if (task == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                {
                    Name = FusionSubmitFailedOrdersScheduleTask.Name,
                    Seconds = SecondsAtHours12,
                    Type = FusionSubmitFailedOrdersScheduleTask.TaskType,
                    Enabled = true,
                    StopOnError = false
                });
            }

            task = await _scheduleTaskService.GetTaskByTypeAsync(OrderInfoDeliveryEmailScheduleTask.TaskType);
            if (task == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                {
                    Name = OrderInfoDeliveryEmailScheduleTask.Name,
                    Seconds = 4 * 3600,
                    Type = OrderInfoDeliveryEmailScheduleTask.TaskType,
                    Enabled = true,
                    StopOnError = false
                });
            }

            task = await _scheduleTaskService.GetTaskByTypeAsync(Get1SStockBalanceScheduleTask.TaskType);
            if (task == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                {
                    Name = Get1SStockBalanceScheduleTask.Name,
                    Seconds = 60,
                    Type = Get1SStockBalanceScheduleTask.TaskType,
                    Enabled = true,
                    StopOnError = false
                });
            }

            task = await _scheduleTaskService.GetTaskByTypeAsync(UpdateStockBalanceScheduleTask.TaskType);
            if (task == null)
            {
                await _scheduleTaskService.InsertTaskAsync(new ScheduleTask()
                {
                    Name = UpdateStockBalanceScheduleTask.Name,
                    Seconds = 1200,
                    Type = UpdateStockBalanceScheduleTask.TaskType,
                    Enabled = true,
                    StopOnError = false
                });
            }

            await InstallLocalizationAsync();
            await InstallSettingsAsync();


            await _customerRoleRepository.InsertAsync(new CustomerRole
            {
                Name = CommonConstants.IsNotResidentCustomerRole,
                Active = true,
                IsSystemRole = true,
                SystemName = CommonConstants.IsNotResidentCustomerRole
            });

            await _customerRoleRepository.InsertAsync(new CustomerRole
            {
                Name = CommonConstants.IsResidentCustomerRole,
                Active = true,
                IsSystemRole = true,
                SystemName = CommonConstants.IsResidentCustomerRole
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall
        /// </summary>
        public override async Task UninstallAsync()
        {
            ////schedule task
            ScheduleTask task = await _scheduleTaskService.GetTaskByTypeAsync(FusionSubmitFailedOrdersScheduleTask.TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }

            task = await _scheduleTaskService.GetTaskByTypeAsync(Get1SStockBalanceScheduleTask.TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }

            task = await _scheduleTaskService.GetTaskByTypeAsync(UpdateStockBalanceScheduleTask.TaskType);
            if (task != null)
            {
                await _scheduleTaskService.DeleteTaskAsync(task);
            }

            await UninstallSettingsAsync();
            await UninstallLocalizationAsync();

            await base.UninstallAsync();
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FiluetCorePlugin/Configure";
        }



        public bool HideInWidgetList { get; }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.Run(async () =>
            {
                var filuetCorePluginSettings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>();
                IList<string> widgetZones = new List<string>()
                {
                    //PublicWidgetZones.HomePageTop,
                    PublicWidgetZones.BodyEndHtmlTagBefore
                };

                if (filuetCorePluginSettings.IsCheckPartnerContract)
                    widgetZones.Add(PublicWidgetZones.OpCheckoutPaymentInfoBottom);

                //oldcode to new
                //if (string.IsNullOrEmpty(await (await _workContext.GetCurrentCustomerAsync()).GetAttributeAsync<string>(CoreGenericAttributes.CustomerInnAttribute)) &&
                //    filuetCorePluginSettings.NumberOfDigitsInn > 0)
                //{
                //    widgetZones.Add(PublicWidgetZones.OpCheckoutPaymentInfoTop);
                //}
                
                if (string.IsNullOrEmpty(await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.CustomerInnAttribute)) &&
                    filuetCorePluginSettings.NumberOfDigitsInn > 0)
                {
                    widgetZones.Add(PublicWidgetZones.OpCheckoutPaymentInfoTop);
                }
                return widgetZones;
            });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.HomepageTop)
            {
                return typeof(APFDateExpiredViewComponent);
            }
            else if (widgetZone == PublicWidgetZones.OpCheckoutPaymentInfoTop)
            {
                return typeof(InnPaymentInfoViewComponent);
            }
            else if (widgetZone == PublicWidgetZones.OpCheckoutPaymentInfoBottom)
            {
                return typeof(CheckPartnerViewComponent);
            }
            else if (widgetZone == PublicWidgetZones.BodyEndHtmlTagBefore)
            {
                return typeof(RootViewComponent);
            }
            return null;
        }

        #endregion
    }
}