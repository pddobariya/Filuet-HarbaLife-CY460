using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Factory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Shipping.Pickup;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping
{
    public class ShippingWidgetService : IShippingWidgetService
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IRepository<LocaleStringResource> _lsrRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;
        private readonly IRepository<StorePickupPoint> _storePickupPointRepository;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly ICountryService _countryService;
        private readonly IShippingModelFactory _shippingModelFactory;
        private readonly IGeoIpHelper _geoIpHelper;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IPluginService _pluginFinder;
        private readonly ILanguageService _languageService;
        private readonly IRepository<CustomerAddressMapping> _customerAddressMappingRepository;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ShippingWidgetService(
            IWorkContext workContext,
            IRepository<LocaleStringResource> lsrRepository,
            IGenericAttributeService genericAttributeService,
            IRepository<Address> addressRepository,
            IRepository<GenericAttribute> genericAttributeRepository,
            IRepository<StorePickupPoint> storePickupPointRepository,
            IFiluetShippingService filuetShippingService,
            ICountryService countryService,
            IShippingModelFactory shippingModelFactory,
            IGeoIpHelper geoIpHelper,
            IAddressService addressService, 
            ICustomerService customerService, 
            IStoreContext storeContext, 
            ILogger logger,
            IPluginService pluginFinder,
            ILanguageService languageService, 
            IRepository<CustomerAddressMapping> customerAddressMappingRepository,
            ISettingService settingService)
        {
            _workContext = workContext;
            _lsrRepository = lsrRepository;
            _genericAttributeService = genericAttributeService;
            _addressRepository = addressRepository;
            _genericAttributeRepository = genericAttributeRepository;
            _storePickupPointRepository = storePickupPointRepository;
            _filuetShippingService = filuetShippingService;
            _countryService = countryService;
            _shippingModelFactory = shippingModelFactory;
            _geoIpHelper = geoIpHelper;
            _addressService = addressService;
            _customerService = customerService;
            _storeContext = storeContext;
            _logger = logger;
            _pluginFinder = pluginFinder;
            _languageService = languageService;
            _customerAddressMappingRepository = customerAddressMappingRepository;
            _settingService = settingService;
        }
        #endregion

        #region Methods

        public async Task<Dictionary<string, string>> GetAllLocaleStringResourcesAsync()
        {
            var lang = await _workContext.GetWorkingLanguageAsync();
            return _lsrRepository.Table
                    .Where(p => p.LanguageId == lang.Id && p.ResourceName.Contains(ResourceKeys.Prefix))
                    .ToDictionary(t => t.ResourceName, t => t.ResourceValue);
        }

        public async Task<IEnumerable<DictionaryModel>> GetCitiesOfSelectedShippingComputationAsync()
        {
            var selectedShippingComputationOption =await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync((await _workContext.GetCurrentCustomerAsync()).Id)
                ?? throw new Exception("No shipping computation option found.");

            var countryCode =(await _filuetShippingService.GetShippingComputationOptionByIdAsync(selectedShippingComputationOption.FiluetFusionShippingComputationOptionId)).CountryCode;
            var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(countryCode)
                ?? throw new Exception("No country found.");

            var filter = new CitiesFilterDto { CountryId = country.Id };
            return (await SearchCitiesAsync(filter)).Select(p => new DictionaryModel
            {
                Id = p.Id.ToString(),
                Name = p.Name
            }).ToList();
        }

        public async Task<List<ShippingComputationOptionModel>> GetShippingComputationOptionsAsync()
        {
            var resultList = new List<ShippingComputationOptionModel>();
            var customer = await _workContext.GetCurrentCustomerAsync();

            var shippingComputationOptions =await _filuetShippingService.GetShippingComputationOptionsByContriesAsync();
            var shippingComputationOptionOfCustomers =await _filuetShippingService
                .GetShippingComputationOptionCustomerDataListByCustomerIdAsync(customer.Id);

            foreach (var option in shippingComputationOptions)
            {
                var isSelected = shippingComputationOptionOfCustomers.Any(p => p.FiluetFusionShippingComputationOptionId == option.Id && p.IsSelected);
                var optionModel = await _shippingModelFactory.PrepareShippingComputationOptionModelAsync(option, isSelected);
                resultList.Add(optionModel);
            }

            if (resultList.Count > 0 && !resultList.Any(x => x.IsSelected))
            {
                var scoSelected = await GetDefaultCountry(resultList);
                scoSelected.IsSelected = true;
               await SetShippingComputationOptionAsync(scoSelected);
            }

            return resultList;
        }

        private async Task<ShippingComputationOptionModel> GetDefaultCountry(List<ShippingComputationOptionModel> countries, string clientTimeZone = null)
        {
            ShippingComputationOptionModel scoSelected = null;
            Customer customer = await _workContext.GetCurrentCustomerAsync();
            if (await _geoIpHelper.IsCountryGeoCodedAsync(customer))
            {
                string selectedCountry = await _geoIpHelper.GetCountryByIpAddressAsync(customer, clientTimeZone);
                scoSelected = countries.Find(x => x.CountryCode == selectedCountry);
            }

            if (scoSelected == null)
            {
                //LV is default
                scoSelected = countries[0];
            }

            return scoSelected;
        }

        public async Task<List<ShippingMethodModel>> GetShippingMethodsAsync()
        {
            var shippingMethods = new List<ShippingMethodModel>();

            var customer = await _workContext.GetCurrentCustomerAsync();
            var selectedOption = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);

            if (selectedOption == null)
            {
                selectedOption = await SetDefaultShippingComputationOption() ?? throw new Exception("Not selectedOption found.");
            }

            var filuetFusionShippingComputationOption =await _filuetShippingService.GetShippingComputationOptionByIdAsync(selectedOption
                .FiluetFusionShippingComputationOptionId);
            if (filuetFusionShippingComputationOption.IsSalesCenter)
            {
                var address =await _addressService.GetAddressByIdAsync(selectedOption.AddressId);
                var shippingAddress = customer.ShippingAddressId is null ? null :await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value);
                var country = address?.CountryId is null
                    ? null
                    :await _addressService.GetAddressByIdAsync(address.CountryId.Value);
                var shippingMethodModel = new ShippingMethodModel
                {
                    IsSalesCenter = true,
                    MethodFriendlyName = filuetFusionShippingComputationOption.Name,
                    CountryName = country?.County,
                    City = address?.City,
                    Address = address?.Address1,
                    FirstName = shippingAddress.FirstName ?? customer.FirstName,
                    LastName = shippingAddress.LastName ?? customer.LastName,
                    PhoneNumber = shippingAddress.PhoneNumber ?? customer.Phone
                };
                shippingMethods.Add(shippingMethodModel);

                return shippingMethods;
            }

            var selectedShippingMethod = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.SelectedShippingMethodSystemName);

            var shippingProviders = await LoadAllFusionShippingProvidersAsync(customer, _storeContext.GetCurrentStore().Id);

            foreach (var shippingProvider in shippingProviders)
            {
                var isSelected = selectedShippingMethod == shippingProvider.PluginDescriptor.SystemName;
                ShippingMethodModel shippingMethod = null;

                if (shippingProvider is IShippingInformationProvider)
                {
                    shippingMethod = await _shippingModelFactory.PrepareShippingMethodModelAsync((IShippingInformationProvider)shippingProvider, isSelected);
                }

                if (shippingProvider is IPickupPointProvider)
                {
                    shippingMethod =await _shippingModelFactory.PrepareShippingMethodModelAsync((IPickupPointProvider)shippingProvider, isSelected);
                }

                if (shippingMethod != null)
                {
                    shippingMethods.Add(shippingMethod);
                }
            }

            shippingMethods = shippingMethods.Select((p, ind) =>
            {
                p.DisplayOrder = ind;
                return p;
            }).ToList();

            if (!shippingMethods.Any(p => p.IsSelected))
            {
                shippingMethods[0].IsSelected = true;
            }

            return shippingMethods;
        }

        private async Task<FiluetFusionShippingComputationOptionCustomerData> SetDefaultShippingComputationOption()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var salesCenter = (await _filuetShippingService.GetAllShippingComputationOptionsAsync()).FirstOrDefault(p => p.IsSalesCenter);

            var salesCenterAddress = GetSalesCenterAddress(salesCenter.Id);
            if (salesCenterAddress == null)
            {
                var msg = $"[{nameof(ShippingWidgetService)}.{nameof(SetShippingComputationOptionAsync)}] " +
                    $"No sales center address found.";
                await _logger.InsertLogAsync(LogLevel.Error, msg);
                throw new Exception(msg);
            }

            customer.ShippingAddressId = salesCenterAddress.Id;

            await _customerService.UpdateCustomerAsync(customer);

            var newCustomerOption = new FiluetFusionShippingComputationOptionCustomerData
            {
                Id = 0,
                FiluetFusionShippingComputationOptionId = salesCenter.Id,
                CustomerId = customer.Id,
                AddressId = salesCenterAddress.Id,
                IsSelected = true,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.Phone
            };

            await _filuetShippingService.AddShippingComputationOptionCustomerDataAsync(newCustomerOption);

            return await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
        }

        private async Task<Address> GetSalesCenterAddress(int storePickupPointId)
        {
            var salesCenterPickupPoint = _storePickupPointRepository.Table
                .FirstOrDefault(p => p.Id == storePickupPointId);

            return await _addressService.GetAddressByIdAsync(salesCenterPickupPoint.AddressId);
        }

        public async Task<IList<IFusionShippingProvider>> LoadAllFusionShippingProvidersAsync(Customer customer = null, int storeId = 0)
        {
            return (await _pluginFinder.GetPluginsAsync<IFusionShippingProvider>(customer: customer, storeId: storeId)).ToList();
        }

        public async Task<ShippingCountryPopupModel> PrepareSippingCountryPopupModelAsync()
        {
            ShippingCountryPopupModel model = new ShippingCountryPopupModel();
            Customer customer = await _workContext.GetCurrentCustomerAsync();
            model.IsShowShippingCountryPopup = await _geoIpHelper.IsShowShippingCountryPopupAsync(customer);
            return model;
        }

        public async Task<List<ShippingComputationOptionModel>> PrepareSippingCountrySelectorModelAsync()
        {
            var countries = await GetShippingComputationOptionsAsync();

            //we hardcode three baltics countries so we will use flags from language model
            var availableLanguages = (await _languageService
                    .GetAllLanguagesAsync(storeId: (await _storeContext.GetCurrentStoreAsync()).Id));
            countries.ForEach(c =>
            {
                c.FlagImageFileName = availableLanguages
                    .FirstOrDefault(l => l.UniqueSeoCode.ToUpper() == c.CountryCode.ToUpper())
                    ?.FlagImageFileName;
            });

            return countries;
        }

        public async Task<IEnumerable<CityDto>> SearchCitiesAsync(CitiesFilterDto filterDto)
        {
            var allPickupPointAddresses = GetPickupPointsQuery().Select(pickupPoint => pickupPoint.AddressId).ToArray();

            var citiesQuery = _addressRepository.Table.Where(p => allPickupPointAddresses.Contains(p.Id));

            if (filterDto.CountryId > 0)
            {
                citiesQuery = citiesQuery.Where(p => p.CountryId == filterDto.CountryId);
            }

            if (!string.IsNullOrEmpty(filterDto.Name))
            {
                citiesQuery = citiesQuery.Where(p => p.City == filterDto.Name);
            }

            var cities = citiesQuery.Select(p => p.City)
                .Distinct()
                .OrderBy(c => c)
                .ToList()
                .Select((c, idx) => new CityDto { Id = idx, Name = c });

            return await Task.FromResult(cities);
        }

        public async Task<IEnumerable<StorePickupPoint>> SearchPickupPointsAsync(PickupPointsFilterDto pickupPointsFilterDto)
        {
            var pickupPointsQuery = GetPickupPointsQuery(pickupPointsFilterDto.CountryId, pickupPointsFilterDto.City,
                pickupPointsFilterDto.NameOrAddress);

            return await pickupPointsQuery.ToListAsync();
        }

        private IQueryable<StorePickupPoint> GetPickupPointsQuery(int? countryId = null, string cityName = null,
            string nameOrAddress = null)
        {
            var externalIds = _genericAttributeRepository.Table.Where(ga => ga.KeyGroup == nameof(StorePickupPoint) && ga.Key == StorePickupPointCustomAttributeNames.ExternalId).Select(ga => ga.EntityId)
             .ToList();

            var pickupPointsQuery = _storePickupPointRepository.Table
                .Where(p => externalIds.Contains(p.Id));

            var addressIdsQuery = _addressRepository.Table;
            var hasCountryId = countryId != null;
            if (hasCountryId)
            {
                addressIdsQuery = addressIdsQuery.Where(a => a.CountryId == countryId);
            }

            var hasCity = !string.IsNullOrEmpty(cityName);
            if (hasCity)
            {
                addressIdsQuery = addressIdsQuery.Where(a => a.City.Contains(cityName));
            }

            var hasNameOrAddress = !string.IsNullOrEmpty(nameOrAddress);
            if (hasNameOrAddress)
            {
                addressIdsQuery = addressIdsQuery
                    .Where(a =>
                        a.Address1.Contains(nameOrAddress)
                        || a.Address2.Contains(nameOrAddress)
                        || a.Company.Contains(nameOrAddress));

                pickupPointsQuery = pickupPointsQuery
                    .Where(p => p.Name.Contains(nameOrAddress)
                                || p.Description.Contains(nameOrAddress));
            }

            var hasFilters = hasCity || hasCountryId || hasNameOrAddress;

            if (hasFilters)
            {
                var addressIds = addressIdsQuery.Select(a => a.Id).ToArray();
                pickupPointsQuery = pickupPointsQuery.Where(p => addressIds.Contains(p.AddressId));
            }

            return pickupPointsQuery;
        }

        public async Task SetShippingComputationOptionAsync(ShippingComputationOptionModel shippingCalculationOptionModel)
        {
            var customer =await _workContext.GetCurrentCustomerAsync();

            if (shippingCalculationOptionModel.IsSalesCenter)
            {
                var salesCenterId = shippingCalculationOptionModel.SalesCenterId ?? throw new ArgumentException();
                var salesCenterAddress = GetSalesCenterAddress(salesCenterId);
                if (salesCenterAddress == null)
                {
                    var msg = $"[{nameof(ShippingWidgetService)}.{nameof(SetShippingComputationOptionAsync)}] " +
                        $"No sales center address found.";
                    await _logger.InsertLogAsync(LogLevel.Error, msg);
                    throw new Exception(msg);
                }

                customer.ShippingAddressId = salesCenterAddress.Id;
            }
            else
            {
                var address = await(await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).FirstOrDefaultAwaitAsync(async p =>
                    (p.CountryId is null ? null : (await _countryService.GetCountryByIdAsync(p.CountryId.Value)))?.TwoLetterIsoCode == shippingCalculationOptionModel.CountryCode);
                if (address == null)
                {
                    var country =await _countryService.GetCountryByTwoLetterIsoCodeAsync(shippingCalculationOptionModel.CountryCode);
                    if (country == null)
                    {
                        var msg = $"[{nameof(ShippingWidgetService)}.{nameof(SetShippingComputationOptionAsync)}] " +
                            $"No {shippingCalculationOptionModel.CountryCode} country found.";
                        await _logger.InsertLogAsync(LogLevel.Error, msg);
                        throw new Exception(msg);
                    }

                    address = await CreateEmptyAddress(country.Id);
                    await _addressService.InsertAddressAsync(address);

                   await _customerAddressMappingRepository.InsertAsync(new CustomerAddressMapping
                    {
                        AddressId = address.Id,
                        CustomerId = customer.Id
                    });
                }

                customer.ShippingAddressId = address.Id;
            }

            await _customerService.UpdateCustomerAsync(customer);

            var shippingAddress =await _addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value);

            var customerOption = await _filuetShippingService.GetShippingComputationOptionCustomerDataByCustomerIdAndOptionIdAsync(customer.Id, shippingCalculationOptionModel.Id);
            if (customerOption == null)
            {
                var newCustomerOption = new FiluetFusionShippingComputationOptionCustomerData
                {
                    FiluetFusionShippingComputationOptionId = shippingCalculationOptionModel.Id,
                    CustomerId = customer.Id,
                    AddressId = shippingAddress.Id,
                    IsSelected = true,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PhoneNumber = customer.Phone
                };

              await _filuetShippingService.AddShippingComputationOptionCustomerDataAsync(newCustomerOption);
            }
            else
            {
                customerOption.AddressId = shippingAddress.Id;
                customerOption.IsSelected = true;
              await _filuetShippingService.UpdateShippingComputationOptionCustomerDataaAsync(customerOption);
            }
        }

        private async Task<Address> CreateEmptyAddress(int? countryId = null)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var address = new Address
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.Phone
            };

            if (countryId.HasValue)
            {
                address.CountryId = countryId.Value;
            }

            return address;
        }

        public async Task UpdateShippingInformationAsync(ShippingMethodModel shippingMethodModel)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            // ShippingInformationProviders
            var shippingInformationProviders = await LoadAllShippingInformationProvidersAsync(customer, _storeContext.GetCurrentStore().Id);
            var isShippingInformationProvider = shippingInformationProviders
                .Any(p => p.PluginDescriptor.SystemName == shippingMethodModel.MethodSystemName);

            if (isShippingInformationProvider)
            {
                await UpdateShippingAddressByShippingInformationProvider(shippingMethodModel);
            }
            else
            {

                // PickupPointProviders
                var pickupPointProviders = (await _settingService.LoadSettingAsync<ShippingSettings>()).ActivePickupPointProviderSystemNames;
                var ispickupPointProvider = await pickupPointProviders.AnyAwaitAsync(async p => (await _pluginFinder.GetPluginDescriptorBySystemNameAsync<IPlugin>(p)).SystemName == shippingMethodModel.MethodSystemName);
                if (ispickupPointProvider)
                {
                  await UpdateShippingAddressByPickupPointProvider(shippingMethodModel);
                }
            }

            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.SelectedShippingMethodSystemName,
                shippingMethodModel.MethodSystemName);
        }

        private async Task UpdateShippingAddressByPickupPointProvider(ShippingMethodModel shippingMethodModel)
        {
            var customer =await _workContext.GetCurrentCustomerAsync();

            var pickupPointId = shippingMethodModel.PickupPointId
                ?? throw new ArgumentException("PickupPointId is null.");

            var existingPickupPoint = _storePickupPointRepository.Table.First(p => p.Id == shippingMethodModel.PickupPointId.Value)
                ?? throw new Exception("No PickupPoint found.");

            var addressPickupPoint =await _addressService.GetAddressByIdAsync(existingPickupPoint.AddressId)
                ?? throw new Exception("No address found for PickupPoint.");
            var country = addressPickupPoint?.CountryId is null
                ? null
                : await _countryService.GetCountryByIdAsync(addressPickupPoint.CountryId.Value);

            customer.ShippingAddressId = addressPickupPoint.CountryId;
            await _customerService.UpdateCustomerAsync(customer);

            var selectedCustomerOptionCustomerData = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id)
                ?? throw new Exception("No ShippingComputationOptionCustomerData found.");

            selectedCustomerOptionCustomerData.FirstName = shippingMethodModel.FirstName;
            selectedCustomerOptionCustomerData.LastName = shippingMethodModel.LastName;
            selectedCustomerOptionCustomerData.PhoneNumber = shippingMethodModel.PhoneNumber;

            await _filuetShippingService.UpdateShippingComputationOptionCustomerDataaAsync(selectedCustomerOptionCustomerData);

            var pickupPoint = new PickupPoint
            {
                Id = existingPickupPoint.Id.ToString(),
                Address = addressPickupPoint.Address1,
                City = addressPickupPoint.City,
                County = addressPickupPoint.County,
                CountryCode = country?.TwoLetterIsoCode,
                ZipPostalCode = addressPickupPoint.ZipPostalCode
            };
            await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPickupPointAttribute,
                pickupPoint,(await _storeContext.GetCurrentStoreAsync()).Id);
        }

        private async Task UpdateShippingAddressByShippingInformationProvider(ShippingMethodModel shippingMethodModel)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var shippingAddress = await GetCustomerShippingAddress();

            if (shippingAddress == null)
            {
                shippingAddress = shippingMethodModel.ToAddressEntity();
                await _addressService.InsertAddressAsync(shippingAddress);
                await _customerAddressMappingRepository.InsertAsync(new CustomerAddressMapping
                {
                    AddressId = shippingAddress.Id,
                    CustomerId = customer.Id
                });
            }
            else
            {
                await _addressRepository.UpdateAsync(shippingMethodModel.ToAddressEntity(shippingAddress));
            }

            customer.ShippingAddressId = shippingAddress.Id;
            customer.BillingAddressId = shippingAddress.Id;
            await _customerService.UpdateCustomerAsync(customer);

            foreach (var additionalShippingField in shippingMethodModel.AdditionalShippingFields)
            {
               await _genericAttributeService.SaveAttributeAsync(customer, additionalShippingField.NameResourceKey, additionalShippingField.Value);
            }
        }

        private async Task<Address> GetCustomerShippingAddress()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            return (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))?.FirstOrDefault(p => p.Id == customer.ShippingAddressId);
        }

        public async virtual Task<List<IShippingInformationProvider>> LoadAllShippingInformationProvidersAsync(Customer customer = null, int storeId = 0)
        {
            return (await _pluginFinder.GetPluginsAsync<IShippingInformationProvider>(customer: customer, storeId: storeId)).ToList();
        }

        #endregion

    }
}