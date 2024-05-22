using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface IDefaultShippingSettingsService
    {
        #region Methods

        Task<string> GetDefaultWareHouseAsync();

        #endregion
    }
}