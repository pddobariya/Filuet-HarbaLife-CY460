using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public interface IDeliveryTypeService
    {
        #region Methods

        Task<DeliveryType> GetDeliveryTypeByIdAsync(int id);
        Task<DeliveryTypeLanguage[]> GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(int id);
        Task InsertDeliveryTypeLanguageAsync(DeliveryTypeLanguage[] deliveryTypeLanguages);
        Task UpdateDeliveryTypeAsync(DeliveryType deliveryType);
        Task UpdateDeliveryTypeLanguageAsync(DeliveryTypeLanguage deliveryTypeLanguage);

        #endregion
    }
}
