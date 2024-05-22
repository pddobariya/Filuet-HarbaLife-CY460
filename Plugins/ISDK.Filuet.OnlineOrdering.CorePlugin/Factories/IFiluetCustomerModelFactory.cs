using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Customer;
using Nop.Core.Domain.Customers;
using Nop.Web.Factories;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    public interface IFiluetCustomerModelFactory : ICustomerModelFactory
    {
        #region Methods

        /// <summary>
        /// Prepare the extended customer info model
        /// </summary>
        /// <param name="model">Extended customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>Customer info model</returns>
        Task<FiluetCustomerInfoModel> PrepareFiluetCustomerInfoModelAsync(FiluetCustomerInfoModel model, Customer customer, bool excludeProperties, 
            string overrideCustomCustomerAttributesXml = "");

        #endregion
    }
}
