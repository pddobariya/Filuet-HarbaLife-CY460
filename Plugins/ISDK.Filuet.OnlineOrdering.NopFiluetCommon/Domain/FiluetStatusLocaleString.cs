using Nop.Core;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain
{
    public class FiluetStatusLocaleString : BaseEntity
    {
        #region Properties

        public int StatusId { get; set; }

        public int LanguageId { get; set; }

        public string StatusName { get; set; }

        public string StatusComment { get; set; }

        #endregion
    }
}
