using ISDK.Filuet.ExternalSSOAuthPlugin.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Services
{
    public class FiluetExternalAuthenticationService : ExternalAuthenticationService
    {
        #region Fileds

        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor
        public FiluetExternalAuthenticationService(
            CustomerSettings customerSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationPluginManager authenticationPluginManager,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService, 
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository, 
            IStoreContext storeContext, 
            IUrlHelperFactory urlHelperFactory,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings) 
            : base(customerSettings,
                  externalAuthenticationSettings,
                  actionContextAccessor,
                  authenticationPluginManager,
                  customerRegistrationService,
                  customerService,
                  eventPublisher,
                  genericAttributeService,
                  httpContextAccessor, 
                  localizationService,
                  externalAuthenticationRecordRepository, 
                  storeContext,
                  urlHelperFactory,
                  workContext, 
                  workflowMessageService,
                  localizationSettings)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> AuthenticateAsync(ExternalAuthenticationParameters parameters, string returnUrl = null)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            //get current logged-in user
            var currentLoggedInUser = await _workContext.GetCurrentCustomerAsync();

            if (!await _customerService.IsRegisteredAsync(currentLoggedInUser))
                currentLoggedInUser = null;

            //authenticate associated user if already exists
            var associatedUser = await GetUserByExternalAuthenticationParametersAsync(parameters);
            if (associatedUser != null)
                return await AuthenticateExistingUserAsync(associatedUser, currentLoggedInUser, returnUrl);
            currentLoggedInUser = await _customerService.GetCustomerByEmailAsync(parameters.Email);
            //or associate and authenticate new user
            return await AuthenticateNewUserAsync(currentLoggedInUser, parameters, returnUrl);
        }

        public override async Task<Customer> GetUserByExternalAuthenticationParametersAsync(ExternalAuthenticationParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var associationRecord = _externalAuthenticationRecordRepository.Table.FirstOrDefault(record =>
                (!string.IsNullOrWhiteSpace(parameters.Email) && record.Email.Equals(parameters.Email) || string.IsNullOrWhiteSpace(parameters.Email) && record.ExternalIdentifier.Equals(parameters.ExternalIdentifier) ) && record.ProviderSystemName.Equals(parameters.ProviderSystemName));
            if (associationRecord == null)
                return null;

            var isChanged = false;

            if (associationRecord.ExternalDisplayIdentifier != parameters.ExternalDisplayIdentifier)
            {
                associationRecord.ExternalDisplayIdentifier = parameters.ExternalDisplayIdentifier;
                isChanged = true;
            }
            if (associationRecord.ExternalIdentifier != parameters.ExternalIdentifier)
            {
                associationRecord.ExternalIdentifier = parameters.ExternalIdentifier;
                isChanged = true;
            }

            var customer = await _customerService.GetCustomerByIdAsync(associationRecord.CustomerId);
            if (isChanged)
            {
                await _externalAuthenticationRecordRepository.UpdateAsync(associationRecord);
                await _genericAttributeService.SaveAttributeAsync(customer, PluginNopStartup.UserUpdatedDate, DateTime.Now);
            }

            return customer;
        }

        #endregion
    }
}
