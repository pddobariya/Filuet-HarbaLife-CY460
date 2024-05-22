using Nop.Web.Models.Customer;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models
{
    public record FiluetLoginModel : LoginModel
    {
        #region Properties

        public bool IsUnCompleted { get; set; }
        public bool CantBuy { get; set; }

        #endregion
    }
}