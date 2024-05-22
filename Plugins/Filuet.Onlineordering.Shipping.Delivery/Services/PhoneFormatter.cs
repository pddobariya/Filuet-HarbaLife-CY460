using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class PhoneFormatter : IPhoneFormatter
    {
        #region Methods

        public async Task<string> FormatAsync(string phone)
        {
            return await Task.FromResult(phone?.Substring(5));
        }

        public async Task<string> FormatPrefixAsync(string prefix, string countryCode)
        {
            string tail = null;
            switch (countryCode)
            {
                case "LV":
                    tail = "1";
                    break;
                case "EE":
                    tail = "2";
                    break;
                case "LT":
                    tail = "0";
                    break;
            }
            return await Task.FromResult(prefix + tail);
        }

        #endregion
    }
}
