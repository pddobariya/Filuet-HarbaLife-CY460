using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public partial interface ISalesCenterService
    {
        #region Methods

        Task<SalesCenterLanguage[]> GetSalesCenterLanguagesBySalesCenterIdAsync(int id);
        Task InsertSalesCenterLanguageAsync(SalesCenterLanguage[] salesCenterLanguages);
        Task UpdateSalesCenterLanguageAsync(SalesCenterLanguage SalesCenterLanguage);
        Task UpdateSalesCenterAsync(SalesCenter salesCenter);

        #endregion
    }
}
