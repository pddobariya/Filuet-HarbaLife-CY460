using Nop.Core.Domain.Localization;
using System.Globalization;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class LanguageExtensions
    {
        #region Methods

        public static string ToISO6391(this Language nopLang)
        {
            string en = "en";
            if(nopLang == null || string.IsNullOrWhiteSpace(nopLang.LanguageCulture))
            {
                return "en";
            }
            string lang = new CultureInfo(nopLang.LanguageCulture).TwoLetterISOLanguageName;

            return lang ?? en;
        }

        #endregion
    }
}
