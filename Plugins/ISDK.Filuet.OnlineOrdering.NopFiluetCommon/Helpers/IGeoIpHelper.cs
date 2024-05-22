using Nop.Core.Domain.Customers;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public interface IGeoIpHelper
    {
        #region Methods

        Task<string> GetCountryByIpAddressAsync(Customer customer, string clientTimeZone = null);
        Task<bool> IsCountryGeoCodedAsync(Customer customer);
        Task<bool> IsShowShippingCountryPopupAsync(Customer customer);
        Task SaveGeoCodedCountryAsync(Customer customer, string countryCode);
        Task ResetGeoCodedCountryAsync(Customer customer);

        #endregion
    }
}
