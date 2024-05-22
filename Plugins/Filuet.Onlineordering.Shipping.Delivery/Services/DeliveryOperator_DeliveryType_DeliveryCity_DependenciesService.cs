using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class DeliveryOperator_DeliveryType_DeliveryCity_DependenciesService : IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService
    {
        #region Fileds

        private readonly IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;

        #endregion

        #region Ctor

        public DeliveryOperator_DeliveryType_DeliveryCity_DependenciesService(IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository)
        {
            _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository = deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;
        }

        #endregion

        #region Methods

        public async Task<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(int id)
        {
            return await _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table
                .FirstOrDefaultAsync(dodtdcd => dodtdcd.Id == id);
        }

        #endregion

    }
}
