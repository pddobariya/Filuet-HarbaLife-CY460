using Nop.Services.Plugins;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts
{
    public interface IFusionShippingProvider : IPlugin
    {
        #region Properties

       Task<string> FreightCode { get; }

        #endregion

    }
}
