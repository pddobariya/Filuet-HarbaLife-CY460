using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface IHerbalifeEnvironment
    {
        #region Methods

        Task<string> GetEnvironmentCode();

        #endregion
    }
}