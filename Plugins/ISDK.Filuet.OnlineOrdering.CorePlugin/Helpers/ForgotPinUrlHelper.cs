using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Utils
{
    /// <summary>
    /// Helper returns the forgot pin setting key by culture name
    /// </summary>
    public static class ForgotPinUrlHelper
    {
        private static Dictionary<string, string> SettingKeyDict = new Dictionary<string, string>()
        {
            { "et", NopFiluetCommonDefaults.ForgotPinEeUrlKey },
            { "lt", NopFiluetCommonDefaults.ForgotPinLtUrlKey },
            { "lv", NopFiluetCommonDefaults.ForgotPinLvUrlKey },
            { "en", NopFiluetCommonDefaults.ForgotPinEnUrlKey },
            { "ru", NopFiluetCommonDefaults.ForgotPinRuUrlKey }
        };

        /// <summary>
        /// Possible names are 'ee', 'lt', 'lv', 'en', 'ru'.
        /// Default name is 'en'.
        /// </summary>
        /// <param name="name">Culture name</param>
        /// <returns>
        /// The Forgot Pin setting key
        /// </returns>
        public static string GetSettingKeyByCultureName(string name)
        {
            if (!string.IsNullOrEmpty(name) && SettingKeyDict.ContainsKey(name))
            {
                return SettingKeyDict[name];
            }

            return NopFiluetCommonDefaults.ForgotPinEnUrlKey;
        }
    }
}
