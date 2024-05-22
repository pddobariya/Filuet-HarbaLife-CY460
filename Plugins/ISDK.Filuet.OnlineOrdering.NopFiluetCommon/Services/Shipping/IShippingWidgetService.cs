using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping
{
    public interface IShippingWidgetService
    {
        #region Methods

        Task<IEnumerable<StorePickupPoint>> SearchPickupPointsAsync(PickupPointsFilterDto pickupPointsFilterDto);

        Task UpdateShippingInformationAsync(ShippingMethodModel shippingMethodModel);

        Task<IEnumerable<CityDto>> SearchCitiesAsync(CitiesFilterDto filterDto);

        Task<IEnumerable<DictionaryModel>> GetCitiesOfSelectedShippingComputationAsync();

        Task<Dictionary<string, string>> GetAllLocaleStringResourcesAsync();

        Task<List<ShippingComputationOptionModel>> GetShippingComputationOptionsAsync();

        Task SetShippingComputationOptionAsync(ShippingComputationOptionModel shippingCalculationOptionModel);

        Task<List<ShippingMethodModel>> GetShippingMethodsAsync();

        Task<IList<IFusionShippingProvider>> LoadAllFusionShippingProvidersAsync(Customer customer = null, int storeId = 0);

        Task<List<ShippingComputationOptionModel>> PrepareSippingCountrySelectorModelAsync();

        Task<ShippingCountryPopupModel> PrepareSippingCountryPopupModelAsync();

        #endregion
    }
}