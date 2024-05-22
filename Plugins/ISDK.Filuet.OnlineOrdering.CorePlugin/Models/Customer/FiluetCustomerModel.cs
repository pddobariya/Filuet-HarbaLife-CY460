using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer
{
    public record FiluetCustomerModel : CustomerModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.ExternalIdentifier")]
        public string ExternalIdentifier { get; set; }

        public new string AvatarUrl { get; set; }

        #endregion
    }
}
