using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK
{
    /// <summary>
    /// Authorization service
    /// </summary>    
    public interface IAuthorizationProxyService
    {
        #region Methods

        /// <summary>
        /// Authorizes user as well as returns user information
        /// </summary>
        /// <param name="userId">User (distributor) Id</param>
        /// <param name="pin">User (distributor) PIN</param>
        /// <param name="countryCode">User country code</param>
        /// <returns>Authorization result / user information</returns>
        Task<AuthorizationResultModel> AuthorizeUser(string userId, string pin, string countryCode);

        #endregion
    }
}
