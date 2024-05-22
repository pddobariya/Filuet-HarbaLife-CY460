using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public partial interface IDeliveryOperatorService
    {
        #region Methods

        Task<DeliveryOperator> GetDeliveryObjectByPriceIdAsync(int id);
        Task<DeliveryOperatorLanguage[]> GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(int id);
        Task InsertDeliveryOperatorAsync(DeliveryOperator deliveryOperator);
        Task InsertDeliveryOperatorLanguage(DeliveryOperatorLanguage deliveryOperatorLanguage);
        Task InsertDeliveryOperatorLanguage(DeliveryOperatorLanguage[] deliveryOperatorLanguages);
        Task UpdateDeliveryOperator(DeliveryOperator deliveryOperator);
        Task UpdateDeliveryOperatorLanguage(DeliveryOperatorLanguage deliveryOperatorLanguage);

        #endregion
    }
}
