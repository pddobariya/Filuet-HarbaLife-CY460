using Nop.Core.Domain.Orders;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface IPromotionWoker
    {
        #region Methods

        Task RunAsync(Order order);
        string[] PromotionNames { get; }

        #endregion
    }
}
