using System;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models
{
    public static class Singletons
    {
        #region Properties

        public static class NoСache
        {
            private static long _tiks;
            private static readonly object threadlock = new object();

            public static long Tiks
            {
                get
                {
                    lock (threadlock)
                    {
                        if (_tiks == 0)
                            _tiks = DateTime.Now.Ticks;

                        return _tiks;
                    }
                }
            }
        }

        #endregion
    }
}
