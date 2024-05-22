using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    public interface IFiluetOrderModelFactory
    {
        #region Methods

        Task<FiluetCustomerOrderListModel> PrepareCustomerOrderListModel(OrderPagingFilteringModel command);

        #endregion
    }
}