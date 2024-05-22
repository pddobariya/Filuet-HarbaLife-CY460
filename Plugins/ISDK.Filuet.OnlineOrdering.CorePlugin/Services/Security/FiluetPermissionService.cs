using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Security
{
    public class FiluetPermissionService : PermissionService
    {
        #region Fields

        private readonly ISpRoleChecker _spRoleChecker;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IHttpContextAccessor _contextAccessor;

        #endregion

        #region Ctor

        public FiluetPermissionService(
            ICustomerService customerService,
            ILocalizationService localizationService,
            IRepository<PermissionRecord> permissionRecordRepository,
            IRepository<PermissionRecordCustomerRoleMapping> permissionRecordCustomerRoleMappingRepository,
            IStaticCacheManager staticCacheManager,
            IWorkContext workContext,
            IGenericAttributeService genericAttributeService,
            ISpRoleChecker spRoleChecker,
            IHttpContextAccessor contextAccessor)
            : base(customerService,
                  localizationService,
                  permissionRecordRepository,
                  permissionRecordCustomerRoleMappingRepository,
                  staticCacheManager, 
                  workContext)
        {
            _genericAttributeService = genericAttributeService;
            _spRoleChecker = spRoleChecker;
            _contextAccessor = contextAccessor;
            _customerService = customerService;
        }

        #endregion

        #region Methods

        public override async Task InstallPermissionsAsync(IPermissionProvider permissionProvider)
        {
            var ovveridePermissionProvider = (IPermissionProvider)Activator.CreateInstance(typeof(FiluetStandardPermissionProvider));
            await base.InstallPermissionsAsync(ovveridePermissionProvider);
        }

        public override async Task<bool> AuthorizeAsync(PermissionRecord permission, Customer customer)
        {
            var contextAccessorHttpContext = _contextAccessor.HttpContext;
            var residentChecker = contextAccessorHttpContext.RequestServices.GetService<IResidentChecker>();
            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);

            if (residentChecker?.IsProhibitedForNotResident == true && customerRoles != null &&
                customerRoles.Any(ccr => ccr.SystemName == CommonConstants.IsNotResidentCustomerRole))
                return false;
            var flag = await base.AuthorizeAsync(permission, customer);
            if (!flag)
                return false;
            if (!string.IsNullOrWhiteSpace(customer.Email) && !await _genericAttributeService.GetAttributeAsync<bool>(customer,CoreGenericAttributes.SpRoleCheckedAttribute))
            {
                await _spRoleChecker.CheckAndUpdateAsync(customer);
                await _genericAttributeService.SaveAttributeAsync<bool>(customer, CoreGenericAttributes.SpRoleCheckedAttribute, true);
            }
            return true;
        }

        #endregion
    }
}