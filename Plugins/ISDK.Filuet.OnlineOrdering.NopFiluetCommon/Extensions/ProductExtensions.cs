using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions
{
    public static class ProductExtensions
    {
        #region Methods

        public async static Task<CategoryTypeEnum?> GetProductTypeAsync(this Product product, bool tolerateMissingCategories = true)
        {
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            ICategoryService categoryService;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                categoryService = serviceScope.ServiceProvider.GetService<ICategoryService>();
            }

            IGenericAttributeService genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var productCategories = await categoryService?.GetProductCategoriesByProductIdAsync(product.Id, true);

            ProductCategory firstNonProductProductCategory = null;
            CategoryTypeEnum? firstNonProductCategoryType = null;

            int categoriesWithCategoryTypeAttributeCount = 0;

            foreach (var productCategory in productCategories)
            {
                var categoryAttributes = await genericAttributeService.GetAttributesForEntityAsync(productCategory.CategoryId, nameof(Category));
                if (!categoryAttributes.Any(
                    x => x.Key.Equals(CategoryAttributeNames.IsMeta, StringComparison.InvariantCultureIgnoreCase)
                        && x.Value.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
                    && categoryAttributes.Any(x => x.Key.Equals(CategoryAttributeNames.CategoryType, StringComparison.InvariantCultureIgnoreCase)))
                {
                    categoriesWithCategoryTypeAttributeCount++;

                    // if at least one of the product categories is a non-product one then we deem product as non product
                    if (categoryAttributes.Any(
                        x => x.Key.Equals(CategoryAttributeNames.CategoryType, StringComparison.InvariantCultureIgnoreCase)
                            && !x.Value.Equals(nameof(CategoryTypeEnum.Product), StringComparison.InvariantCultureIgnoreCase))/* && firstNonProductProductCategory != null*/)
                    {
                        firstNonProductProductCategory = productCategory;
                        firstNonProductCategoryType = genericAttributeService.GetCustomAttributeValue<CategoryTypeEnum>(categoryAttributes, CategoryAttributeNames.CategoryType);
                    }
                }
            }

            if (firstNonProductProductCategory == null)
            {
                // first try to find if categories exist at all and thus contains product only categories
                if (categoriesWithCategoryTypeAttributeCount > 0)
                {
                    return CategoryTypeEnum.Product;
                }

                return tolerateMissingCategories == true ? (CategoryTypeEnum?)CategoryTypeEnum.Product : null;
            }

            return firstNonProductCategoryType;
        }

        public async static Task<CategoryTypeEnum> GetCategoryTypeAsync(this IEnumerable<Product> products)
        {
            Product firstNonProductProduct =await products.FirstOrDefaultAwaitAsync(async x =>(await x.GetProductTypeAsync()).Value != CategoryTypeEnum.Product);

            if (firstNonProductProduct != null)
            {
                return (await firstNonProductProduct.GetProductTypeAsync()).Value;
            }

            return CategoryTypeEnum.Product;
        }

        public async static Task<ProductAttributeCombination> GetProductAttributeCombinationByAttributesAsync(this Product product, IProductAttributeService attrServ, Dictionary<string, string> attrsData)
        {
            if (product == null || attrsData == null || attrsData.Count == 0 || attrServ == null)
            {
                return null;
            }

            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            IProductAttributeService productAttributeService;
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                productAttributeService = serviceScope.ServiceProvider.GetService<IProductAttributeService>();
            }

            ProductAttributeCombination combination = null;

            foreach (ProductAttributeCombination comb in await productAttributeService.GetAllProductAttributeCombinationsAsync(product.Id))
            {
                Dictionary<ProductAttribute, ProductAttributeValue> combAttrs = await comb.ToProductAttributesAsync(attrServ);
                if (combAttrs == null || combAttrs.Count != attrsData.Count)
                {
                    continue;
                }

                bool isMatch = true;
                foreach (KeyValuePair<ProductAttribute, ProductAttributeValue> attr in combAttrs)
                {
                    string attrId = Convert.ToString(attr.Key.Id);
                    string attrValId = Convert.ToString(attr.Value.Id);
                    if (!attrsData.Any(a => a.Key == attrId && a.Value == attrValId))
                    {
                        isMatch = false;
                        break;
                    }

                    if (!isMatch)
                    {
                        break;
                    }
                }

                if (isMatch)
                {
                    combination = comb;
                    break;
                }
            }

            return combination;
        }

        #endregion
    }
}
