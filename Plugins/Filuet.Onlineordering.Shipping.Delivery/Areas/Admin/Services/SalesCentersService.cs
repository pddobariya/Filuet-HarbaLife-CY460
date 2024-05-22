using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Microsoft.EntityFrameworkCore;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Services
{
    public class SalesCentersService : ISalesCentersService
    {
        #region Fileds

        private readonly IRepository<SalesCenterDto> _repository;
        private readonly IRepository<DeliveryCityLanguage> _AllCityLanguagerepository;
        private readonly IRepository<DeliveryOperatorLanguage> _allOpperatorLanguagerepository;
        private readonly IRepository<DeliveryTypeLanguage> _allTypeLanguagerepository;
        private readonly IRepository<AutoPostOffice> _autoPostOffice;
        private readonly IRepository<DeliveryType> _deliveryTypeRepository;
        private readonly IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;

        #endregion

        #region Ctor

        public SalesCentersService(
            IRepository<SalesCenterDto> repository,
            IRepository<DeliveryCityLanguage> allCityLanguagerepository,
            IRepository<DeliveryOperatorLanguage> allOpperatorLanguagerepository,
            IRepository<DeliveryTypeLanguage> allTypeLanguagerepository,
            IRepository<AutoPostOffice> autoPostOffice,
            IRepository<DeliveryType> deliveryTypeRepository,
            IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository)
        {
            _repository = repository;
            _AllCityLanguagerepository = allCityLanguagerepository;
            _allOpperatorLanguagerepository = allOpperatorLanguagerepository;
            _allTypeLanguagerepository = allTypeLanguagerepository;
            _autoPostOffice = autoPostOffice;
            _deliveryTypeRepository = deliveryTypeRepository;
            _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository = deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;
        }

        #endregion

        #region Methods

        public async Task<IList<SalesCenterDto>> GetSalesCentersAsync(int id)
        {
            return await _repository.GetAllAsync(query =>
            {
                return from s in query orderby s.Id select s;
            }, CacheKey => default, includeDeleted: false);
        }

        public virtual async Task<IList<DeliveryCityLanguage>> GetAllCityAsync()
        {
            return await _AllCityLanguagerepository.GetAllAsync(query =>
            {
                return from s in query orderby  s.CityName select s;
            }, includeDeleted: false);
        }

        public virtual async Task<IList<DeliveryOperatorLanguage>> GetAllOperatorAsync()
        {
            return await _allOpperatorLanguagerepository.GetAllAsync(query =>
            {
                return (from s in query orderby s.Id,s.OperatorName select s).Distinct();
            }, CacheKey => default, includeDeleted: false);
        }

        public async Task<List<int>> GetSystemTypesByLanguageIdAsync(int languageId)
        {
            var query = (from dt in _deliveryTypeRepository.Table
                        where _allTypeLanguagerepository.Table
                            .Where(dtl => dtl.LanguageId == languageId)
                            .Select(dtl => dtl.DeliveryTypeId)
                            .Contains(dt.Id)
                        select dt.Id).Distinct();

            var result = await query.ToListAsync();
            return result;
        }

        public virtual async Task<DeliveryType> GetDeliveryTypeId(int deliveryTypeId)
        {
            var deliveryType = _deliveryTypeRepository.Table
                .Where(dt => dt.Id == deliveryTypeId).FirstOrDefault();
                return await Task.FromResult(deliveryType);
        }

        public virtual async Task<AutoPostOffice> GetAutoPostOfficeByIdAsync(int id)
        {
            return await _autoPostOffice.GetByIdAsync(id, cache => default);
        }

        #endregion

    }
}
