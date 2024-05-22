using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Factories
{
    public interface ISalesCenterModelFactory
    {
        #region Methods

        Task<SalesCentersListModel> PrepareSalesCentersListModelAsync(SalesCenterSearchModel searchModel);

        Task<SalesCenterDtoModel> PrepareSalesCenterDtoModelAsync(SalesCenterDtoModel model,
            SalesCenterDto salesCenterDto, bool excludeProperties = false);

        #endregion

    }
}
