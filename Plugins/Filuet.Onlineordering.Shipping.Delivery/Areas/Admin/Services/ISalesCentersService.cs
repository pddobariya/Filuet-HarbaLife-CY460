using Filuet.Onlineordering.Shipping.Delivery.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Services
{
    public interface ISalesCentersService
    {
        #region Methods

        Task<IList<SalesCenterDto>> GetSalesCentersAsync(int id);
        Task<IList<DeliveryCityLanguage>> GetAllCityAsync();
        Task<IList<DeliveryOperatorLanguage>> GetAllOperatorAsync();
        Task<AutoPostOffice> GetAutoPostOfficeByIdAsync(int id);
        Task<List<int>> GetSystemTypesByLanguageIdAsync(int languageId);
        Task<DeliveryType> GetDeliveryTypeId(int deliveryTypeId);

        #endregion
    }
}
