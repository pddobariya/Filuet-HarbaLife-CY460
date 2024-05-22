using Nop.Core.Caching;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants
{
    /// <summary>
    /// Constants
    /// </summary>
    public static class NopFiluetCommonDefaults
    {
        #region separate SKU

        /// <summary>
        /// Using to separate SKU
        /// </summary>
        public const string SkuSeparator = "|";

        public const string FusionFailedOrderNotesMessage = "FUSION SUBMISSION FAILED";

        public const string FusionSuccessOrderNotesMessage = "FUSION SUBMISSION SUCCESS";

        public const string EmptyDisplayPlaceholder = "N/A";

        /// <summary>
        /// Key for category picture caching
        /// </summary>
        /// <remarks>
        /// {0} : order id
        /// {1} : picture size
        /// {2} : value indicating whether a default picture is displayed in case if no real picture exists
        /// {3} : language ID ("alt" and "title" can depend on localized category name)
        /// {4} : is connection SSL secured?
        /// {5} : current store ID
        /// </remarks>
        public static CacheKey OrderPictureModelKey => new CacheKey("Nop.isdk.order.picture-{0}-{1}-{2}-{3}-{4}-{5}", OrderPicturePrefixCacheKey, OrderPicturePrefixCacheKeyById);
        public static string OrderPicturePrefixCacheKey => "Nop.isdk.order.picture";
        public static string OrderPicturePrefixCacheKeyById => "Nop.isdk.order.picture-{0}-";

        public const string DistributorDetailedProfile = "NOP_FILUET_COMMON_DISTRIBUTOR_DETAILED_PROFILE";

        #endregion

        #region Custom properties of models

        /// <summary>
        /// Volume Points
        /// </summary>
        public const string VolumePoints = "cp_VolumePoints";

        /// <summary>
        /// Base Price
        /// </summary>
        /// <remarks>En: Base for calculating prices / Ru: База для расчета цены</remarks>
        public const string BasePrice = "cp_BasePrice";

        /// <summary>
        /// Base Renumeration
        /// </summary>
        /// <remarks>En: Base for calculating renumerations / Ru: База для расчета вознаграждения</remarks>
        public const string BaseRenumeration = "cp_BaseRenumeration";

        /// <summary>
        /// Retail Price
        /// </summary>
        /// <remarks>En: Recommended retail price: / Ru: Рекомендованная розничная цена</remarks>
        public const string RetailPrice = "cp_RetailPrice";

        /// <summary>
        /// Is Debtor
        /// </summary>
        public const string IsDebtor = "cp_IsDebtor";

        /// <summary>
        /// Topic Url
        /// </summary>
        /// <remarks>To override url in footer</remarks>
        public const string TopicUrl = "cp_TopicUrl";

        /// <summary>
        /// Hide All Product Review
        /// </summary>
        public const string ShowProductReview = "ShowProductReview";

        #endregion

        #region Settings

        /// <summary>
        /// Supervisor APF Product SKU Key
        /// </summary>1
        public const string SupervisorApfSkuKey = "product.sp_apfsku";

        /// <summary>
        /// Distributor APF Product SKU Key
        /// </summary>1
        public const string DistributorApfSkuKey = "product.ds_apfsku";

        /// <summary>
        /// Supervisor APF Product SKU
        /// </summary>1
        public const string SupervisorApfSkuDefaultValue = "0909";

        /// <summary>
        /// Distributor APF Product SKU
        /// </summary>1
        public const string DistributorApfSkuDefaultValue = "9909";

        /// <summary>
        /// Forgot Pin EE Url
        /// </summary>1
        public const string ForgotPinEeUrlKey = "userlogin.ee_forgotpinurl";

        /// <summary>
        /// Forgot Pin LV Url
        /// </summary>1
        public const string ForgotPinLvUrlKey = "userlogin.lv_forgotpinurl";

        /// <summary>
        /// Forgot Pin LT Url
        /// </summary>1
        public const string ForgotPinLtUrlKey = "userlogin.lt_forgotpinurl";

        /// <summary>
        /// Forgot Pin RU Url
        /// </summary>1
        public const string ForgotPinRuUrlKey = "userlogin.ru_forgotpinurl";

        /// <summary>
        /// Forgot Pin EN Url
        /// </summary>1
        public const string ForgotPinEnUrlKey = "userlogin.en_forgotpinurl";


        public const string OAuthAuthTokenUrlKey = "sso.authtokenurl";

        public const string OAuthClientIdKey = "sso.clientid";

        public const string OAuthClientSecretKey = "sso.clientsecret";

        public const string OAuthResponseTypeKey = "sso.responsetype";

        public const string OAuthScopeKey = "sso.scope";

        public const string OAuthRedirectUriKey = "sso.redirecturi";

        public const string OAuthAccessTokenUrlKey = "sso.accesstokenurl";

        public const string OAuthGrantTypeKey = "sso.granttype";

        public const string ApiDistributorProfileUrlKey = "api.distributorprofileurl";

        public const string ApiDistributorDetailedProfileUrlKey = "api.distributordetailedprofileurl";

        public const string ApiDistributorVolumesUrlKey = "api.distributorvolumesurl";

        /// <summary>
        /// Hash Url
        /// </summary>1
        public const string UrlDefaultValue = "#";

        /// <summary>
        /// Footer Privacy Policy EE Url
        /// </summary>1
        public const string FooterPrivacyPolicyEeUrlKey = "footer.ee_privacypolicyurl";

        /// <summary>
        /// Footer Privacy Policy LV Url
        /// </summary>1
        public const string FooterPrivacyPolicyLvUrlKey = "footer.lv_privacypolicyurl";

        /// <summary>
        /// Footer Privacy Policy LT Url
        /// </summary>1
        public const string FooterPrivacyPolicyLtUrlKey = "footer.lt_privacypolicyurl";

        /// <summary>
        /// Footer Privacy Policy RU Url
        /// </summary>1
        public const string FooterPrivacyPolicyRuUrlKey = "footer.ru_privacypolicyurl";

        /// <summary>
        /// Footer Privacy Policy EN Url
        /// </summary>1
        public const string FooterPrivacyPolicyEnUrlKey = "footer.en_privacypolicyurl";

        /// <summary>
        /// Template for the Facebook localized page.
        /// </summary>
        public const string SocialFacebookLinkKeyTemplate = "social.facebook-{0}.url";

        /// <summary>
        /// Template for the Youtube localized channel.
        /// </summary>
        public const string SocialYoutubeLinkKeyTemplate = "social.youtube-{0}.url";

        /// <summary>
        /// Template for the localized site.
        /// </summary>
        public const string SiteLinkKeyTemplate = "social.site-{0}.url";
        #endregion

        #region Session keys

        public const string SessionTokenKey = "token";

        public const string SessionOAuthStateKey = "oauthstate";

        #endregion

        #region CacheKeys

        public const string FiluetFusionShippingComputationOptionCacheKey =
            "FiluetFusionShippingComputationOptionCacheKey";

        public const string FiluetFusionShippingComputationOptionCustomerDataCacheKey =
            "FiluetFusionShippingComputationOptionCustomerDataCacheKey-{0}";

        public const string DeliverySalesCentersByLanguageCacheKey =
            "DeliverySalesCentersByLanguageCacheKey-{0}";

        public const string DeliveryTypesByLanguageCacheKey =
            "DeliveryTypesByLanguageCacheKey-{0}";

        public const string DeliveryOperatorsCitiesByLanguageCacheKey =
            "DeliveryOperatorsCitiesByLanguageCacheKey-{0}";

        public const string DeliveryOperatorsByLanguageCacheKey =
            "DeliveryOperatorsByLanguageCacheKey-{0}";

        public const string AutoPostOfficesByLanguageCacheKey =
            "AutoPostOfficesByLanguageCacheKey-{0}";

        #endregion
    }
}