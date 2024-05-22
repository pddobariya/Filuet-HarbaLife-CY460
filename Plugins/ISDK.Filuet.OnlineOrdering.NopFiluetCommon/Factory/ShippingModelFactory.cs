using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.DTO;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Shipping.Pickup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Factory
{
    public class ShippingModelFactory : IShippingModelFactory
    {
        #region Fields

        private readonly IAddressService _addressService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IFiluetShippingService _filuetShippingService;        
        private readonly IRepository<StorePickupPoint> _storePickupPointRepository;


        #endregion

        #region Ctor

        public ShippingModelFactory(
            IAddressService addressService,
            IWorkContext workContext, 
            ILocalizationService localizationService, 
            IGenericAttributeService genericAttributeService, 
            IFiluetShippingService filuetShippingService, 
            IRepository<StorePickupPoint> storePickupPointRepository)
        {
            _addressService = addressService;
            _workContext = workContext;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _filuetShippingService = filuetShippingService;
            _storePickupPointRepository = storePickupPointRepository;
        }


        #endregion

        #region Methods

        public async Task<DictionaryModel> PreparePickupPointModelAsync(StorePickupPoint storePickupPoint)
        {
            var address = await _addressService.GetAddressByIdAsync(storePickupPoint.AddressId);

            return new DictionaryModel
            {
                Id = storePickupPoint.Id.ToString(),
                Name = address?.Address1
            };
        }

        public async Task<ShippingComputationOptionModel> PrepareShippingComputationOptionModelAsync(FiluetFusionShippingComputationOption filuetFusionShippingComputationOption,
            bool isSelected)
        {
            var resultModel = new ShippingComputationOptionModel
            {
                Id = filuetFusionShippingComputationOption.Id,                
                Name = await _localizationService.GetLocalizedAsync(filuetFusionShippingComputationOption, range => range.Name),
                DisplayOrder = filuetFusionShippingComputationOption.DisplayOrder,
                CountryCode = filuetFusionShippingComputationOption.CountryCode,
                ProcessingLocationCode = filuetFusionShippingComputationOption.ProcessingLocationCode,
                WarehouseCode = filuetFusionShippingComputationOption.WarehouseCode,
                IsSalesCenter = filuetFusionShippingComputationOption.IsSalesCenter,
                SalesCenterId = filuetFusionShippingComputationOption.SalesCenterId,
                IsSelected = isSelected,
            };

            return resultModel;
        }

        public async Task<ShippingMethodModel> PrepareShippingMethodModelAsync(IShippingInformationProvider shippingInformationProvider, bool isSelected)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var shippingAddress = await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value);
            var shippingMethod = new ShippingMethodModel
            {
                MethodFriendlyName = shippingInformationProvider.PluginDescriptor.FriendlyName,
                MethodSystemName = shippingInformationProvider.PluginDescriptor.SystemName,
                IsSelected = isSelected,
                FirstName = shippingAddress.FirstName ?? customer.FirstName,
                LastName = shippingAddress.LastName ?? customer.LastName,
                PhoneNumber = shippingAddress.PhoneNumber ?? customer.Phone,
                Address = shippingAddress?.Address1 ?? string.Empty,
                City = shippingAddress?.City ?? string.Empty,
                CountryName = shippingAddress?.County ?? string.Empty,
                CountryId = shippingAddress?.CountryId,
                ZipPostalCode = shippingAddress?.ZipPostalCode,
                PickupPointId = null
            };

            shippingMethod.PhoneNumber = shippingMethod.PhoneNumber == NopFiluetCommonDefaults.EmptyDisplayPlaceholder 
                ? string.Empty : shippingMethod.PhoneNumber;

            var additionalShippingFields = shippingInformationProvider.GetAdditionalShippingFields()
            .SelectAwait(async p => await PrepareFormFieldMetaModelAsync(p));
            shippingMethod.AdditionalShippingFields = await additionalShippingFields.ToListAsync();
            shippingMethod.HiddenShippingFields = shippingInformationProvider.GetHiddenShippingFields();

            return shippingMethod;
        }

        public async Task<ShippingMethodModel> PrepareShippingMethodModelAsync(IPickupPointProvider pickupPointProvider, bool isSelected)
        {
            var customer =await _workContext.GetCurrentCustomerAsync();
            var selectedCustomerOption =await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
            var shippingAddress = await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value);
            StorePickupPoint storePickupPoint = null;
            if (customer.ShippingAddressId != null) {
                storePickupPoint = _storePickupPointRepository.Table.FirstOrDefault(p => p.AddressId == customer.ShippingAddressId);
            }

            var shippingMethod = new ShippingMethodModel
            {
                MethodFriendlyName = pickupPointProvider.PluginDescriptor.FriendlyName,
                MethodSystemName = pickupPointProvider.PluginDescriptor.SystemName,
                IsSelected = isSelected,
                FirstName = selectedCustomerOption?.FirstName ?? customer.FirstName,
                LastName = selectedCustomerOption?.LastName ?? customer.FirstName,
                PhoneNumber = selectedCustomerOption?.PhoneNumber ?? customer.Phone,
                Address = shippingAddress?.Address1 ?? string.Empty,
                City = shippingAddress?.City ?? string.Empty,
                CountryName = shippingAddress?.County ?? string.Empty,
                CountryId = shippingAddress?.CountryId,
                ZipPostalCode = shippingAddress?.ZipPostalCode,
                PickupPointId = storePickupPoint?.Id,
                HiddenShippingFields = new List<string>
                {
                    ShippingFieldNames.ZipPostalCodeTextBox,
                    ShippingFieldNames.CityTextBox,
                    ShippingFieldNames.AddressTextBox
                }
            };

            return shippingMethod;
        }

        public async Task<FormFieldMetaModel> PrepareFormFieldMetaModelAsync(FormFieldMeta formFieldMeta)
        {
            var customer =await _workContext.GetCurrentCustomerAsync();
            return new FormFieldMetaModel
            {
                NameResourceKey = formFieldMeta.NameResourceKey,
                ControlType = (int)formFieldMeta.ControlType,
                HelptextResourceKey = formFieldMeta.HelptextResourceKey,
                PlaceholderResourceKey = formFieldMeta.PlaceholderResourceKey,
                Value = await _genericAttributeService.GetAttributeAsync<string>(customer,formFieldMeta.NameResourceKey)

            };
        }

        #endregion
    }
}