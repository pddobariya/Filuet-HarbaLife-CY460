using Nop.Core.Domain.Directory;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts
{
    public interface IWhitelistedCountryService
    {
        #region Methods

        Task<IEnumerable<Country>> GetWhitelistedCountries();

        void SaveWhitelistedCountries(string[] countryCodes);

        #endregion
    }
}
