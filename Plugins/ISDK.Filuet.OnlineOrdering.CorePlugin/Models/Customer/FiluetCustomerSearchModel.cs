using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer
{
    public record FiluetCustomerSearchModel : CustomerSearchModel
    {
        #region Properties

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.ExternalIdentifier")]
        public string SearchExternalIdentifier { get; set; }

        #endregion
    }
}
