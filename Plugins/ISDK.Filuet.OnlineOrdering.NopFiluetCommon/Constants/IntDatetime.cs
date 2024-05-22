using System;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants
{
    public class IntDatetime
    {
        #region Properties

        public int Int { get; set; }

        public DateTime DateTime { get; set; }

        public override int GetHashCode()
        {
            return Int; 
        }

        public override bool Equals(object obj)
        {
            if (obj is IntDatetime st)
            {
                return st.Int == Int;
            }
            return base.Equals(obj);
        }

        #endregion
    }
}
