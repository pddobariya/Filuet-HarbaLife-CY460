using Nop.Services.Configuration;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class HerbalifeEnvironment : IHerbalifeEnvironment
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public HerbalifeEnvironment(ISettingService settingService)
        {
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public async Task<string> GetEnvironmentCode()
        {
            return ( await _settingService.LoadSettingAsync<FiluetCorePluginSettings>()).HerbalifeEnvironment;
        }

        #endregion
    }
}
