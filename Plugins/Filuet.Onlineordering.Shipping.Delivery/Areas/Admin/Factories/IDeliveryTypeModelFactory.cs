using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Factories
{
    public interface IDeliveryTypeModelFactory 
    {
        #region Methods

        Task<DeliveryTypeDtoListModel> PrepareDeliveryTypesListModelAsync(DeliveryTypeDtoSearchModel searchModel);
        Task<DeliveryOperatorDtoListModel> PrepareDeliveryOperatorListModelAsync(DeliveryOperatorDtoSearchModel searchModel);
        Task<DeliveryOperatorsCityListModel> PrepareDeliveryCityRegionListModelAsync(DeliveryOperatorsCitySearchModel searchModel);
        Task<PriceDtoListModel> PreparePriceListModelAsync(PriceDtoSearchModel searchModel);
        Task<AutoPostOfficeDtoListModel> PrepareAutoPostOfficeListModelAsync(AutoPostOfficeDtoSearchModel searchModel);
        Task<PriceDtoAddModel> PreparePriceDtoModelAsync(PriceDtoAddModel model,int languageId, bool excludeProperties = false);
        Task<AutoPostOfficeDtoModel> PrepareAutoPostOfficeDtoModelAsync(AutoPostOfficeDtoModel model, int languageId);
        Task<AutoPostOfficeDtoModel> PrepareAutoPostOfficeEditDtoModelAsync(AutoPostOfficeDto autopostOfficeDto, int languageId, bool excludeProperties = false);
        Task<PriceDtoAddModel> PreparePriceEditDtoModelAsync(PriceDto priceDto, int languageId, bool excludeProperties = false);
        Task<DeliveryOperatorsCityModel> PrepareDeliveryOperatorsCityModelAsync(DeliveryOperatorsCityModel model,
            DeliveryOperatorsCityDto deliveryOperatorsCityDto, bool excludeProperties = false);

        Task<DeliveryOperatorDtoModel> PrepareDeliveryOperatorDtoModelAsync(DeliveryOperatorDtoModel model,
            DeliveryOperatorDto deliveryOperatorDto, bool excludeProperties = false);

        #endregion
    }
}
