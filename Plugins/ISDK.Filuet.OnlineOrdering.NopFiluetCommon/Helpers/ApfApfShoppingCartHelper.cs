using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums.HerbalifeRESTServicesSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public class ApfApfShoppingCartHelper
    {
        #region Methods

        public async static Task RemoveApfItemAsync(Customer customer)
        {
            IApfExtendedFunctionsHelper apfExtendedFunctionsHelper = null;
            try
            {
                apfExtendedFunctionsHelper = EngineContext.Current.Resolve<IApfExtendedFunctionsHelper>();
            }
            catch { }
            var apfDueDateWarningPeriodDays = apfExtendedFunctionsHelper?.GetAPFDueDateWarningPeriodDays() ?? 0;
            var apfDueDate = await customer.GetApfDueDateAsync();
            if (apfDueDate != null && apfDueDate.Value.AddDays(-apfDueDateWarningPeriodDays) <= DateTime.Today)
            {
                return;
            }

            var product = GetApfProductAsync(customer);
            if (product == null)
                return;
            var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var apfItem = (await shoppingCartService.GetShoppingCartAsync(customer)).FirstOrDefault(x => x.ProductId == product.Id);

            if (apfItem != null)
            {
               await shoppingCartService.DeleteShoppingCartItemAsync(apfItem);
            }
        }

        public async static Task<bool> ReplaceShoppingCartWithApfItemAsync(Customer customer)
        {
            var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var cartItems =(await shoppingCartService.GetShoppingCartAsync(customer)).ToList();
            foreach (var cartItem in cartItems)
            {
                await shoppingCartService.DeleteShoppingCartItemAsync(cartItem);
            }

            var product = await GetApfProductAsync(customer);

            if (product == null)
            {
                return false;
            }
            IStoreContext storeContext = EngineContext.Current.Resolve<IStoreContext>();
            var storeId = (await storeContext.GetCurrentStoreAsync()).Id;
            await shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, storeId);

            return true;
        }

        public async static Task<bool> ShoppingCartContainsApfItemAsync(Customer customer)
        {
            var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var apfProduct = await GetApfProductAsync(customer);
            if (apfProduct == null)
            {
                return false;
            }

            var cartItems =(await shoppingCartService.GetShoppingCartAsync(customer)).ToList();

            return cartItems.Count == 1 && cartItems[0].ProductId == apfProduct.Id;
        }

        public async static Task<Product> GetApfProductAsync(Customer customer)
        {
            var productService = EngineContext.Current.Resolve<IProductService>();
            var apfSku = await GetApfProductSkuAsync(customer);

            return await productService.GetProductBySkuAsync(apfSku);
        }

        public async static Task<string> GetApfProductSkuAsync(Customer customer)
        {
            var distributorType = await customer.GetDistributorTypeAsync();
            var settingKey = ShoppingConstants.SupervisorApfSkuKey;
            var defaultApfSkuValue = ShoppingConstants.SupervisorApfSkuDefaultValue;
            if (distributorType == DistributorTypes.Distributor)
            {
                settingKey = ShoppingConstants.DistributorApfSkuKey;
                defaultApfSkuValue = ShoppingConstants.DistributorApfSkuDefaultValue;
            }
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            return await settingService.GetSettingByKeyAsync(settingKey, defaultApfSkuValue);
        }

        #endregion
    }
}