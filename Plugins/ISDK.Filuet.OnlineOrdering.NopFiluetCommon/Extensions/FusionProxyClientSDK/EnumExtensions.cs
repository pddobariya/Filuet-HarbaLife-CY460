using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using System;
using AuthorizationStatusEnum = ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.AuthorizationStatusEnum;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.FusionProxyClientSDK
{
    public static class EnumExtensions
    {
        #region Methods

        public static AuthorizationStatusEnum ToFiluetEnum(this ProxyServiceReference.AuthorizationStatusEnum value)
        {
            var result = AuthorizationStatusEnum.Unknown;
            if (Enum.TryParse<AuthorizationStatusEnum>(value.ToString(), out var parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        public static DistributorTypeEnum ToFiluetEnum(this ProxyServiceReference.DistributorType value)
        {
            var result = DistributorTypeEnum.Unknown;
            if (Enum.TryParse<DistributorTypeEnum>(value.ToString(), out var parsedValue))
            {
                result = parsedValue;
            }

            return result;
        }

        #endregion
    }
}
