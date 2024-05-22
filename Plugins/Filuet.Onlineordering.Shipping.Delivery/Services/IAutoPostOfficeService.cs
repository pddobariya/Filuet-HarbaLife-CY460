using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public partial interface IAutoPostOfficeService
    {
        #region Methods

        Task<AutoPostOffice> GetAutoPostOfficeByIdAsync(int id);
        Task<AutoPostOffice[]> GetAutoPostOfficesByDeliveryOperator_DeliveryType_DeliveryCity_DependencyIdAsync(int id);
        Task<AutoPostOfficeLanguage[]> GetAutoPostOfficeLanguagesByIdAsync(int id);
        Task InsertAutoPostOfficeAsync(AutoPostOffice autoPostOffice);
        Task InsertAutoPostOfficeLanguageAsync(AutoPostOfficeLanguage autoPostOfficeLanguage);
        Task InsertAutoPostOfficeLanguageAsync(AutoPostOfficeLanguage[] autoPostOfficeLanguages);
        #endregion
    }
}
