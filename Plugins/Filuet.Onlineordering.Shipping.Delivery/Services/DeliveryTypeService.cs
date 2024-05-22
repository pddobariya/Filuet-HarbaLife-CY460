using Filuet.Onlineordering.Shipping.Delivery.Domain;
using LinqToDB;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class DeliveryTypeService : IDeliveryTypeService
    {
        #region Fields

        private readonly IRepository<DeliveryType> _deliveryTypeRepository;
        private readonly IRepository<DeliveryTypeLanguage> _deliveryTypeLanguageRepository;

        #endregion

        #region Ctor

        public DeliveryTypeService(
            IRepository<DeliveryType> deliveryTypeRepository,
            IRepository<DeliveryTypeLanguage> deliveryTypeLanguageRepository)
        {
            _deliveryTypeRepository = deliveryTypeRepository;
            _deliveryTypeLanguageRepository = deliveryTypeLanguageRepository;
        }

        #endregion

        #region Methods

        public async Task<DeliveryType> GetDeliveryTypeByIdAsync(int id)
        {
            return await _deliveryTypeRepository.Table.FirstOrDefaultAsync(dt => dt.Id == id);
        }

        public async Task<DeliveryTypeLanguage[]> GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(int id)
        {
            return await _deliveryTypeLanguageRepository.Table.Where(dcl => dcl.DeliveryTypeId == id).ToArrayAsync();
        }

        public async Task InsertDeliveryTypeLanguageAsync(DeliveryTypeLanguage[] deliveryTypeLanguages)
        {
            await _deliveryTypeLanguageRepository.InsertAsync(deliveryTypeLanguages);
        }

        public async Task UpdateDeliveryTypeAsync(DeliveryType deliveryType)
        {
            await _deliveryTypeRepository.UpdateAsync(deliveryType);
        }

        public async Task UpdateDeliveryTypeLanguageAsync(DeliveryTypeLanguage deliveryTypeLanguage)
        {
            await _deliveryTypeLanguageRepository.UpdateAsync(deliveryTypeLanguage);
        }

        #endregion
    }
}
