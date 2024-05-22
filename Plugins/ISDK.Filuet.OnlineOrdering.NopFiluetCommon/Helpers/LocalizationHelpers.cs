using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Infrastructure;
using System.Globalization;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public static class LocalizationHelpers
    {
        #region Methods

        public async static Task<CultureInfo> GetCurrentCultureDataAsync()
        {
            return CultureInfo.CreateSpecificCulture(await GetCurrentIso6391LocaleAsync());
        }

        public async static Task<string> GetCurrentCultureAsync()
        {
            IWorkContext workContext = EngineContext.Current.Resolve<IWorkContext>();
            string cult = (await workContext.GetWorkingLanguageAsync()).LanguageCulture;
            return cult;
        }

        public async static Task<string> GetCurrentIso6391LocaleAsync()
        {
            return (await GetCurrentCultureAsync()).CultureToIso6391();
        }

        #endregion
    }
}
