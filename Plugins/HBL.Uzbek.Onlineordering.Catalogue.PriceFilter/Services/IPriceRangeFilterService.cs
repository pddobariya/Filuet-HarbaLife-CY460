using HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HBL.Uzbek.Onlineordering.Catalogue.PriceFilter.Services
{
    public interface IPriceRangeFilterService 
    {
        #region Methods

        Task<IList<PriceRange>> GetPriceRangesAsync();
        Task CreatePriceRangeAsync(PriceRange model);
        Task UpdatePriceRangeAsync(PriceRange model);
        Task<PriceRange> GetPriceRangeByIdAsync(int priceFilterId);

        #endregion

    }
}
