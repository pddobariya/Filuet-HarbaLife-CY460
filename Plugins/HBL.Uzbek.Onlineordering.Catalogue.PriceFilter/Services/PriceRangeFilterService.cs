using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain;
using Nop.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services
{
    public class PriceRangeFilterService : IPriceRangeFilterService
    {
        #region Fields

        private readonly IRepository<PriceRange> _repository;

        #endregion

        #region Ctor

        public PriceRangeFilterService(IRepository<PriceRange> repository)
        {
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<IList<PriceRange>> GetPriceRangesAsync()
        {
            return await _repository.GetAllAsync(query =>
            {
                return from s in query orderby s.Id select s;
            }, includeDeleted: false);
        }

        public async Task CreatePriceRangeAsync(PriceRange model)
        {

            await _repository.InsertAsync(model);
        }

        public async Task UpdatePriceRangeAsync(PriceRange model)
        {
            await _repository.UpdateAsync(model);
        }

        public async Task DeletePriceRangeAsync(PriceRange model)
        {
            await _repository.DeleteAsync(model);
        }

        public async Task<PriceRange> GetPriceRangeByIdAsync(int priceFilterId)
        {
            return await _repository.GetByIdAsync(priceFilterId, default, false);
        }

        #endregion
    }
}
