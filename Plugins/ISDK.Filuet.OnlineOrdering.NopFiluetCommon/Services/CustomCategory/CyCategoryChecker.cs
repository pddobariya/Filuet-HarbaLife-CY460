using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory
{
    public class CyCategoryChecker : CategoryChecker, ICategoryChecker
    {
        #region Fields

        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public CyCategoryChecker(
            IWorkContext workContext,
            IProductService productService, 
            IGenericAttributeService genericAttributeService,
            ICategoryService categoryService,
            IStoreContext storeContext) 
            : base(workContext, 
                  productService,
                  genericAttributeService,
                  categoryService,
                  storeContext)
        {
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public override async Task<bool> ProductsCompatibleAsync(Product product1, Product product2)
        {
            if (product1.Sku == "5451" && product2.Sku == "3798" || product2.Sku == "5451" && product1.Sku == "3798")
                return true;

            return !(await product1.GetProductTypeAsync()).HasValue || !(await product2.GetProductTypeAsync()).HasValue || ((await product1.GetProductTypeAsync()).Value == (await product2.GetProductTypeAsync()).Value);
        }

        public override async Task<JsonResult> CheckProductsCompatibilityWithMessageAsync(int productId)
        {
            IShoppingCartService shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var ShoppingCartItems = await shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (await ShoppingCartItems.AllAwaitAsync(async sci => await ProductsCompatibleAsync((await _productService.GetProductByIdAsync(sci.ProductId)), (await _productService.GetProductByIdAsync(productId)))))
            {
                return null;
            }
            return await base.CheckProductsCompatibilityWithMessageAsync(productId);
        }

        public override async Task<bool> AddIfNotPublishedAsync(Product product)
        {
            return  await Task.FromResult(product.Sku == "3798");
        }

        public override async Task<bool> CheckStockBalanceAsync(Product product)
        {
            return await base.CheckStockBalanceAsync(product) && product.Sku != "3798";
        }

        #endregion
    }
}
