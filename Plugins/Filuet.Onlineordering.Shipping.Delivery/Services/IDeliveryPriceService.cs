using Filuet.Onlineordering.Shipping.Delivery.Constants;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public interface IDeliveryPriceService
    {
        #region Methods

        Task<SalesCenterDto[]> GetSalesCentersAsync(int languageId);
        Task<SalesCenterDto> GetSalesCenterByIdAsync(int id, int languageId);
        Task UpdateSalesCenterAsync(SalesCenterDtoModel model, int languageId);
        Task DeleteSalesCenterAsync(SalesCenterDtoModel model, int languageId);
        Task CreateSalesCenterAsync(SalesCenterDtoModel model, int languageId);

        Task<DeliveryTypeDto[]> GetDeliveryTypesAsync(int languageId);
        Task DeliveryTypeUpdateAsync(DeliveryTypeDto model, int languageId);

       // Task<DeliveryOperator[]> GetDeliveryOperatorsAsync();
        Task<DeliveryOperatorDto[]> GetDeliveryOperatorDtosAsync(int languageId);
        Task UpdateDeliveryOperatorAsync(DeliveryOperatorDtoModel model, int languageId);
        Task DeleteDeliveryOperatorAsync(DeliveryOperatorDtoModel model, int languageId);
        Task CreateDeliveryOperatorAsync(DeliveryOperatorDtoModel model, int languageId);

        Task<DeliveryOperatorsCityDto[]> GetDeliveryOperatorsCitiesDtoAsync(int languageId, string[] wareHouses = null, CultureInfo cultureInfo = null);
        Task<DeliveryCity[]> GetCitiesAsync();
        Task<CityViewModel[]> GetDeliveryCitiesAsync(ShipingMethodEnum shipingMethod, string[] wareHouses = null);
        Task UpdateDeliveryCityAsync(DeliveryOperatorsCityModel model, int languageId);
        Task DeleteDeliveryCityAsync(DeliveryOperatorsCityModel model, int languageId);
        Task CreateDeliveryCityAsync(DeliveryOperatorsCityModel model);

        Task<IEnumerable<PriceDto>> GetPricesAsync();
        Task<PriceDto[]> GetOperatorPriceAsync(int dodtdcd);
        Task<PriceDto> GetPriceDtoByIdAsync(int id);
        Task<DeliveryObject> GetDeliveryObjectByPriceIdAsync(int priceId);
        Task UpdatePriceAsync(PriceDtoAddModel model);
        Task DeletePriceAsync(PriceDtoAddModel model);
        Task CreatePriceAsync(PriceDtoAddModel model);

        Task<IEnumerable<AutoPostOfficeDto>> GetAutoPostOfficesDtoAsync(int languageId);
       // Task<IEnumerable<AutoPostOffice>> GetAutoPostOfficesAsync();
        Task<AutoPostOfficeDto[]> GetAutoPostOfficeDtosAsync(int languageId, CultureInfo cultureInfo = null);
        Task<AutoPostOfficeDto> GetAutoPostOfficeDtoByIdAsync(int id, int languageId);
        Task<DeliveryObject> GetDeliveryObjectByAutoPostOfficeIdAsync(int autoPostOfficeId, int priceId);
        Task UpdateAutoPostOfficeAsync(AutoPostOfficeDtoModel model, int languageId);
        Task DeleteAutoPostOfficeAsync(AutoPostOfficeDtoModel model, int languageId);
        Task<DeliveryCityLanguage> GetDeliveryCityNameByIdAsync(int deliveryCityId);
        Task<DeliveryTypeLanguage> GetDeliveryTypeNameByIdAsync(int deliveryTypeId);
        Task<DeliveryOperatorLanguage> GetDeliveryOperatorNameByIdAsync(int deliveryOperatorTypeId);
        Task<AutoPostOfficeDto> GetAutoPostOfficesDtoById(int Id);
        Task<PriceDto> GetPriceDtoByIdDropdownAsync(int id, int languageId);
        Task<DeliveryType> GetDeliverySystemTypeNameByIdAsync(int deliveryTypeId);

        #endregion
    }
}
