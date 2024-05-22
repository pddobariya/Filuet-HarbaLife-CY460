using Filuet.Onlineordering.Shipping.Delivery.Domain;
using LinqToDB;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class DeliveryCityService : IDeliveryCityService
    {
        #region Fileds

        private readonly IRepository<DeliveryCityLanguage> _deliveryCityLanguageRepository;

        #endregion

        #region Ctor

        public DeliveryCityService(IRepository<DeliveryCityLanguage> deliveryCityLanguageRepository)
        {
            _deliveryCityLanguageRepository = deliveryCityLanguageRepository;
        }

        #endregion

        #region Methods

        public async Task<DeliveryCityLanguage[]> GetDeliveryCityLanguagesByDeliveryCityIdAsync(int id)
        {
            return await _deliveryCityLanguageRepository.Table.Where(dcl => dcl.DeliveryCityId == id).ToArrayAsync();
        }

        public async Task InsertDeliveryCityLanguageAsync(DeliveryCityLanguage[] deliveryCityLanguages)
        {
            await _deliveryCityLanguageRepository.InsertAsync(deliveryCityLanguages);
        }

        public async Task UpdateDeliveryCityLanguage(DeliveryCityLanguage deliveryCityLanguage)
        {
            await _deliveryCityLanguageRepository.UpdateAsync(deliveryCityLanguage);

        }
        #endregion
    }
}
