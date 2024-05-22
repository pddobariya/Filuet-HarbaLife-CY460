using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Seo;
using Nop.Services.Topics;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Extensions
{
    public static class CustomExtensions
    {
        /// <summary>
        /// Get topic SEO name by system name
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="html">HTML helper</param>
        /// <param name="systemName">System name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the opic SEO Name
        /// </returns>
        public static async Task<string> GetTopicSeNameAsync(string systemName)
        {
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();
            var store = await storeContext.GetCurrentStoreAsync();
            var topicService = EngineContext.Current.Resolve<ITopicService>();
            var topic = await topicService.GetTopicBySystemNameAsync(systemName, store.Id);

            if (topic == null)
                return string.Empty;

            var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

            return await urlRecordService.GetSeNameAsync(topic);
        }
    }
}