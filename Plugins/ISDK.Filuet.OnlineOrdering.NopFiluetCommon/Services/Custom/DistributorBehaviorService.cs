namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class DistributorBehaviorService : IDistributorBehaviorService
    {
        #region Methods

        public bool GetVpLimitsFromOracle(string countryCode)
        {
            switch (countryCode)
            {
                case "LV":
                    return true;
                case "KV":
                    return true;
                default:
                    return true;
            }
        }

        #endregion
    }
}
