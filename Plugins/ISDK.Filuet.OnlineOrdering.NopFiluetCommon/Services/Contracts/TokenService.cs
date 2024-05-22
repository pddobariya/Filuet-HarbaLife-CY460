using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Authentication;
using Nop.Services.Customers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Contracts
{
    public class TokenService : ITokenService
    {
        #region Filed 

        private readonly IRepository<RefreshToken> _tokenRepository;
        private readonly Nop.Services.Orders.IShoppingCartService _shoppingCartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;

        #endregion

        #region Ctor

        public TokenService(
            IRepository<RefreshToken> tokenRepository,
            Nop.Services.Orders.IShoppingCartService shoppingCartService,
            IAuthenticationService authenticationService,
            IWorkContext workContext, ICustomerService customerService)
        {
            _tokenRepository = tokenRepository;
            _shoppingCartService = shoppingCartService;
            _authenticationService = authenticationService;
            _workContext = workContext;
            _customerService = customerService;
        }

        #endregion

        #region Methods 

        public async Task<RefreshToken> GetTokenAsync(int tokenId)
        {
            return await Task.FromResult(_tokenRepository.GetById(tokenId));
        }

        public async Task<int> SetTokenAsync(Customer customer, string token)
        {
            int expiresInMinutes = await GetTokenSessionTimeoutMinutesAsync(customer);
            var refreshToken = _tokenRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id && x.Token == token);
            if (refreshToken == null)
            {
                var now = DateTime.UtcNow;
                refreshToken = new RefreshToken()
                {
                    Customer = customer,
                    Token = token,
                    CreatedOnUtc = now,
                    ExpiredOnUtc = DateTime.UtcNow.AddMinutes(expiresInMinutes)
                };
                _tokenRepository.Insert(refreshToken);
            }
            else
            {
                refreshToken.ExpiredOnUtc = DateTime.UtcNow.AddMinutes(expiresInMinutes);
                _tokenRepository.Update(refreshToken);
            }

            return refreshToken.Id;
        }

        public async Task<string> EncodeMobileTokenAsync(Customer customer, string token)
        {
            MobileTokenModel mobileToken = new MobileTokenModel()
            {
                CustomerId = customer.Id,
                Token = token
            };

            return await Task.FromResult(JsonConvert.SerializeObject(mobileToken));

            // UNDONE AES256 CBC encoding of the return json
        }

        /// <summary>
        /// Tries to decode a mobile token
        /// </summary>
        /// <param name="encodedToken">Encoded mobile token</param>
        /// <returns>Decoded token</returns>
        public async Task<MobileTokenModel> DecodeMobileTokenAsync(string encodedToken)
        {
            try
            {
                return await Task.FromResult(JsonConvert.DeserializeObject<MobileTokenModel>(encodedToken));
            }
            catch
            {
                return null;
            }
        }

        public async Task<ValidateTokenResults> ValidateTokenAsync(int customerId, string token)
        {
            try
            {
                RefreshToken refreshToken = _tokenRepository.Table.FirstOrDefault(x => (customerId == 0 || x.CustomerId == customerId) && x.Token == token);

                ValidateTokenResults result = ValidateTokenResults.Valid;
                if (refreshToken == null)
                {
                    result = ValidateTokenResults.InValid;
                }
                if (refreshToken.ExpiredOnUtc < DateTime.UtcNow)
                {
                    result = ValidateTokenResults.Expired;
                }

                return await Task.FromResult(result);
            }
            catch
            {
                return ValidateTokenResults.InValid;
            }
        }

        public async Task<int> GetTokenSessionTimeoutMinutesAsync(Customer customer)
        {
            int expiresInHours = await customer.GetDistributorIdAsync() == "38341099" ? 1 : 120; //TODO make configurable via plugin settings
            return expiresInHours;
        }

        private async Task<Customer> GetCustomerByTokenAsync(string token)
        {
            try
            {
                RefreshToken refreshToken = _tokenRepository.Table.FirstOrDefault(x => x.Token == token);

                if (refreshToken == null)
                {
                    //try parse as mobile token
                    MobileTokenModel mobileToken = await DecodeMobileTokenAsync(token);
                    refreshToken = _tokenRepository.Table.FirstOrDefault(x => x.Token == mobileToken.Token);
                }

                if (refreshToken != null)
                {
                    return await _customerService.GetCustomerByIdAsync(refreshToken.CustomerId);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task AuthenticateCustomerByTokenAsync(string token)
        {
            Customer customer = await GetCustomerByTokenAsync(token);
            if (customer != null)
            {

                //migrate shopping cart
                await _shoppingCartService.MigrateShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), customer, true);

                //sign in new customer
                await _authenticationService.SignInAsync(customer, false);
                await _workContext.SetCurrentCustomerAsync(customer);
            }
        }

        #endregion
    }
}
