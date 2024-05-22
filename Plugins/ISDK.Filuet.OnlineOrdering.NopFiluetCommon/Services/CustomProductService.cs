using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services
{
    public class CustomProductService : ICustomProductService
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly string _wareHouse;

        #endregion

        #region Ctor

        public CustomProductService(
            IFiluetShippingService filuetShippingService,
            ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _wareHouse = filuetShippingService.GetWareHouseAsync().Result;
        }

        #endregion

        #region Methods


        public IEnumerable<Tuple<Product, string>> GetProductWarehousePairs(Product[] products)
        {
            var categories = _categoryService.GetAllCategoriesAsync(showHidden: true).Result.Where(x => x.Name == "EE" || x.Name == "LT").ToArray();
            foreach (var product in products)
            {
                var productCategories = _categoryService.GetProductCategoriesByProductIdAsync(product.Id, true).Result;

                var wareHouse = _wareHouse;
                var category = categories.FirstOrDefault(c => productCategories.Any(pc => pc.CategoryId == c.Id));
                if (category != null)
                {
                    wareHouse = category.Name;
                }
                yield return new Tuple<Product, string>(product, wareHouse);
            }
        }

        #endregion
    }
}