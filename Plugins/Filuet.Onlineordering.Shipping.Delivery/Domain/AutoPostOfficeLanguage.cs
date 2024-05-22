using Nop.Core;

namespace Filuet.Onlineordering.Shipping.Delivery.Domain
{
    public class AutoPostOfficeLanguage : BaseEntity
    {
        #region Properties

        public string Address { get; set; }
        public string Comment { get; set; }
        public int AutoPostOfficeId { get; set; }
        public int LanguageId { get; set; }

        #endregion
    }
}
