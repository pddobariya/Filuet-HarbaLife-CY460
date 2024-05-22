using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Authorization request result
    /// </summary>
    public class AuthorizationResultModel
    {
        #region Properties

        /// <summary>
        /// Authorization status
        /// </summary>
        public AuthorizationStatusEnum AuthorizationStatus { get; set; }

        /// <summary>
        /// User information.
        /// In case of successful authorization
        /// </summary>
        public UserInformationModel UserInformation { get; set; }

        /// <summary>
        /// Access token is used to authentificate user
        /// </summary>
        public string AccessToken { get; set; }

        #endregion
    }
}
