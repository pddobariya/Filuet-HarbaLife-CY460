namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    /// <summary>
    /// Represents token which is sent to a mobile client
    /// </summary>
    public class MobileTokenModel
    {
        #region Properties

        public int CustomerId { get; set; }

        public string Token { get; set; }

        #endregion
    }
}
