using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface ISettingsLoader
    {
        #region Methods

        Task<int> GetHoursShift();
        Task<bool> IsDeptorEnabled();

        #endregion

    }
}
