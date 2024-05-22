using Nop.Core.Domain.Orders;
using Nop.Web.Areas.Admin.Models.Orders;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services
{
    public interface IFiluetAdminOrderButtons
    {
        #region Methods

        Task<bool> FirstButton(Order order, OrderModel model);

        #endregion
    }
}
