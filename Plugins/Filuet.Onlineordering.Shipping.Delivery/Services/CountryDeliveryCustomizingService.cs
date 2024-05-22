using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class CountryDeliveryCustomizingService : ICountryDeliveryCustomizingService
    {
        #region Fileds

        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public CountryDeliveryCustomizingService(
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            IProductService productService, 
            IStoreContext storeContext,
            IGenericAttributeService genericAttributeService)
        {
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public async Task<decimal> GetDeliveryPriceCriterionValue()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var shoppingcart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            return await shoppingcart.AggregateAwaitAsync(0m, async (sum, sci) => (await _genericAttributeService.GetAttributeAsync<decimal>(await _productService.GetProductByIdAsync(sci.ProductId), ProductAttributeNames.BasicRetailPriceAttribute) * (sci.Quantity)) + sum);
        }
        public async Task<bool> IsOnlySelfPickup()
        {
            return await Task.FromResult(false);
        }

        #endregion
    }
}
