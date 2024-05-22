using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class DateTimeExtensions
    {
        #region Methods

        public static long ToUnixTimestamp(this DateTime dt)
        {
            long unixTimestamp = (long)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static DateTime FromUnixTimestamp(this long ut)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            start = start.AddSeconds(ut);
            return start;
        }

        #endregion
    }
}
