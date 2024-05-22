using System.Net;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public static class CommonHelper
    {
        #region Methods

        public static void SetCertificatePolicy()
        {   
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        }

        #endregion
    }
}
