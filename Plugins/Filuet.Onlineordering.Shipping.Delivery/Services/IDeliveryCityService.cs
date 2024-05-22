using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public partial interface IDeliveryCityService
    {
        #region Methods

        Task<DeliveryCityLanguage[]> GetDeliveryCityLanguagesByDeliveryCityIdAsync(int id);
        Task InsertDeliveryCityLanguageAsync(DeliveryCityLanguage[] deliveryCityLanguages);
        Task UpdateDeliveryCityLanguage(DeliveryCityLanguage deliveryCityLanguage);

        #endregion
    }
}
