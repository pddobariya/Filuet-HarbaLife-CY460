using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion
{
    public interface IFusionPresets
    {
        #region Methods
        //Task<FusionServiceParams> GetFusionCallParams(Customer customer, Order order = null);
        Task<FusionServiceParams> GetFusionCallParamsAsync(Customer customer, Order order = null);
        //Task<FusionServiceParams> GetFusionCallParamsAsync(Customer customer, Order order = null);

        #endregion
    }
}
