using Filuet.Onlineordering.Shipping.Delivery.Domain;
using LinqToDB;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class DeliveryOperatorService : IDeliveryOperatorService
    {
        #region Fields

        private readonly IRepository<DeliveryOperator> _deliveryOperatorRepository;
        private readonly IRepository<DeliveryOperatorLanguage> _deliveryOperatorLanguageRepository;

        #endregion

        #region Ctor

        public DeliveryOperatorService(
            IRepository<DeliveryOperator> deliveryOperatorRepository, 
            IRepository<DeliveryOperatorLanguage> deliveryOperatorLanguageRepository)
        {
            _deliveryOperatorRepository = deliveryOperatorRepository;
            _deliveryOperatorLanguageRepository = deliveryOperatorLanguageRepository;
        }

        #endregion

        #region Methods

        public async Task<DeliveryOperator> GetDeliveryObjectByPriceIdAsync(int id)
        {
            return await _deliveryOperatorRepository.Table.FirstOrDefaultAsync(@do => @do.Id == id);
        }

        public async Task<DeliveryOperatorLanguage[]> GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(int id)
        {
            return await  _deliveryOperatorLanguageRepository.Table.Where(dcl => dcl.DeliveryOperatorId == id).ToArrayAsync();
        }

        public async Task InsertDeliveryOperatorAsync(DeliveryOperator deliveryOperator)
        {
            await _deliveryOperatorRepository.InsertAsync(deliveryOperator);
        }

        public async Task InsertDeliveryOperatorLanguage(DeliveryOperatorLanguage deliveryOperatorLanguage)
        {
            await _deliveryOperatorLanguageRepository.InsertAsync(deliveryOperatorLanguage);
        }

        public async Task InsertDeliveryOperatorLanguage(DeliveryOperatorLanguage[] deliveryOperatorLanguages)
        {
            await _deliveryOperatorLanguageRepository.InsertAsync(deliveryOperatorLanguages);
        }

        public async Task UpdateDeliveryOperator(DeliveryOperator deliveryOperator)
        {
            await _deliveryOperatorRepository.UpdateAsync(deliveryOperator);
        }

        public async Task UpdateDeliveryOperatorLanguage(DeliveryOperatorLanguage deliveryOperatorLanguage)
        {
              await _deliveryOperatorLanguageRepository.UpdateAsync(deliveryOperatorLanguage);
        }

        #endregion
    }
}
