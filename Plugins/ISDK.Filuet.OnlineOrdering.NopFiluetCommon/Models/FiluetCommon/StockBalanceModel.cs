using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Stock balance class
    /// </summary>
    public class StockBalanceModel
    {
        #region Properties

        /// <summary>
        /// The limits and blocks information
        /// </summary>
        public IEnumerable<StockBalanceItemModel> StockBalances { get; set; }

        public bool IsValid { get {
                if (StockBalances == null || !StockBalances.Any())
                {
                    return true;
                }

                return !StockBalances.Any(x => !x.IsAvailable || x.IsBlocked || !x.IsValid);
            }
        }

        #endregion
    }
}
