using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public interface IPhoneFormatter
    {
        #region Methods
        Task<string> FormatAsync(string phone);
        Task<string> FormatPrefixAsync(string prefix, string countryCode);
       
        #endregion
    }
}
