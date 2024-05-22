using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Exceptions
{
    public class DistributorDetailedException : Exception
    {
        #region Ctor

        public DistributorDetailedException(string message) : base(message)
        {
        }

        #endregion
    }
}
