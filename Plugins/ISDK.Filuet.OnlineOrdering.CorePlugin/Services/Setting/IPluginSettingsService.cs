using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Setting
{
    public interface IPluginSettingsService
    {
        #region Methods
        void Install(Dictionary<string, string> pluginSettings);

        #endregion
    }
}
