using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class StringExtensions
    {
        #region Methods

        public static bool IsNumber(this string text, bool? isPositive = null, bool integer = true)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            int result = 0;
            if(Int32.TryParse(text, out result))
            {
                if (isPositive.HasValue)
                {
                    if (isPositive.Value == true && result <= 0)
                    {
                        return false;
                    }
                    if (isPositive.Value == false && result >= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static string Truncate(this string text, int limitChars, bool appendSuffix, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text == null ? "" : text.Trim();
            }
            text = text.Trim();
            if(text.Length > limitChars)
            {
                return !appendSuffix ? text.Substring(0, limitChars - suffix.Length) + suffix : text.Substring(0, limitChars); 
            }
            return text;
        }

        public static string ToCapitalCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            input = input.Trim();
            if(input.Length < 2)
            {
                return input.ToUpper();
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string RemoveHtmlTags(this string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html;
            }

            return Regex.Replace(html, @"<[^>]*>", string.Empty);
        }

        public static string NormalizePhoneNumber(this string phoneNumber)
        {
            if (phoneNumber != null)
            {
                phoneNumber = phoneNumber.Replace("+", "").Replace("-", "").Replace("(", "").Replace(")", "").Trim();
                phoneNumber = Regex.Replace(phoneNumber, @"\s+", "");
            }
            return phoneNumber;
        }

        #endregion
    }
}
