using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Logging;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using System;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    public class FiluetCommonApiController : BaseController
    {
        #region Fields

        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public FiluetCommonApiController(
            IExternalAuthenticationService externalAuthenticationService,
            IGenericAttributeService genericAttributeService,
            ILogger logger)
        {
            _externalAuthenticationService = externalAuthenticationService;
            _genericAttributeService = genericAttributeService;
            _logger = logger;
        }

        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> VerifyAft(string landingToken, string dsId)
        {
            var user = await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                new ExternalAuthenticationParameters
                    {ExternalIdentifier = dsId, ProviderSystemName = SSOAuthHerbalifeDefaults.ProviderSystemName});
            return Ok(await VerifyAftImpl(landingToken, dsId) ? "SUCCESS" : "FAIL");
        }

        [HttpPost]
        public async Task<IActionResult> LogLanding(string landingToken, string dsId, LogLevel logLevel, string message)
        {
            try
            {
                var flag = await VerifyAftImpl(landingToken, dsId);
                if (!flag)
                    return Ok("FAIL");
                await _logger.InsertLogAsync(logLevel, message);
                return Ok("SUCCESS");
            }
            catch (Exception )
            {
                return Ok("FAIL");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DisposeAft(string landingToken, string dsId)
        {
            var flag = await VerifyAftImpl(landingToken, dsId);
            if (flag)
            {
                var user = await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                    new ExternalAuthenticationParameters
                        {ExternalIdentifier = dsId, ProviderSystemName = SSOAuthHerbalifeDefaults.ProviderSystemName});
                await _genericAttributeService.SaveAttributeAsync(user, CommonConstants.LandingToken, "");
            }
            return Ok(flag ? "SUCCESS" : "FAIL");
        }

        private async Task<bool> VerifyAftImpl(string landingToken, string dsId)
        {
            var user = await _externalAuthenticationService.GetUserByExternalAuthenticationParametersAsync(
                new ExternalAuthenticationParameters
                    {ExternalIdentifier = dsId, ProviderSystemName = SSOAuthHerbalifeDefaults.ProviderSystemName});
            var token =await _genericAttributeService.GetAttributeAsync<string>(user,CommonConstants.LandingToken);
            return !string.IsNullOrEmpty(token) && token == landingToken;
        }

        #endregion
    }
}