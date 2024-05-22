using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public static class BizWorkShoppingCartHelper
    {
        #region Methods

        public async static Task<bool> ReplaceShoppingCartWithBizWorkItemAsync(Customer customer)
        {
            var shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var storeContext = EngineContext.Current.Resolve<IStoreContext>();

            var cartItems = await shoppingCartService.GetShoppingCartAsync(customer,
                  ShoppingCartType.ShoppingCart, storeId: (await storeContext.GetCurrentStoreAsync()).Id);
            foreach (var cartItem in cartItems)
            {
                await shoppingCartService.DeleteShoppingCartItemAsync(cartItem);
            }

            var product = await GetBizWorkProductAsync(customer);

            if (product == null)
            {
                return false;
            }

            var storeId = EngineContext.Current.Resolve<IStoreContext>().GetCurrentStoreAsync().Id;
            await shoppingCartService.AddToCartAsync(customer, product, ShoppingCartType.ShoppingCart, storeId);

            return true;
        }

        public async static Task<Product> GetBizWorkProductAsync(Customer customer)
        {
            var settingKey = ShoppingConstants.BizWorkSkuKey;
            var defaultSkuValue = ShoppingConstants.BizWorkSkuDefaultValue;
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            var productService = EngineContext.Current.Resolve<IProductService>();
            var sku = await settingService.GetSettingByKeyAsync(settingKey, defaultSkuValue);

            return await productService.GetProductBySkuAsync(sku);
        }

        #endregion
    }
}
