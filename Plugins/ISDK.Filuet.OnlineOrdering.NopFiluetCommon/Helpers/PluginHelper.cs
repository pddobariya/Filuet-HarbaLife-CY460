using Newtonsoft.Json;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public class PluginHelper
    {
        #region Methods

        public async static Task<bool> EnsurePluginInstalledAsync(string systemName)
        {
           
            var pluginService = EngineContext.Current.Resolve<IPluginService>();
            var logger = EngineContext.Current.Resolve<ILogger>();
            var descriptor =await pluginService.GetPluginDescriptorBySystemNameAsync<IPlugin>(systemName, LoadPluginsMode.All);
            if (descriptor == null)
            {
                logger.InsertLog(LogLevel.Error, "Can't install plugin.",
                    $"Required '{systemName}' plugin not installed.");

                return false;
            }

            INopFileProvider _fileProvider = EngineContext.Current.Resolve<INopFileProvider>();
            var filePath = _fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
            var text = await _fileProvider.ReadAllTextAsync(filePath, Encoding.UTF8);
            var pluginsInfo = JsonConvert.DeserializeObject<PluginsInfo>(text);
            var installedPluginSystemNames = pluginsInfo.InstalledPlugins.Select(p => p.SystemName).ToList();
            if (!installedPluginSystemNames.Contains(systemName))
            {
                //descriptor.Instance().Install();
                await descriptor.Instance<IPlugin>().InstallAsync();
            };

            return true;
        }

        #endregion
    }
}
