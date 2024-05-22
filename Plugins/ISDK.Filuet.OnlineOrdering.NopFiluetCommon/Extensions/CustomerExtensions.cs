using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Directory;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class CustomerExtensions
    {
        #region Methods

        /// <summary>
        /// Gets the customer's full name.
        /// </summary>
        /// <param name="customer">Customer.</param>
        /// <returns>The customer's full name.</returns>
        public async static Task<string> GetFullNameAsync(this Customer customer)
        {
            IGenericAttributeService genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            //var firstName = await customer.GetAttributeAsync<string>(NopFiluetCommonDefault.FirstNameAttribute);
            //var lastName = await customer.GetAttributeAsync<string>(NopFiluetCommonDefault.LastNameAttribute);
             var firstName = await genericAttributeService.GetAttributeAsync<string>(customer,NopFiluetCommonDefault.FirstNameAttribute);
            var lastName = await genericAttributeService.GetAttributeAsync<string>(customer,NopFiluetCommonDefault.LastNameAttribute);
           
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                return string.Format("{0} {1}", firstName, lastName);
            }

            if (!string.IsNullOrWhiteSpace(firstName))
            {
                return firstName;
            }

            return lastName;
        }

        public async static Task<string> GetShippingCountryCodeAsync(this Customer customer)
        {
            if (customer.ShippingAddressId is null)
                return null;
            IAddressService addressService;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                addressService = serviceScope.ServiceProvider.GetService<IAddressService>();
            }
            ICountryService countryService;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                countryService = serviceScope.ServiceProvider.GetService<ICountryService>();
            }

            var address =await addressService.GetAddressByIdAsync(customer.ShippingAddressId.Value);

            if (address is null || address.CountryId is null)
                return null;
            return (await countryService?.GetCountryByIdAsync(address.CountryId.Value)).TwoLetterIsoCode;
    }

        public async static Task<bool> IsCustomerFromBalticCountryAsync(this Customer customer)
        {
            var countryCode = await GetShippingCountryCodeAsync(customer);
            if (string.IsNullOrEmpty(countryCode))
            {
                return false;
            }
            else
            {
                var balticCountries = new string[] { Constants.CountryCodes.LV, Constants.CountryCodes.LT, Constants.CountryCodes.EE };
                return balticCountries.Contains(countryCode);
            }
        }

        public async static Task<string> GetDistributorIdAsync(this Customer customer)
        {
            IExternalAuthenticationService externalAuthenticationService = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                externalAuthenticationService = serviceScope.ServiceProvider.GetService<IExternalAuthenticationService>();
            }
            var authRecord = (await externalAuthenticationService?.GetCustomerExternalAuthenticationRecordsAsync(customer)).FirstOrDefault(p => p.ProviderSystemName == SSOAuthHerbalifeDefaults.ProviderSystemName);
            return authRecord?.ExternalIdentifier;
        }

        public async static Task<string> GetAccessTokenAsync(this Customer customer)
        {
            IExternalAuthenticationService externalAuthenticationService = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                externalAuthenticationService = serviceScope.ServiceProvider.GetService<IExternalAuthenticationService>();
            }
            var authRecord = (await externalAuthenticationService?.GetCustomerExternalAuthenticationRecordsAsync(customer)).FirstOrDefault(p => p.ProviderSystemName == SSOAuthHerbalifeDefaults.ProviderSystemName && p.CustomerId == customer.Id);
            return authRecord?.OAuthAccessToken;
        }

        public async static Task<bool> GetCantBuyFlagAsync(this Customer customer)
        {
#if LOCAL_DEBUG || TEST_SERVER
            return false;
#else
            var distributorService = EngineContext.Current.Resolve<IDistributorService>();
            var profile = await distributorService.GetDistributorDetailedProfileAsync(customer);
            if (profile == null)
            {
                return false;
            }
            return profile.Flags.CantBuy;
#endif
        }

        public async static Task<DateTime?> GetApfDueDateAsync(this Customer customer)
        {
            var distributorService = EngineContext.Current.Resolve<IDistributorService>();
            var profile = await distributorService.GetDistributorDetailedProfileAsync(customer);
            return profile?.ApfDueDate.Date/*.AddYears(-1)*/;
        }

        public async static Task<DistributorTypes> GetDistributorTypeAsync(this Customer customer)
        {
            var distributorService = EngineContext.Current.Resolve<IDistributorService>();
            var profile = await distributorService.GetDistributorDetailedProfileAsync(customer);
            return profile?.DistributorType ?? DistributorTypes.Unknown;
        }

        public static string GetCustomerCacheKey(this Customer customer, string cacheKey)
            => $"{cacheKey}|{customer.CustomerGuid}|{customer.Id}|{customer.Email}";

        #endregion
    }
}
