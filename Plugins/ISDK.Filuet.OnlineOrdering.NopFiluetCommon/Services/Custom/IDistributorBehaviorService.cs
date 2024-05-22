namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public interface IDistributorBehaviorService
    {
        #region Methods

        bool GetVpLimitsFromOracle(string countryCode);

        #endregion
    }
}
