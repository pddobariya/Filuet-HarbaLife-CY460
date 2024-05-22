using Filuet.Onlineordering.Shipping.Delivery.Components;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

[assembly: JetBrains.Annotations.AspMvcViewLocationFormat("~/../Areas/Admin/Views/Shared/{0}.cshtml")]


namespace Filuet.Onlineordering.Shipping.Delivery
{
    public class DeliveryPlugin : BasePlugin, IWidgetPlugin ,IFusionShippingProvider
    {
        #region Const

        public const string DeliveryComponentName = "Delivery";
        public const string DeliveryHeaderZoneComponentName = "DeliveryHeaderZone";
        public const string SalesCenterElectronicQueueViewComponent = "SalesCenterElectronicQueue";

        #endregion

        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;
        private readonly INopFileProvider _fileProvider;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
      
        #endregion

        #region Ctor
        public DeliveryPlugin(IWebHelper webHelper,
            ILocalizationService localizationService,
            ILanguageService languageService,
            INopFileProvider fileProvider,
            IWorkContext workContext, IGenericAttributeService genericAttributeService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _languageService = languageService;
            _fileProvider = fileProvider;
            _workContext = workContext;
            _genericAttributeService = genericAttributeService;
         
        }

        #endregion


        #region Widget Method
        public bool HideInWidgetList => false;


        //public string FreightCode =>_workContext.GetCurrentCustomerAsync().Result.GetAttributeAsync<string>(CustomerAttributeNames.SelectedShippingFreightCode).Result;
        //  public string FreightCode => _genericAttributeService.GetAttributeAsync<string>(_workContext.GetCurrentCustomerAsync().Result,CustomerAttributeNames.SelectedShippingFreightCode).Result;

        public Task<string> FreightCode => GetFreightCodeAsync();

        private async Task<string> GetFreightCodeAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var freightCode = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.SelectedShippingFreightCode);

            return freightCode;
        }



        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.HeadHtmlTag)
            {
                return typeof(DeliveryHeaderZoneViewComponent);
            }
            else if (widgetZone == FiluetPublicWidgetZones.BeforeOrderSummaryCartFooter)
            {
                return typeof(SalesCenterElectronicQueueViewComponent);
            }
            return typeof(DeliveryViewComponent);
        }
        public  Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.OpCheckoutShippingMethodTop,
                PublicWidgetZones.HeadHtmlTag,
                FiluetPublicWidgetZones.BeforeOrderSummaryCartFooter
            });
        }
        #endregion

        #region PageConfigure Register Method
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/Config/Configure";
        }
        #endregion

        #region Resourcestring
        protected virtual async Task InstallLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, string.Format("ResourceString_{0}.xml", language.UniqueSeoCode)))
                {
                    using var reader = new StreamReader(filePath);
                    await _localizationService.ImportResourcesFromXmlAsync(language, reader);
                }
            }

        }
        protected virtual async Task DeleteLocaleResources()
        {
            var allLanguages = await _languageService.GetAllLanguagesAsync(showHidden: true);
            var directoryPath = _fileProvider.MapPath("~/Plugins/Filuet.Onlineordering.Shipping.Delivery/Localization/ResourceString/");
            foreach (var language in allLanguages)
            {
                foreach (var filePath in _fileProvider.EnumerateFiles(directoryPath, string.Format("ResourceString_{0}.xml", language.UniqueSeoCode)))
                {
                    var languageResourceNames = from name in XDocument.Load(filePath).Document.Descendants("LocaleResource")
                                                select name.Attribute("Name").Value;

                    foreach (var item in languageResourceNames)
                    {
                        await _localizationService.DeleteLocaleResourcesAsync(item);
                    }
                }
            }

        }
        #endregion

        #region Install & UnInstall Method
        public override async Task InstallAsync()
        {
            await InstallLocaleResources();
            await base.InstallAsync();
        }
        public override async Task UninstallAsync()
        {
            await DeleteLocaleResources();
            await base.UninstallAsync();
        }
        #endregion
    }
}