namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon
{
    public static class FiluetCoreDefault
    {
        #region Properties

        public static string ProviderSystemName => "ExternalSSOAuth.Herbalife";
        public static string LandingToken => "LandingToken";
        public static string SpCustomerRole => "SP";
        public static string IsFopPeriodActive => "IsFopPeriodActive";
        public static string AvailableFOPLimits => "AvailableFOPLimits";
        public static string AvailablePCLimits => "AvailablePCLimits";
        public static string IsNotResidentCustomerRole => "IsNotResident";
        public static string CompanyAttribute => "Company";
        public static string PhoneAttribute => "Phone";
        public static string ZipPostalCodeAttribute => "ZipPostalCode";
        public const string SocialFacebookLinkKeyTemplate = "social.facebook-{0}.url";
        public const string SocialYoutubeLinkKeyTemplate = "social.youtube-{0}.url";
        public const string SiteLinkKeyTemplate = "social.site-{0}.url";
        public static string StreetAddressAttribute => "StreetAddress";
        public const string VolumePoints = "cp_VolumePoints";
        public static string SelectedLimitsMonth => "SelectedLimitsMonth";

        public static string SelectedLimitsYear => "SelectedLimitsYear";
        public static string FirstNameAttribute => "FirstName";
        public static string LastNameAttribute => "LastName";

        #endregion
    }
}
