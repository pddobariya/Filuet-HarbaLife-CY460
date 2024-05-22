using ISDK.Filuet.ExternalSSOAuthPlugin.Enum;
using ISDK.Filuet.ExternalSSOAuthPlugin.Models;
using ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Data;
using Nop.Services.Authentication.External;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Controllers
{
    public class SSOAuthController : BasePluginController
    {
        #region Fields

        private  SSOAuthPluginSettings _sSOAuthPluginSettings;
        private readonly ILogger _logger;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<ExternalAuthenticationRecord> _externalAuthenticationRecordRepository;
        private readonly IWorkContext _workContext;
        private readonly ISpRoleChecker _spRoleChecker;
        private readonly IDistributorService _distributorService;
        private readonly IPluginService _pluginService;
        private readonly ICrmDataProviderAdapter _crmDataProviderAdapter;
        private readonly IDistributorRestrictionService _distributorRestrictionService;
        private readonly FiluetCorePluginSettings _filuetCorePluginSettings;
        private readonly IDistributorBehaviorService _distributorBehaviorService;
        private readonly IResidentChecker _residentChecker;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly INotificationService _notificationService;
        private string _authErrorViewPath => $"~/Plugins/ISDK.Filuet.ExternalSSOAuthPlugin/Views/AuthError.cshtml";

        #endregion

        #region Ctor

        public SSOAuthController(
            SSOAuthPluginSettings sSOAuthPluginSettings,
            ILogger logger,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            IHttpContextAccessor httpContextAccessor,
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IWorkContext workContext,
            ISpRoleChecker spRoleChecker,
            IDistributorService distributorService,
            IPluginService pluginService,
            ICrmDataProviderAdapter crmDataProviderAdapter,
            IDistributorRestrictionService distributorRestrictionService,
            FiluetCorePluginSettings filuetCorePluginSettings,
            IDistributorBehaviorService distributorBehaviorService,
            IResidentChecker residentChecker,
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            INotificationService notificationService)
        {
            _sSOAuthPluginSettings = sSOAuthPluginSettings;
            _logger = logger;
            _externalAuthenticationService = externalAuthenticationService;
            _localizationService = localizationService;
            _httpContextAccessor = httpContextAccessor;
            _externalAuthenticationRecordRepository = externalAuthenticationRecordRepository;
            _workContext = workContext;
            _spRoleChecker = spRoleChecker;
            _distributorService = distributorService;
            _pluginService = pluginService;
            _crmDataProviderAdapter = crmDataProviderAdapter;
            _distributorRestrictionService = distributorRestrictionService;
            _filuetCorePluginSettings = filuetCorePluginSettings;
            _distributorBehaviorService = distributorBehaviorService;
            _residentChecker = residentChecker;
            _customerService = customerService;
            _customerRegistrationService = customerRegistrationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Login(string returnUrl)
        {
            if (await _pluginService.GetPluginDescriptorBySystemNameAsync<IExternalAuthenticationMethod>(SSOAuthHerbalifeDefaults.ProviderSystemName) == null)
                throw new NopException("SSO authentication module cannot be loaded");

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "SSOAuth", new { returnUrl = returnUrl })
            };
            return Challenge(authenticationProperties, SSOAuthHerbalifeDefaults.AuthenticationScheme);
        }

        public IActionResult CountryRestrictions()
        {
            ClearCookies();
            return View();
        }

        public IActionResult APFRestrictions()
        {
            ClearCookies();
            return View();
        }

        public IActionResult DistributorRestrictions()
        {
            ClearCookies();


            return View();
        }

        public async Task<IActionResult> LoginCallback1(string returnUrl)
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(SSOAuthHerbalifeDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                await _logger.WarningAsync("LoginCallback Auth result FAIL");
                return RedirectToRoute("Login");
            }

            var tokenAsync = await HttpContext.GetTokenAsync(SSOAuthHerbalifeDefaults.AuthenticationScheme, "access_token");

            if (string.IsNullOrEmpty(tokenAsync))
            {
                await _logger.WarningAsync("LoginCallback Auth result FAIL");
                return RedirectToRoute("Login");
            }

            var distributorInfo = await _distributorService.GetDistributorDetailedProfileSSOAsync(tokenAsync);

            if (distributorInfo == null)
            {
                await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallback GetDistributorInfo is null");
                return RedirectToRoute("Login");
            }

            await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallback GetDistributorInfo: MemberId = {distributorInfo.MemberId} and Email = {distributorInfo.Email}", JsonConvert.SerializeObject(distributorInfo));

            var checkRestrictions =  _distributorRestrictionService.CheckDistributorRestrictionAsync(distributorInfo);

            if (checkRestrictions != DsRestriction.None)
            {
                await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} has distributor restriction",
                    $"Restriction current customer is {checkRestrictions}");
                TempData[checkRestrictions.ToString()] = true;
                return Redirect("DistributorRestrictions");
            }

            if (_sSOAuthPluginSettings.DenyNoResident)
            {
                var whitelistCountries = _sSOAuthPluginSettings.WhitelistedCountries.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (!whitelistCountries.Any(countryCode => string.Equals(distributorInfo.ResidenceCountryCode,
                        countryCode, StringComparison.CurrentCultureIgnoreCase)))
                {
                    await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} has country restriction");

                    return Redirect("CountryRestrictions");
                }
            }

            if (_sSOAuthPluginSettings.DenyEntryToUnpaidAPF && distributorInfo.ApfDueDate.ToUniversalTime().Date < DateTime.UtcNow.Date)
            {
                await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} has APF restriction");

                return RedirectToRoute("APFRestrictions");
            }

            var email = distributorInfo.Email;

            var identity = (ClaimsIdentity)authenticateResult.Principal.Identity;
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, distributorInfo?.MemberId));
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
            identity.AddClaim(new Claim(ClaimTypes.Name, email));
            identity.AddClaim(new Claim(ClaimTypes.Country, distributorInfo.ResidenceCountryCode));

            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = SSOAuthHerbalifeDefaults.ProviderSystemName,
                AccessToken = tokenAsync,
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            try
            {
                var associatedUser =
                    await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                        authenticationParameters);
                if (associatedUser != null)
                {
                    UpdateAccessToken(associatedUser, authenticationParameters.AccessToken);
                }

                IActionResult result = null;
                result = await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
                var currentCustomer = associatedUser ?? await _workContext.GetCurrentCustomerAsync();

                if (_residentChecker?.NeedToCheckIfAuthenticatedCustomerIsResident == true &&
                   !_residentChecker.CheckIfAuthenticatedCustomerIsResident(authenticateResult) &&
                   _residentChecker.IsProhibitedForNotResident)
                {
                    var err = _localizationService.GetResourceAsync("Plugins.ExternalAuth.SSO.NotAuthorized");
                    const string urlAuthErrorPage = "/SSOAuth/AuthError";

                    await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} resident cheker",
                        "Error resident checker", currentCustomer);

                    return Redirect($"{urlAuthErrorPage}?error_msg={err}");
                }

                await _residentChecker.CheckAndUpdateAsync(currentCustomer, authenticateResult, distributorInfo);

                if (_sSOAuthPluginSettings.UpdateCountryDisRole)
                    await _distributorService.SetDistributorRegionalRole(currentCustomer, distributorInfo);


                bool spRole = await _spRoleChecker.SetupAsync(currentCustomer, distributorInfo.DistributorType);

                if (spRole)
                {
                    await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallBack MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} sp Role",
                        "Set SP Role for current user, if not", currentCustomer);
                }
                else
                {
                    await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallBack MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} sp Role",
                        "Remove SP Role for current user, if is", currentCustomer);
                }

                _crmDataProviderAdapter.InvalidateDistributorInfo(currentCustomer);

                var getVpLimitsFromOracle = _distributorBehaviorService.GetVpLimitsFromOracle(_filuetCorePluginSettings.CountryCode);

                if (getVpLimitsFromOracle)
                {
                    var distributorFopLimitsModel = await _distributorService.PutDistributorVpLimits(currentCustomer, distributorInfo.MemberId, distributorInfo.ProcessingCountryCode);

                    if (distributorFopLimitsModel == null)
                        await _logger.InsertLogAsync(LogLevel.Error, "LoginCallback put vpLimits", "Oracle did not send vpLimits");
                }

                await _distributorService.SaveCustomerDataAsync(currentCustomer, distributorInfo);

                await _distributorService.PutAddressesFromOracleAsync(currentCustomer, distributorInfo);

                return result;
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync("[SSOAuthController] LoginCallback() Error", exc);
                return RedirectToRoute("Login");
            }
        }


        public async Task<IActionResult> LoginCallback1(string returnUrl)
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(SSOAuthHerbalifeDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                await _logger.WarningAsync("LoginCallback Auth result FAIL");
                return RedirectToRoute("Login");
            }

            var tokenAsync = await HttpContext.GetTokenAsync(SSOAuthHerbalifeDefaults.AuthenticationScheme, "access_token");

            if (string.IsNullOrEmpty(tokenAsync))
            {
                await _logger.WarningAsync("LoginCallback Auth result FAIL");
                return RedirectToRoute("Login");
            }

            var distributorInfo = await _distributorService.GetDistributorDetailedProfileSSOAsync(tokenAsync);

            if (distributorInfo == null)
            {
                await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallback GetDistributorInfo is null");
                return RedirectToRoute("Login");
            }

            await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallback GetDistributorInfo: MemberId = {distributorInfo.MemberId} and Email = {distributorInfo.Email}", JsonConvert.SerializeObject(distributorInfo));

            var checkRestrictions =  _distributorRestrictionService.CheckDistributorRestrictionAsync(distributorInfo);

            if (checkRestrictions != DsRestriction.None)
            {
                await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} has distributor restriction",
                    $"Restriction current customer is {checkRestrictions}");
                TempData[checkRestrictions.ToString()] = true;
                return RedirectToRoute("DistributorRestrictions");
            }

            if (_sSOAuthPluginSettings.DenyNoResident)
            {
                var whitelistCountries = _sSOAuthPluginSettings.WhitelistedCountries.Split(",", StringSplitOptions.RemoveEmptyEntries);
                if (!whitelistCountries.Any(countryCode => string.Equals(distributorInfo.ResidenceCountryCode,
                        countryCode, StringComparison.CurrentCultureIgnoreCase)))
                {
                    await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} has country restriction");

                    return RedirectToRoute("CountryRestrictions");
                }
            }

            if (_sSOAuthPluginSettings.DenyEntryToUnpaidAPF && distributorInfo.ApfDueDate.ToUniversalTime().Date < DateTime.UtcNow.Date)
            {
                await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} has APF restriction");

                return RedirectToRoute("APFRestrictions");
            }

            var email = distributorInfo.Email;

            var identity = (ClaimsIdentity)authenticateResult.Principal.Identity;
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, distributorInfo?.MemberId));
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
            identity.AddClaim(new Claim(ClaimTypes.Name, email));
            identity.AddClaim(new Claim(ClaimTypes.Country, distributorInfo.ResidenceCountryCode));

            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = SSOAuthHerbalifeDefaults.ProviderSystemName,
                AccessToken = tokenAsync,
                Email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value,
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };

            try
            {
                var associatedUser =
                    await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                        authenticationParameters);
                if (associatedUser != null)
                {
                    UpdateAccessToken(associatedUser, authenticationParameters.AccessToken);
                }

                IActionResult result = null;
                
                var currentCustomer = associatedUser ?? await _workContext.GetCurrentCustomerAsync();

                if (_residentChecker?.NeedToCheckIfAuthenticatedCustomerIsResident == true &&
                   !_residentChecker.CheckIfAuthenticatedCustomerIsResident(authenticateResult) &&
                   _residentChecker.IsProhibitedForNotResident)
                {
                    var err = _localizationService.GetResourceAsync("Plugins.ExternalAuth.SSO.NotAuthorized");
                    const string urlAuthErrorPage = "/SSOAuth/AuthError";

                    await _logger.InsertLogAsync(LogLevel.Warning, $"LoginCallback MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} resident cheker",
                        "Error resident checker", currentCustomer);

                    return Redirect($"{urlAuthErrorPage}?error_msg={err}");
                }

                await _residentChecker.CheckAndUpdateAsync(currentCustomer, authenticateResult, distributorInfo);

                if (_sSOAuthPluginSettings.UpdateCountryDisRole)
                    await _distributorService.SetDistributorRegionalRole(currentCustomer, distributorInfo);


                bool spRole = await _spRoleChecker.SetupAsync(currentCustomer, distributorInfo.DistributorType);

                if (spRole)
                {
                    await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallBack MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} sp Role",
                        "Set SP Role for current user, if not", currentCustomer);
                }
                else
                {
                    await _logger.InsertLogAsync(LogLevel.Information, $"LoginCallBack MemberId = {distributorInfo.MemberId} and {distributorInfo.Email} sp Role",
                        "Remove SP Role for current user, if is", currentCustomer);
                }

                _crmDataProviderAdapter.InvalidateDistributorInfo(currentCustomer);

                var getVpLimitsFromOracle = _distributorBehaviorService.GetVpLimitsFromOracle(_filuetCorePluginSettings.CountryCode);

                if (getVpLimitsFromOracle)
                {
                    var distributorFopLimitsModel = await _distributorService.PutDistributorVpLimits(currentCustomer, distributorInfo.MemberId, distributorInfo.ProcessingCountryCode);

                    if (distributorFopLimitsModel == null)
                        await _logger.InsertLogAsync(LogLevel.Error, "LoginCallback put vpLimits", "Oracle did not send vpLimits");
                }

                await _distributorService.SaveCustomerDataAsync(currentCustomer, distributorInfo);


                await _distributorService.PutAddressesFromOracleAsync(currentCustomer, distributorInfo);
                result = await _externalAuthenticationService.AuthenticateAsync(authenticationParameters, returnUrl);
                return result;
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync("[SSOAuthController] LoginCallback() Error", exc);
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToRoute("Login");
            }
        }
        public ActionResult AuthError(BaseErrorModel model = null)
        {
            if (model == null)
            {
                model = new BaseErrorModel(false, null, "");
            }
            return View(_authErrorViewPath, model);
        }

        private void ClearCookies()
        {
            foreach (var cookie in _httpContextAccessor.HttpContext.Request.Cookies.Keys)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie);
            }
        }
        private async void UpdateAccessToken(Customer associatedUser, string accessToken)
        {

            var externalAuthenticationRecord =(await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(associatedUser)).First();
            externalAuthenticationRecord.OAuthAccessToken = accessToken;
            await _externalAuthenticationRecordRepository.UpdateAsync(externalAuthenticationRecord);
        }
        #endregion
    }
}