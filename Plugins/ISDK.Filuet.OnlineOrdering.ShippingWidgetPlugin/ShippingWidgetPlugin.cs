using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Components;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin
{
    public class ShippingWidgetPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;        
        private readonly WidgetSettings _widgetSettings;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly INopFileProvider _nopFileProvider;
        private readonly IRepository<Language> _languageRepository;


        #endregion

        #region Ctor

        public ShippingWidgetPlugin(
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            WidgetSettings widgetSettings,
            ICheckoutAttributeService checkoutAttributeService,
            INopFileProvider nopFileProvider,
            IRepository<Language> languageRepository)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _widgetSettings = widgetSettings;
            _checkoutAttributeService = checkoutAttributeService;
            _nopFileProvider = nopFileProvider;
            _languageRepository = languageRepository;
        }

        #endregion

        #region Methods

        #region WidgetZone

        /// <summary>
        /// Get Widget zone
        /// </summary>
        /// <returns></returns>
        /// 
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> 
            {  
                PublicWidgetZones.HeadHtmlTag,
                PublicWidgetZones.CheckoutShippingMethodTop,
                PublicWidgetZones.OrderSummaryContentBefore ,
                PublicWidgetZones.HeaderSelectors
            });
        }

        public bool HideInWidgetList => false;

        //Get component for front end side
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == PublicWidgetZones.CheckoutShippingMethodTop 
                || widgetZone == PublicWidgetZones.OpCheckoutShippingMethodTop)
            {
                return typeof(CheckoutShippingMethodTopWidgetViewComponent);
            }

            if (widgetZone == PublicWidgetZones.OrderSummaryContentBefore)
            {
                return typeof(OrderSummaryContentBeforeWidgetViewComponent);                
            }
            if (widgetZone == PublicWidgetZones.HeaderSelectors)
            {
                return typeof(ShippingCountrySelectorViewComponent);
            }

            return typeof(WidgetHeadViewComponent);
        }

        #endregion

        #region GetConfigurationPageUrl

        //Get Configuration path
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/ShippingWidgetPlugin/Configure";
        }

        #endregion

        #region InstallAsync

        public override async Task InstallAsync()
        
        {
            await InstallLocaleResources();

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(ISDKFiluetPluginNames.ShippingWidgetPluginSystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(ISDKFiluetPluginNames.ShippingWidgetPluginSystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            var giftWrappingCheckoutAttribute =(await _checkoutAttributeService.GetAllCheckoutAttributesAsync()).FirstOrDefault(p => p.Name == "Gift wrapping");
            if (giftWrappingCheckoutAttribute != null)
            {
                await _checkoutAttributeService.DeleteCheckoutAttributeAsync(giftWrappingCheckoutAttribute);
            }

            await base.InstallAsync();
        }

        #endregion

        #region UninstallAsync

        public override async Task UninstallAsync()
        {            
            await _settingService.DeleteSettingAsync<ShippingWidgetPluginSettings>();

            await DeleteLocalResourcesAsync();

            await base.UninstallAsync();
        }

        #endregion

        #region InstallLocaleResources

        /// <summary>
        ///Import Resource string from xml and save
        /// </summary>
        protected virtual async Task InstallLocaleResources()
        {
            var languages = _languageRepository.Table.ToList();
            foreach (var language in languages)
            {
                //save resources
                foreach (var filePath in System.IO.Directory.EnumerateFiles(_nopFileProvider.MapPath("~/Plugins/ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin/Localization"), "ResourceString.xml", SearchOption.TopDirectoryOnly))
                {
                    using (var streamReader = new StreamReader(filePath))
                    {
                        await _localizationService.ImportResourcesFromXmlAsync(language, streamReader);
                    }
                }
            }
        }

        #endregion

        #region DeleteLocalResourcesAsync

        ///<summry>
        ///Delete Resource String
        ///</summry>
        protected virtual async Task DeleteLocalResourcesAsync()
        {
            var file = Path.Combine(_nopFileProvider.MapPath("~/Plugins/ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin/Localization"), "ResourceString.xml");
            var languageResourceName = from name in XDocument.Load(file).Document?.Descendants("LocaleResource")
                                       select name.Attribute("Name")?.Value;
            await _localizationService.DeleteLocaleResourcesAsync(languageResourceName.ToList());
        }

        #endregion
       

        #endregion
    }
}