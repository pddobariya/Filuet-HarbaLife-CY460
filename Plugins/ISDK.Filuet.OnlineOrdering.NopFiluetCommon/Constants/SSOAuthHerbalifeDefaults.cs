namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants
{
    public static class SSOAuthHerbalifeDefaults
    {
        #region Properties
        public static string AuthenticationScheme => "Herbalife";

        public static string ProviderSystemName => "ExternalSSOAuth.Herbalife";

        public const string ViewComponentName = "ExternalSSOAuth";

        public static string CallbackPath => "/SSOAuth/LoginCallback";

        #endregion
    }
}