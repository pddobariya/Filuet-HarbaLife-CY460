using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.FusionProxyClientSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ProxyServiceReference;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    public class AuthorizationProxyService : ClientBase<IFusionServiceProxy>, IAuthorizationProxyService
    {
        #region Fields

        private readonly IHerbalifeEnvironment _herbalifeEnvironment;

        #endregion

        #region Methods

        public async Task<AuthorizationResultModel> AuthorizeUser(string userId, string pin, string countryCode)
        {

            AuthorizationResult proxyResult = await Channel.AuthorizeUserAsync(userId, pin, countryCode, countryCode,(await _herbalifeEnvironment?.GetEnvironmentCode()));
            
            return new AuthorizationResultModel
            {
                AccessToken = proxyResult.AccessToken,
                AuthorizationStatus = proxyResult.AuthorizationStatus.ToFiluetEnum(),                
                UserInformation = new UserInformationModel
                {
                    Id = proxyResult.UserInformation.Id,
                    Address = proxyResult.UserInformation.Addresses?[0].Address,
                    City = proxyResult.UserInformation.Addresses?[0].City,
                    Country = proxyResult.UserInformation.Addresses?[0].Country,
                    DistributorType = proxyResult.UserInformation.DistributorType.ToFiluetEnum(),
                    Email = proxyResult.UserInformation.Email,
                    FirstName = proxyResult.UserInformation.FirstName,
                    IsDebtor = proxyResult.UserInformation.IsDebtor,
                    IsDualMonthAllowed = proxyResult.UserInformation.IsDualMonthAllowed,
                    LastName = proxyResult.UserInformation.LastName,
                    MailingCountry = proxyResult.UserInformation.MailingCountry,
                    Phone = proxyResult.UserInformation.Phone,
                    Ppv = proxyResult.UserInformation.Ppv,
                    Pv = proxyResult.UserInformation.Pv,
                    ResidenceCountry = proxyResult.UserInformation.ResidenceCountry,
                    Tv = proxyResult.UserInformation.Tv,
                    ZipCode = proxyResult.UserInformation.Addresses?[0].ZipCode
                }
            };             
        }

        #endregion
    }
}
