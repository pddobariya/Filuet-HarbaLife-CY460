using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISDK.Filuet.ExternalSSOAuthPlugin.Models;
using ISDK.Filuet.ExternalSSOAuthPlugin.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Directory;

namespace ISDK.Filuet.ExternalSSOAuthPlugin.Controllers
{
    [ApiController]
    [Route("countries")]
    public class WhitelistedCountryApiController : ControllerBase
    {
        #region Fields

        private readonly IWhitelistedCountryService _whitelistedCountryService;
        private readonly ICountryService _countryService;

        #endregion

        #region Ctor

        public WhitelistedCountryApiController(
            IWhitelistedCountryService whitelistedCountryService,
            ICountryService countryService)
        {
            _whitelistedCountryService = whitelistedCountryService;
            _countryService = countryService;
        }

        #endregion

        #region Methods

        [HttpGet]
        [Route("whitelisted")]
        public async Task<ActionResult<IEnumerable<FiluetCountryModel>>> GetWhitelistedCountries()
        {
            return (await _whitelistedCountryService.GetWhitelistedCountries())
                 .Select(c => new FiluetCountryModel
                 {
                     Id = c.Id,
                     Name = c.Name,
                     ImageUrl = string.Empty,//TODO
                     IsoCode = c.TwoLetterIsoCode
                     
                 }).ToList();
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<FiluetCountryModel>>> GetCountries(string nameOrCode = null)
        {
            var result = (await _countryService.GetAllCountriesAsync())
                .Select(c => new FiluetCountryModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = string.Empty,//TODOm
                    IsoCode = c.TwoLetterIsoCode
                });

            return result.ToList();
        }
        #endregion
    }
}
