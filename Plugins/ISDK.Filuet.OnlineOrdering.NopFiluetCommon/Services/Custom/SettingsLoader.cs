using Nop.Core;
using Nop.Services.Configuration;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class SettingsLoader : ISettingsLoader
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public SettingsLoader(ISettingService settingService, IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<int> GetHoursShift()
        {
            var settings =
                await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            return settings.HoursShift;
        }

        public async Task<bool> IsDeptorEnabled()
        {
            var settings =
                await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            return settings.IsDeptorEnabled;
        }

        #endregion
    }
}
