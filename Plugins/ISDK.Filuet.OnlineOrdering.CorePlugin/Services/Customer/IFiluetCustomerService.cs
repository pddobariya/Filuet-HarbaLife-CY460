using Nop.Services.Customers;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public interface IFiluetCustomerService : ICustomerService
    {
        #region Methods

        void AddStreetAddress(string streetAddress);
        void AddPhone(string phone);

        #endregion
    }
}
