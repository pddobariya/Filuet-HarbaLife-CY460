using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory
{
    public interface ICategoryChecker
    {
        #region Methods

        Task<bool> ProductsCompatibleAsync(Product product1, Product product2);
        Task<bool> AddIfNotPublishedAsync(Product product);
        Task<bool> CheckStockBalanceAsync(Product product);
        Task<JsonResult> CheckProductsCompatibilityWithMessageAsync(int productId);
        Task<IList<Category>> GetCategoriesByProductIdAsync(int productId);

        #endregion
    }
}