using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Factory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Controllers
{

    [Route("api/shipping", Name = "shipping")]
    public class ShippingController : ApiControllerBase
    {
        #region Fields

        private readonly IShippingModelFactory _shippingModelFactory;
        private readonly IShippingWidgetService _shippingWidgetService;        
        private readonly IWorkContext _workContext;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly IFiluetShippingCartService _filuetShippingCartService;
        private readonly ICrmDataProviderAdapter _crmDataProvider;
        private readonly IDistributorService _distributorService;
        private readonly IGeoIpHelper _geoIpHelper;
        private readonly ILogger _logger;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public ShippingController(
            IShippingModelFactory shippingModelFactory,
            IShippingWidgetService shippingWidgetService,
            IWorkContext workContext,
            IDualMonthsService dualMonthsService,
            IFiluetShippingCartService filuetShippingCartService,
            IGeoIpHelper geoIpHelper,
            ICrmDataProviderAdapter crmDataProvider,
            IDistributorService distributorService,
            ILogger logger,
            INotificationService notificationService,
            ILocalizationService localizationService)
        {
            _shippingModelFactory = shippingModelFactory;
            _shippingWidgetService = shippingWidgetService;
            _workContext = workContext;
            _dualMonthsService = dualMonthsService;
            _filuetShippingCartService = filuetShippingCartService;
            _geoIpHelper = geoIpHelper;
            _crmDataProvider = crmDataProvider;
            _distributorService = distributorService;
            _logger = logger;
            _notificationService = notificationService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        #region GetShippingMethods

        [HttpGet("shipping_methods")]
        public virtual async Task<IActionResult> GetShippingMethods()
        {
            try
            {
                return Ok(await _shippingWidgetService.GetShippingMethodsAsync());
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #region SearchPickupPoints

        [HttpPost("pickup_points", Name = "pickup_points")]
        public virtual async Task<IActionResult> SearchPickupPoints(PickupPointsFilterModel filter)
        {
            try
            {
                var pickupPoints =await _shippingWidgetService.SearchPickupPointsAsync(filter.ToDto());

                var models = pickupPoints.Select(async storePickupPoint =>
                    await _shippingModelFactory.PreparePickupPointModelAsync(storePickupPoint));
                return Ok(models);
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #region UpdateShippingInformation

        [HttpPost("update_info")]
        public virtual async Task<IActionResult> UpdateShippingInformation(ShippingMethodModel shippingMethodModel)
        {
            try
            {                
                await _shippingWidgetService.UpdateShippingInformationAsync(shippingMethodModel);
                return Ok();
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }            
        }

        #endregion

        #region SearchCities

        [HttpPost("search_cities")]
        public virtual async Task<IActionResult> SearchCities(CitiesFilterModel filter)
        {
            try
            {
                var filterDto = new CitiesFilterDto
                {
                    CountryId = filter.CountryId,
                    Name = filter.Name
                };

                var citiesDto =await _shippingWidgetService.SearchCitiesAsync(filterDto);

                var cityModels = citiesDto.Select(c => new CityModel { Id = c.Id, Name = c.Name });

                return Ok(cityModels);
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #region GetCitiesOfSelectedShippingComputation

        [HttpGet("cities_of_selected_shipping_computation")]
        public virtual async Task<IActionResult> GetCitiesOfSelectedShippingComputation()
        {
            try
            {
                return Ok(await _shippingWidgetService.GetCitiesOfSelectedShippingComputationAsync());
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #region GetPluginAllLocaleStringResources

        [HttpGet("get_plugin_all_resources")]
        public virtual async Task<IActionResult> GetPluginAllLocaleStringResources()
        {            
            try
            {
                var result =await _shippingWidgetService.GetAllLocaleStringResourcesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }            
        }

        #endregion

        #region GetShippingCalculation

        [HttpGet("get_shipping_calculation")]
        [Produces("application/json")]
        public virtual async Task<IActionResult> GetShippingCalculation()
        {
            try
            {
                var shippingCalculationMonthModels =(await _dualMonthsService.GetAvailableMonthsAsync())
                    .Select(p => new ShippingCalculationMonthModel
                    {
                        DisplayName = p.DisplayName,
                        Timestamp = p.Timestamp,
                        IsSelected = false //p.IsSelected
                    }).ToList();
                shippingCalculationMonthModels.Insert(0, new ShippingCalculationMonthModel
                {
                    DisplayName = "Select",
                    Timestamp = 0,
                    IsSelected = true
                });
                var shippingCalculationModel = new ShippingCalculationModel
                {
                    Options = await _shippingWidgetService.GetShippingComputationOptionsAsync(),
                    AvailableMonths = shippingCalculationMonthModels,
                    IsAllowMonthSelect = await _dualMonthsService.GetDualMonthAllowedAsync()
                };

                return Ok(shippingCalculationModel);
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #region SetShippingComputationOption

        [HttpPost("set_shipping_computation_option")]
        public virtual async Task<IActionResult> SetShippingComputationOption(ShippingComputationOptionModel model)
        {
            try
            {
                await _shippingWidgetService.SetShippingComputationOptionAsync(model);
                var skus = await _filuetShippingCartService.IsCartValid();

                Customer customer = await _workContext.GetCurrentCustomerAsync();
                await _geoIpHelper.SaveGeoCodedCountryAsync(customer, model.CountryCode);

                // Update customer role
                var distributorFullProfile = await _crmDataProvider.GetDistributorFullProfileAsync(customer);

                // Check if distributorFullProfile is not null before accessing properties
                if (distributorFullProfile != null)
                    await _distributorService.SetDistributorRegionalRole(customer, distributorFullProfile.Discount, model.CountryCode);

                else
                {
                    await _logger.ErrorAsync(string.Format("DistributorFullProfile is null for customer {0}", customer.Id), null, customer);
                    return Ok(new { isCartValid = !skus.Any(), skus = skus, distributorNull = true });
                }

                return Ok($"{{\"isCartValid\": \"{!skus.Any()}\", \"skus\": \"{string.Join(", ", skus)}\"}}");
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #region UpdateDistributorLimits

        [HttpPost("update_distributor_limits")]
        public virtual async Task<IActionResult> UpdateDistributorLimits([FromBody]long? selectedLimitsMonthTimestamp)
        {
            try
            {
                if (selectedLimitsMonthTimestamp.HasValue)
                {
                    DateTime monthDate = selectedLimitsMonthTimestamp.Value.FromUnixTimestamp();
                     await _dualMonthsService.UpdateSelectedLimitsAsync(await _workContext.GetCurrentCustomerAsync(), monthDate);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return SaveError(ex);
            }
        }

        #endregion

        #endregion

    }
}