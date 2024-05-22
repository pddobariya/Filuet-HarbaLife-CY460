using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.Services.Authentication;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    /// <summary>
    /// Extended Customer Registration Service class
    /// </summary>
    public class FiluetCustomerRegistrationService : CustomerRegistrationService, ICustomerRegistrationService
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILocalizationService _localizationService;
        private readonly IEventPublisher _eventPublisher;
        private readonly CustomerSettings _customerSettings;
        
        // All passwords shall be a combination of alpha, numeric, and special characters
        private const string passwordStrengthPatternFormat =
            @"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[<>(){{}}!@#$%^&_\+\-\=\*])[\w<>(){{}}!@#$%^&\+\-\=\*]{{{0},}}$";

        // All passwords shall be at least 8 characters long
        private const int minPasswordLength = 8;

        #endregion

        #region Ctor

        public FiluetCustomerRegistrationService(
            CustomerSettings customerSettings,
            IActionContextAccessor actionContextAccessor,
            IAuthenticationService authenticationService,
            ICustomerActivityService customerActivityService, 
            ICustomerService customerService,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher, 
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService, 
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            INotificationService notificationService,
            IPermissionService permissionService, 
            IRewardPointService rewardPointService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory, 
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            RewardPointsSettings rewardPointsSettings)
            : base(customerSettings,
                  actionContextAccessor,
                  authenticationService,
                  customerActivityService,
                  customerService,
                  encryptionService,
                  eventPublisher,
                  genericAttributeService,
                  localizationService, 
                  multiFactorAuthenticationPluginManager,
                  newsLetterSubscriptionService,
                  notificationService,
                  permissionService, 
                  rewardPointService,
                  shoppingCartService,
                  storeContext,
                  storeService,
                  urlHelperFactory,
                  workContext, 
                  workflowMessageService,
                  rewardPointsSettings)
        {
            _customerService = customerService;
            _encryptionService = encryptionService;
            _localizationService = localizationService;
            _eventPublisher = eventPublisher;
            _customerSettings = customerSettings;
        }

        #endregion

        #region Methods

        public override async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var result = new ChangePasswordResult();
            if (String.IsNullOrWhiteSpace(request.Email))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailIsNotProvided"));
                //return result;
            }
            if (String.IsNullOrWhiteSpace(request.NewPassword))
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordIsNotProvided"));
                //return result;
            }

            var customer = await _customerService.GetCustomerByEmailAsync(request.Email);
            if (customer == null)
            {
                result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.EmailNotFound"));
                //return result;
            }

            if (request.ValidateRequest)
            {
                //request isn't valid
                if (!PasswordsMatch(await _customerService.GetCurrentPasswordAsync(customer.Id), request.OldPassword))
                {
                    result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.OldPasswordDoesntMatch"));
                    //return result;
                }
            }

            //check for duplicates
            if (_customerSettings.UnduplicatedPasswordsNumber > 0)
            {
                //get some of previous passwords
                var previousPasswords = await _customerService.GetCustomerPasswordsAsync(customer.Id, passwordsToReturn: _customerSettings.UnduplicatedPasswordsNumber);

                var newPasswordMatchesWithPrevious = previousPasswords.Any(password => PasswordsMatch(password, request.NewPassword));
                if (newPasswordMatchesWithPrevious)
                {
                    result.AddError(await _localizationService.GetResourceAsync("Account.ChangePassword.Errors.PasswordMatchesWithPrevious"));
                    //return result;
                }
            }

            //check the min length
            if (request.NewPassword.Length < _customerSettings.PasswordMinLength)
            {
                result.AddError(string.Format(await _localizationService.GetResourceAsync("Account.ChangePassword.Fields.NewPassword.LengthValidation"), _customerSettings.PasswordMinLength));
                //return result;
            }

            if (!result.Success)
            {
                throw new Exception(string.Join(Environment.NewLine, result.Errors));
            }

            //at this point request is valid
            var customerPassword = new CustomerPassword
            {
                CustomerId = customer.Id,
                PasswordFormat = request.NewPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };

            switch (request.NewPasswordFormat)
            {
                case PasswordFormat.Clear:
                    customerPassword.Password = request.NewPassword;
                    break;
                case PasswordFormat.Encrypted:
                    customerPassword.Password = _encryptionService.EncryptText(request.NewPassword);
                    break;
                case PasswordFormat.Hashed:
                    {
                        var saltKey = _encryptionService.CreateSaltKey(5);
                        customerPassword.PasswordSalt = saltKey;
                        customerPassword.Password = _encryptionService.CreatePasswordHash(request.NewPassword, saltKey, _customerSettings.HashedPasswordFormat);
                    }
                    break;
            }

            await _customerService.InsertCustomerPasswordAsync(customerPassword);

            //publish event
            await _eventPublisher.PublishAsync(new CustomerPasswordChangedEvent(customerPassword));

            return result;
        }

        #endregion
    }
}
