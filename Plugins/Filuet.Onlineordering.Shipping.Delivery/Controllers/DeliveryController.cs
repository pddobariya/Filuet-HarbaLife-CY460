using DocumentFormat.OpenXml.Spreadsheet;
using Filuet.Onlineordering.Shipping.Delivery.Constants;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Controllers
{
    public class DeliveryController : BasePluginController
    {
        #region Fields

        private readonly IDeliveryPriceService _deliveryPriceService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICountryDeliveryCustomizingService _countryDeliveryCustomizing;
        private readonly DeliveryPluginSettings _deliveryPluginSettings;
        private readonly IDistributorService _distributorService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService;


        #endregion

        #region Ctor

        public DeliveryController(IDeliveryPriceService deliveryPriceService,
            ILanguageService languageService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            ICountryDeliveryCustomizingService countryDeliveryCustomizing,
            DeliveryPluginSettings deliveryPluginSettings,
            IDistributorService distributorService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IFiluetShippingService filuetShippingService,
            IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService deliveryOperatorDeliveryTypeDeliveryCityDependenciesService)
        {
            _deliveryPriceService = deliveryPriceService;
            _languageService = languageService;
            _workContext = workContext;
            _localizationService = localizationService;
            _genericAttributeService = genericAttributeService;
            _countryDeliveryCustomizing = countryDeliveryCustomizing;
            _deliveryPluginSettings = deliveryPluginSettings;
            _distributorService = distributorService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _filuetShippingService = filuetShippingService;
            _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService = deliveryOperatorDeliveryTypeDeliveryCityDependenciesService;
        }

        #endregion

        #region Methods

        public async Task<JsonResult> GetDeliveryTypes()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            return await Task.Run(async () =>
            {
                var deliveryTypeDtos = (await _deliveryPriceService.GetDeliveryTypesAsync(language.Id)).Where(dt => dt.IsActive).ToList();

                if (await _countryDeliveryCustomizing.IsOnlySelfPickup())
                {
                    deliveryTypeDtos.ForEach(dt => dt.IsBlocked = true);
                }
                else
                {
                    deliveryTypeDtos.ForEach(dt => dt.IsBlocked = false);
                }

                if (_deliveryPluginSettings.SelfPickupActive && (await GetSalesCentersImpAsync()).Any())
                {
                    deliveryTypeDtos.Add(new DeliveryTypeDto
                    {
                        Id = 0,
                        TypeName = await _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Pickup")
                    });
                }

                return new JsonResult(deliveryTypeDtos);

            });
        }

        public async Task<JsonResult> GetSalesCenters()
        {
            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));

            return await Task.Run(async () =>
            {
                return new JsonResult(await GetSalesCentersImpAsync());
            });
        }

        public async Task<JsonResult> GetDeliveryOperatorsCities()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            var filuetFusionShippingComputationOptionCustomerData = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(
                (customer.Id));
            var filuetFusionShippingComputationOption = await _filuetShippingService.GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData
                .FiluetFusionShippingComputationOptionId);

            var cultureInfo = await GetCultureInfoAsync(filuetFusionShippingComputationOption);
            var wareHouses = filuetFusionShippingComputationOptionCustomerData is null ? null : filuetFusionShippingComputationOption?.WarehouseCode.Split(';');
            return await Task.Run(async () => new JsonResult((await _deliveryPriceService.GetDeliveryOperatorsCitiesDtoAsync(language.Id, wareHouses, cultureInfo)).ToList()));

        }

        public async Task<JsonResult> GetDeliveryOperators()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            var deliveryOperators = (await _deliveryPriceService.GetDeliveryOperatorDtosAsync(language.Id)).ToList();
            return await Task.Run(() => new JsonResult(deliveryOperators));
        }

        public async Task<JsonResult> GetDeliveryAddresses()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();
            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            var filuetFusionShippingComputationOptionCustomerData = (await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(
                (customer.Id)));
            var filuetFusionShippingComputationOption = (await _filuetShippingService.GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData
                .FiluetFusionShippingComputationOptionId));
            var cultureInfo = await GetCultureInfoAsync(filuetFusionShippingComputationOption);
            var localComparer = new LocalComparer(cultureInfo);

            return await Task.Run(async () =>
            {
                List<string> addresses = new List<string>();
                var streetAddressAttribute = customer.StreetAddress;
                if (streetAddressAttribute != null)
                {
                    addresses.Add(streetAddressAttribute);
                }
                streetAddressAttribute = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetCurrentCustomerAsync(), CoreGenericAttributes.StreetAddressAttribute);
                if (streetAddressAttribute != null)
                {
                    addresses.AddRange(JsonConvert.DeserializeObject<List<string>>(streetAddressAttribute));
                }

                return new JsonResult(addresses.OrderBy(x => x, localComparer));
            });
        }

        public async Task<JsonResult> GetAutoPostOffices()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));
            var filuetFusionShippingComputationOptionCustomerData = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(
                (customer.Id));
            var filuetFusionShippingComputationOption = await _filuetShippingService.GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData
                .FiluetFusionShippingComputationOptionId);
            var cultureInfo = await GetCultureInfoAsync(filuetFusionShippingComputationOption);
            return await Task.Run(async () => new JsonResult((await _deliveryPriceService.GetAutoPostOfficeDtosAsync(language.Id, cultureInfo)).ToList()));

        }

        public async Task<JsonResult> LoadLocalization()
        {
            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));

            return await Task.Run(() => new JsonResult(new { TheCostWillBe = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.TheCostWillBe"), WorkTime = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Fields.WorkTime"), AboveReceiverName = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.AboveReceiverName"), ReceiverName = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.ReceiverName"), BelowReceiverName = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.BelowReceiverName"), Phone = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Phone"), EnterPhoneFormat = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.EnterPhoneFormat"), Comment = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Comment"), PickupPointInfo = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.PickupPointInfo"), City = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Fields.City"), PickupCenterAddress = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.PickupCenterAddress"), DeliveryCity = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.DeliveryCity"), DeliveryOperator = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.DeliveryOperator"), PickupPointCity = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.PickupPointCity"), PickupPointOperator = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.PickupPointOperator"), PickupPointAddress = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.PickupPointAddress"), Address = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Fields.Address"), DeliveryTypesBlockedMessage = _localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.DeliveryTypesBlockedMessage") }));
        }

        public async Task<JsonResult> LoadMiscs()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();
            var currencycode = await _workContext.GetWorkingCurrencyAsync();

            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));

            List<string> addresses = new List<string>();
            List<string> phones = new List<string>();

            var streetAddressAttribute = customer.StreetAddress;
            var phoneAttribute = customer.Phone;

            if (streetAddressAttribute == null || phoneAttribute == null)
            {
                var distributorFullProfile = await _distributorService.GetDistributorFullProfileAsync(customer);
                if (streetAddressAttribute == null)
                {
                    streetAddressAttribute = distributorFullProfile.DistributorDetailedProfileResponse?.Addresses
                        ?.MailingAddress?.FullAddress;
                    await _genericAttributeService.SaveAttributeAsync(customer, streetAddressAttribute, streetAddressAttribute);
                }

                if (phoneAttribute == null)
                {
                    phoneAttribute = distributorFullProfile.DistributorDetailedProfileResponse?.Phones.FirstOrDefault();
                    await _genericAttributeService.SaveAttributeAsync(customer, phoneAttribute, phoneAttribute);
                }
            }

            if (streetAddressAttribute != null)
            {
                addresses.Add(streetAddressAttribute);
            }
            streetAddressAttribute = customer.StreetAddress;
            if (streetAddressAttribute != null)
            {
                addresses.AddRange(JsonConvert.DeserializeObject<List<string>>(streetAddressAttribute));
            }
            if (!addresses.Any())
            {
                addresses.Add(
                    customer.StreetAddress2);
            }

            if (phoneAttribute != null)
            {
                phones.Add(phoneAttribute);
            }
            phoneAttribute = customer.Phone;
            if (phoneAttribute != null)
            {
                phones.AddRange(JsonConvert.DeserializeObject<List<string>>(phoneAttribute));
            }
            phones = phones.Where(phone => !string.IsNullOrWhiteSpace(phone)).Select(phone =>
                phone.Replace(" ", string.Empty).Replace("-", string.Empty).Replace("+", string.Empty)).ToList();
            IPhoneFormatter phoneFormatter = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                phoneFormatter = serviceScope.ServiceProvider.GetService<IPhoneFormatter>();
            }
            var filuetFusionShippingComputationOptionCustomerData = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
            var phonePrefix = await phoneFormatter?.FormatPrefixAsync(_deliveryPluginSettings.PhonePrefix,
                filuetFusionShippingComputationOptionCustomerData?.FiluetFusionShippingComputationOption.CountryCode) ?? _deliveryPluginSettings.PhonePrefix;
            if (!string.IsNullOrEmpty(phonePrefix))
            {
                phones = phones.Select(phone =>
                    phone.Replace(phonePrefix, string.Empty)).ToList();
            }

            return await Task.Run(() => new JsonResult(new { ReceiverName = string.Join(' ', customer.LastName), PhonePrefix = phonePrefix, Phones = phones, Email = customer.Email, Addresses = addresses, CurrencyCode = currencycode.CurrencyCode, CriterionValue = _countryDeliveryCustomizing.GetDeliveryPriceCriterionValue() }));
        }

        public async Task<JsonResult> GetOperatorPrice([FromBody] int dodtdcd)
        {

            if (dodtdcd == 0)
            {
                try
                {
                    if (Request.Query["dodtdcd"].Count > 0)
                        dodtdcd = int.Parse(Request.Query["dodtdcd"]);
                }
                catch (Exception) { }
            }

            Response.Headers.Add(new KeyValuePair<string, StringValues>("Access-Control-Allow-Origin", "*"));

            var criterion = (await _countryDeliveryCustomizing.GetDeliveryPriceCriterionValue());
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            var priceResult = await Task.Run(async () =>
            {
                var operatorPrice = (await _deliveryPriceService.GetOperatorPriceAsync(dodtdcd)).FirstOrDefault(p => p.MinCriterionValue <= criterion && p.MaxCriterionValue > criterion);
                if (criterion >= _deliveryPluginSettings.MinCriterion)
                {
                    var operatorPrice1 = (await _deliveryPriceService.GetOperatorPriceAsync(dodtdcd)).FirstOrDefault();
                    var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService.GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(dodtdcd);
                    if (deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId == (int)ShipingMethodEnum.PickPoint)
                    {
                        operatorPrice1.DeliveryPrise = Math.Round((criterion * (_deliveryPluginSettings.PickPoint)) / 100, 2);
                    }
                    if (deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId == (int)ShipingMethodEnum.Delivery)
                    {
                        operatorPrice1.DeliveryPrise = Math.Round((criterion * (_deliveryPluginSettings.HomeDelivery)) / 100, 2);
                    }
                    await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.DeliveryPrice, operatorPrice1?.DeliveryPrise ?? 0);
                    return new JsonResult(operatorPrice1);
                }
                else
                {
                    await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.DeliveryPrice, operatorPrice?.DeliveryPrise ?? 0);
                }
                return new JsonResult(operatorPrice);
            });

            return priceResult;
        }

        public async Task<JsonResult> CostsAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();

            return new JsonResult(_deliveryPriceService.GetDeliveryOperatorsCitiesDtoAsync(language.Id));
        }

        public async Task<JsonResult> PickupCostsAsync()
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            return new JsonResult(await _deliveryPriceService.GetSalesCentersAsync(language.Id));
        }

        private async Task<SalesCenterDto[]> GetSalesCentersImpAsync()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            decimal? cartVP = null;
            if (_deliveryPluginSettings.SelfPickupActive)
            {

                cartVP = await (await _shoppingCartService.GetShoppingCartAsync(currentCustomer)).AggregateAwaitAsync(0m,
                async (sum, sci) => (await _genericAttributeService.GetAttributeAsync<decimal>(await _productService.GetProductByIdAsync(sci.ProductId), ProductAttributeNames.VolumePoints) * sci.Quantity) +
                              sum);
            }

            var filuetFusionShippingComputationOptionCustomerData = await _filuetShippingService.GetSelectedShippingComputationOptionCustomerDataAsync(currentCustomer.Id);

            var salesCenters = (await _deliveryPriceService.GetSalesCentersAsync(language.Id)).Where(sc =>
            {
                if (string.IsNullOrWhiteSpace(sc.VolumePoints))
                    return true;
                var range = sc.VolumePoints.Split(';').Select(vp => decimal.Parse(vp)).ToArray();
                return cartVP != null && range.Length == 2 && range[0] <= cartVP && range[1] > cartVP;
            });


            var shippingComputationOption = await _filuetShippingService.GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData.FiluetFusionShippingComputationOptionId);

            var salesCenterDto = salesCenters.Where(scc =>
                filuetFusionShippingComputationOptionCustomerData == null || shippingComputationOption.WarehouseCode.Split(';').Contains(scc.WarehouseCode)).ToArray();

            return salesCenterDto;
        }

        private async Task<CultureInfo> GetCultureInfoAsync(FiluetFusionShippingComputationOption filuetFusionShippingComputationOption)
        {
            var languageCulture = (await _languageService.GetAllLanguagesAsync()).FirstOrDefault(lang =>
                lang.Name.Equals(filuetFusionShippingComputationOption.CountryCode,
                    StringComparison.InvariantCultureIgnoreCase))?.LanguageCulture;
            return languageCulture == null ? null : new CultureInfo(languageCulture);
        }

        #endregion
    }
}
