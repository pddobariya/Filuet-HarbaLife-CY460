using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using SevenSpikes.Nop.Plugins.AjaxCart.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory
{
    public class CategoryChecker : ICategoryChecker
    {
        #region Fields 

        protected readonly IWorkContext _workContext;
        protected readonly IProductService _productService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICategoryService _categoryService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public CategoryChecker(
            IWorkContext workContext,
            IProductService productService,
            IGenericAttributeService genericAttributeService,
            ICategoryService categoryService,
            IStoreContext storeContext)
        {
            _workContext = workContext;
            _productService = productService;
            _genericAttributeService = genericAttributeService;
            _categoryService = categoryService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public virtual async Task<bool> ProductsCompatibleAsync(Product product1, Product product2)
        {
            return !(await product1.GetProductTypeAsync()).HasValue || !(await product2.GetProductTypeAsync()).HasValue || ((await product1.GetProductTypeAsync()).Value == (await product2.GetProductTypeAsync()).Value);
        }

        public virtual async Task<bool> AddIfNotPublishedAsync(Product product)
        {
            return await Task.FromResult(false);
        }
        public virtual async Task<bool> CheckStockBalanceAsync(Product product)
        {
            return await product.GetProductTypeAsync(true) == CategoryTypeEnum.Product;
        }

        public virtual async Task<JsonResult> CheckProductsCompatibilityWithMessageAsync(int productId)
        {
            var currentProduct = await _productService.GetProductByIdAsync(productId);
            IShoppingCartService shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            var shoppingCartItems = await shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: (await _storeContext.GetCurrentStoreAsync()).Id);

            if (await shoppingCartItems.AnyAwaitAsync(async sci =>
            {
                var sciProductOrderCategory = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetWorkingCurrencyAsync(), ProductAttributeNames.ProductForOrderCategoryAttribute);
                var currentProductOrderCategory = await _genericAttributeService.GetAttributeAsync<string>(await _workContext.GetWorkingCurrencyAsync(), ProductAttributeNames.ProductForOrderCategoryAttribute);
                return !string.IsNullOrWhiteSpace(sciProductOrderCategory) && sciProductOrderCategory != OrderCategories.Empty &&
                       !string.IsNullOrWhiteSpace(currentProductOrderCategory) && currentProductOrderCategory != OrderCategories.Empty &&
                       sciProductOrderCategory != currentProductOrderCategory;
            }))
            {
                var model = new AddProductToCartResultModel();
                model.Status = "productsconflict";
                model.PopupTitle = "Добавить в корзину не удалось из-за следующих предупреждений";
                model.AddToCartWarnings =
                    "В корзине находятся товары другого типа. Если вы хотите добавить товар в корзину, остальные товары будут удалены. Продолжить?";
                return new JsonResult(model);
            }
            var bizWorkProduct = await BizWorkShoppingCartHelper.GetBizWorkProductAsync(await _workContext.GetCurrentCustomerAsync());
            if (bizWorkProduct == null)
            {
                return null;
                //throw new Exception("bizWorkProduct not found");
            }
            var shoppingProduct = await shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
            if (await shoppingCartItems.AnyAwaitAsync(async sci => (await _productService.GetProductByIdAsync(sci.ProductId)) == bizWorkProduct) && currentProduct != bizWorkProduct)
            {
                var model = new AddProductToCartResultModel();
                model.Status = "frombizwork";
                model.PopupTitle = "Добавить в корзину не удалось из-за следующих предупреждений";
                model.AddToCartWarnings =
                    "В вашей корзине на данный момент находится bizwork. Если вы хотите добавить в корзину выбранный товар, то подписка bizwork будет удалена из корзины. Вы хотите продолжить покупки и удалить bizwork?";
                return new JsonResult(model);
            }
        
            if (await shoppingCartItems.AnyAwaitAsync(async sci => (await _productService.GetProductByIdAsync(sci.ProductId)) != bizWorkProduct) && currentProduct == bizWorkProduct)
            {
                var model = new AddProductToCartResultModel();
                model.Status = "bizwork";
                model.PopupTitle = "Добавить в корзину не удалось из-за следующих предупреждений";
                model.AddToCartWarnings =
                    "В вашей корзине в данный момент находятся товары. При оформлении bizwork текущие товары в корзине будут удалены. Вы хотите  обнулить корзину и добавить bizwork?";
                return new JsonResult(model);
            }
            return null;
        }

        public virtual async Task<IList<Category>> GetCategoriesByProductIdAsync(int productId)
        {
            var productCategories = await _categoryService.GetProductCategoryIdsAsync(new[] { productId });
            return await _categoryService.GetCategoriesByIdsAsync(productCategories.SelectMany(x => x.Value).Distinct().ToArray());
        }

        #endregion
    }
}
