using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public partial interface IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService
    {
        #region Methods

        Task<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(int id);
        
        #endregion
    }
}
