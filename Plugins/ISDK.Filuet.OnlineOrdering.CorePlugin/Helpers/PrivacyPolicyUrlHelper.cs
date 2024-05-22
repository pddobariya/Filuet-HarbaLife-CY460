using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Utils
{
    /// <summary>
    /// Helper returns the privacy policy setting key by culture name
    /// </summary>
    public static class PrivacyPolicyUrlHelper
    {
        private static Dictionary<string, string> SettingKeyDict = new Dictionary<string, string>()
        {
            { "et", NopFiluetCommonDefaults.FooterPrivacyPolicyEeUrlKey },
            { "lt", NopFiluetCommonDefaults.FooterPrivacyPolicyLtUrlKey },
            { "lv", NopFiluetCommonDefaults.FooterPrivacyPolicyLvUrlKey },
            { "en", NopFiluetCommonDefaults.FooterPrivacyPolicyEnUrlKey },
            { "ru", NopFiluetCommonDefaults.FooterPrivacyPolicyRuUrlKey }
        };

        /// <summary>
        /// Possible names are 'ee', 'lt', 'lv', 'en', 'ru'.
        /// Default name is 'en'.
        /// </summary>
        /// <param name="name">Culture name</param>
        /// <returns>
        /// The privacy policy setting key
        /// </returns>
        public static string GetSettingKeyByCultureName(string name)
        {
            if (!string.IsNullOrEmpty(name) && SettingKeyDict.ContainsKey(name))
            {
                return SettingKeyDict[name];
            }

            return NopFiluetCommonDefaults.FooterPrivacyPolicyEnUrlKey;
        }
    }
}
