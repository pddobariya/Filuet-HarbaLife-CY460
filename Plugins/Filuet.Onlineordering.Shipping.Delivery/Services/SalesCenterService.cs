using Filuet.Onlineordering.Shipping.Delivery.Domain;
using LinqToDB;
using Nop.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class SalesCenterService : ISalesCenterService
    {
        #region Fields

        private readonly IRepository<SalesCenter> _salesCenterRepository;
        private readonly IRepository<SalesCenterLanguage> _salesCenterLanguageRepository;

        #endregion

        #region Ctor
        public SalesCenterService(
            IRepository<SalesCenter> salesCenterRepository,
            IRepository<SalesCenterLanguage> salesCenterLanguageRepository)
        {
            _salesCenterRepository = salesCenterRepository;
            _salesCenterLanguageRepository = salesCenterLanguageRepository;
        }
        #endregion

        #region Methods

        public async Task<SalesCenterLanguage[]> GetSalesCenterLanguagesBySalesCenterIdAsync(int id)
        {
            return await _salesCenterLanguageRepository.Table.Where(dcl => dcl.SalesCenterId == id).ToArrayAsync();
        }

        public async Task InsertSalesCenterLanguageAsync(SalesCenterLanguage[] salesCenterLanguages)
        {
             await _salesCenterLanguageRepository.InsertAsync(salesCenterLanguages);
        }

        public async Task UpdateSalesCenterAsync(SalesCenter salesCenter)
        {
            await _salesCenterRepository.UpdateAsync(salesCenter);
        }

        public async Task UpdateSalesCenterLanguageAsync(SalesCenterLanguage salesCenterLanguage)
        {
           await _salesCenterLanguageRepository.UpdateAsync(salesCenterLanguage);
        }

        #endregion
    }
}
