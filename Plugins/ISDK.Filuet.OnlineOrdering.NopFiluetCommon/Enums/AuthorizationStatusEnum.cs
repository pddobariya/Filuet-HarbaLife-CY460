﻿namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums
{
    /// <summary>
    /// Authorization status
    /// </summary>
    public enum AuthorizationStatusEnum
    {
        Success = 0,
        InvalidId = 1,
        InvalidPin = 2,
        Unknown = 3,
        NotCompleteAdvisory = 4,
        ForeignPurchaseRestriction = 5,
        CantBuy = 6,
    }
}
