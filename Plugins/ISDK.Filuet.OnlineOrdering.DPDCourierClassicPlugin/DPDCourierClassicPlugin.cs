using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.DTO;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace ISDK.Filuet.OnlineOrdering.DPDCourierClassicPlugin
{
    public class DPDCourierClassicPlugin : BasePlugin, IShippingRateComputationMethod, IShippingInformationProvider, IFusionShippingProvider
    {
        #region Fields

        private readonly ShippingSettings _shippingSettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public DPDCourierClassicPlugin(
            ShippingSettings shippingSettings,
            ISettingService settingService)
        {
            _shippingSettings = shippingSettings;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public IShipmentTracker ShipmentTracker => null!;

        //  public string FreightCode => FreightCodes.BLC;

        public Task<string> FreightCode => GetFreightCodeAsync();

        private async Task<string> GetFreightCodeAsync()
        {
            return await Task.FromResult(FreightCodes.BLC);
        }

        public IEnumerable<FormFieldMeta> GetAdditionalShippingFields()
        {
            var result = new List<FormFieldMeta>
            {
                new FormFieldMeta {
                    NameResourceKey = CustomerAttributeNames.ShippingNotes,
                    ControlType = AttributeControlType.MultilineTextbox,
                    DisplayOrder = 1
                }
            };

            return result.OrderBy(p => p.DisplayOrder);
        }

        //public string GetConfigurationPageUrl()
        //{
        //    return "";
        //}

        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            return Task.FromResult<decimal?>(0);
        }

        public IEnumerable<string> GetHiddenShippingFields()
        {
            return new List<string>() {
                ShippingFieldNames.CityDropdownList,
                ShippingFieldNames.AddressDropdownList
            };
        }

        public Task<IShipmentTracker> GetShipmentTrackerAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            var response = new GetShippingOptionResponse();
            return await Task.FromResult(response);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            if (!_shippingSettings.ActiveShippingRateComputationMethodSystemNames.Contains(ISDKFiluetPluginNames.DPDCourierClassicPluginSystemName))
            {
                _shippingSettings.ActiveShippingRateComputationMethodSystemNames.Add(ISDKFiluetPluginNames.DPDCourierClassicPluginSystemName);
                _settingService.SaveSetting(_shippingSettings);
            }
            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        #endregion
    }
}
