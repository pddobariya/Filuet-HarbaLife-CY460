using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping
{
    public interface IFiluetShippingService
    {
        #region Methods

        //add for 4.4 
        Task<FiluetFusionShippingComputationOption> GetShippingComputationOptionByCustomerIdAsync(int customerId);

        //add for 4.4 
        Task<FiluetFusionShippingComputationOption> GetShippingComputationOptionByIdAsync(int id);

        Task<List<FiluetFusionShippingComputationOption>> GetAllShippingComputationOptionsAsync();

        Task<List<FiluetFusionShippingComputationOption>> GetShippingComputationOptionsByContriesAsync(string[] contries = null);

        Task<List<FiluetFusionShippingComputationOptionCustomerData>> GetShippingComputationOptionCustomerDataListByCustomerIdAsync(int customerId);

        Task<FiluetFusionShippingComputationOptionCustomerData> AddShippingComputationOptionCustomerDataAsync(FiluetFusionShippingComputationOptionCustomerData filuetFusionShippingComputationOptionCustomerData);

        Task<FiluetFusionShippingComputationOptionCustomerData> UpdateShippingComputationOptionCustomerDataaAsync(FiluetFusionShippingComputationOptionCustomerData filuetFusionShippingComputationOptionCustomerData);

        Task<FiluetFusionShippingComputationOptionCustomerData> GetShippingComputationOptionCustomerDataByCustomerIdAndOptionIdAsync(int customerId, int optionId);

        Task<FiluetFusionShippingComputationOptionCustomerData> GetSelectedShippingComputationOptionCustomerDataAsync(int customerId);

        Task<FiluetFusionShippingComputationOptionModel> GetSelectedShippingComputationOptionModelAsync(Customer customer, Order order = null);

        Task<string> GetFreeShippingSkuAsync(Customer customer);

        Task<List<string>> GetFreeShippingSkusAsync();
        Task<string> GetWareHouse();
        Task<string> GetWareHouseAsync();

        #endregion
    }
}
