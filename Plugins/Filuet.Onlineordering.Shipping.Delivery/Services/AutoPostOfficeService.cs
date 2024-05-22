using Filuet.Onlineordering.Shipping.Delivery.Domain;
using LinqToDB;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class AutoPostOfficeService : IAutoPostOfficeService
    {
        #region Fields

        private readonly IRepository<AutoPostOffice> _autoPostOfficeRepository;
        private readonly IRepository<AutoPostOfficeLanguage> _autoPostOfficeLanguageRepository;

        #endregion

        #region Ctor
        public AutoPostOfficeService(
            IRepository<AutoPostOffice> autoPostOfficeRepository, 
            IRepository<AutoPostOfficeLanguage> autoPostOfficeLanguageRepository)
        {
            _autoPostOfficeRepository = autoPostOfficeRepository;
            _autoPostOfficeLanguageRepository = autoPostOfficeLanguageRepository;
        }

        #endregion

        #region Methods

        private async Task<IQueryable<AutoPostOffice>> GetAutoPostOffices() => await Task.FromResult(_autoPostOfficeRepository.Table.Where(x => !x.Blocked));

        public async Task<AutoPostOffice> GetAutoPostOfficeByIdAsync(int id)
        {
            return await (await GetAutoPostOffices()).FirstOrDefaultAsync(dcl => dcl.Id == id);
        }

        public async Task<AutoPostOffice[]> GetAutoPostOfficesByDeliveryOperator_DeliveryType_DeliveryCity_DependencyIdAsync(int id)
        {
            return await (await GetAutoPostOffices()).Where(apo => apo.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId == id).ToArrayAsync();
        }

        public async Task<AutoPostOfficeLanguage[]> GetAutoPostOfficeLanguagesByIdAsync(int id)
        {
            return  await Task.FromResult(_autoPostOfficeLanguageRepository.Table.Where(dcl => dcl.AutoPostOfficeId == id).ToArray());
        }

        public async Task InsertAutoPostOfficeAsync(AutoPostOffice autoPostOffice)
        {
            await _autoPostOfficeRepository.InsertAsync(autoPostOffice);
        }

        public async Task InsertAutoPostOfficeLanguageAsync(AutoPostOfficeLanguage autoPostOfficeLanguage)
        {
            await _autoPostOfficeLanguageRepository.InsertAsync(autoPostOfficeLanguage);
        }

        public async Task InsertAutoPostOfficeLanguageAsync(AutoPostOfficeLanguage[] autoPostOfficeLanguages)
        {
            await _autoPostOfficeLanguageRepository.InsertAsync(autoPostOfficeLanguages);
        }

        #endregion
    }
}
