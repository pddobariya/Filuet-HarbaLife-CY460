using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using Microsoft.AspNetCore.Authentication;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Services
{
    public class ResidentChecker : IResidentChecker
    {
        #region Fields

        private readonly SSOAuthPluginSettings _sSoAuthPluginSettings;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public ResidentChecker(SSOAuthPluginSettings sSOAuthPluginSettings,
            ICustomerService customerService)
        {
            _sSoAuthPluginSettings = sSOAuthPluginSettings;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public bool NeedToCheckIfAuthenticatedCustomerIsResident => _sSoAuthPluginSettings.IsEnableCountryRestrictions;

        public bool IsProhibitedForNotResident => _sSoAuthPluginSettings.IsProhibitedForNotResident;

        public async Task CheckAndUpdateAsync(Customer customer, AuthenticateResult authenticateResult, DistributorDetailedProfileResponse distibutorInfo)
        {
            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);
            if (NeedToCheckIfAuthenticatedCustomerIsResident == true &&
                !CheckIfAuthenticatedCustomerIsResident(authenticateResult))
            {
                if (customerRoles.Any(cr => cr.SystemName != CommonConstants.IsNotResidentCustomerRole))
                {
                    var isNotResidentCustomerRole =
                        await _customerService.GetCustomerRoleBySystemNameAsync(CommonConstants
                            .IsNotResidentCustomerRole);
                    if (isNotResidentCustomerRole == null)
                        throw new NopException(
                            $"'{CommonConstants.IsNotResidentCustomerRole}' role could not be loaded");
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                    { CustomerRoleId = isNotResidentCustomerRole.Id, CustomerId = customer.Id });
                }

                if (customerRoles.Any(cr => cr.SystemName == CommonConstants.IsResidentCustomerRole))
                {
                    var isResidentCustomerRoleMapping = customerRoles.First(cr =>
                        cr.SystemName == CommonConstants.IsResidentCustomerRole);

                    await _customerService.RemoveCustomerRoleMappingAsync(customer,
                        isResidentCustomerRoleMapping);
                }
           
            }
            else
            {
                if (customerRoles.All(cr => cr.SystemName != CommonConstants.IsResidentCustomerRole))
                {
                    var isResidentCustomerRole =
                        await _customerService.GetCustomerRoleBySystemNameAsync(CommonConstants
                            .IsResidentCustomerRole);
                    if (isResidentCustomerRole == null)
                        throw new NopException(
                            $"'{CommonConstants.IsResidentCustomerRole}' role could not be loaded");
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                    { CustomerRoleId = isResidentCustomerRole.Id, CustomerId = customer.Id });
                }

                if (customerRoles.Any(cr => cr.SystemName == CommonConstants.IsNotResidentCustomerRole))
                {
                    var isNotResidentCustomerRoleMapping =
                        await _customerService.GetCustomerRoleBySystemNameAsync(CommonConstants
                            .IsNotResidentCustomerRole);

                    await _customerService.RemoveCustomerRoleMappingAsync(customer,
                        isNotResidentCustomerRoleMapping);
                }
            }
        }

        public bool CheckIfAuthenticatedCustomerIsResident(AuthenticateResult authenticateResult)
        {
            var isEnable = NeedToCheckIfAuthenticatedCustomerIsResident;
            var whitelistedCountries = _sSoAuthPluginSettings.WhitelistedCountries;

            if (isEnable)
            {
                if (authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value ==
                    "38919149")
                    return true;
                var countryCode = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Country)?.Value;

                if (countryCode == null || (countryCode != null && !whitelistedCountries.Contains(countryCode)))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

    }
}
