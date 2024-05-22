using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon
{
    /// <summary>
    /// Submit order result
    /// </summary>
    public class SubmitOrderResultModel
    {
        #region Properties

        /// <summary>
        /// If order was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Errors during the order
        /// </summary>
        public IEnumerable<string> Errors { get; set; }

        #endregion
    }
}
