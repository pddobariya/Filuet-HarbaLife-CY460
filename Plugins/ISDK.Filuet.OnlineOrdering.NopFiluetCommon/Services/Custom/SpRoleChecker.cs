using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class SpRoleChecker : ISpRoleChecker
    {
        #region Fields

        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public SpRoleChecker(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public async Task<bool> CheckAndUpdateAsync(Customer customer)
        {
            if (await customer.GetDistributorTypeAsync() == DistributorTypes.Supervisor /*|| customer.Username == "DINAK@HERBALIFE.COM"*/)
            {
                var spCustomerRole = await _customerService.GetCustomerRoleBySystemNameAsync(CustomerRoles.SpCustomerRole);
                if (spCustomerRole == null)
                    throw new NopException("'SpCustomerRole' role could not be loaded");
                if ((await _customerService.GetCustomerRolesAsync(customer)).All(ccr =>
                    ccr.Id != spCustomerRole.Id))
                {
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = spCustomerRole.Id,
                        CustomerId = customer.Id
                    });
                }
                return true;
            }
            else
            {
                var spCustomerRole = (await _customerService.GetCustomerRolesAsync(customer)).FirstOrDefault(cr =>
                    cr.SystemName == CustomerRoles.SpCustomerRole);
                if (spCustomerRole != null)
                {
                    await _customerService.DeleteCustomerRoleAsync(spCustomerRole);
                }

                return false;
            }
        }

        public async Task<bool> SetupAsync(Customer customer, DistributorTypes distributorTypes)
        {
            var roles = await _customerService.GetCustomerRolesAsync(customer);

            if (distributorTypes == DistributorTypes.Supervisor)
            {
                var spCustomerRole = await _customerService.GetCustomerRoleBySystemNameAsync(CommonConstants.SpCustomerRole);

                if (spCustomerRole == null)
                    throw new NopException("'SpCustomerRole' role could not be loaded");

                if (roles.All(cr => cr.Id != spCustomerRole.Id))
                {
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                    {
                        CustomerRoleId = spCustomerRole.Id,
                        CustomerId = customer.Id
                    });
                }

                return true;
            }
            else
            {
                var spCustomerRole = roles.FirstOrDefault(cr => cr.SystemName == CommonConstants.SpCustomerRole);

                if (spCustomerRole != null)
                    await _customerService.RemoveCustomerRoleMappingAsync(customer, spCustomerRole);

                return false;
            }
        }

        #endregion
    }
}
