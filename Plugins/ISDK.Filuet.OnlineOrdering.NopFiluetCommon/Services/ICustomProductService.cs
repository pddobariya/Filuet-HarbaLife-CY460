using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services
{
    public interface ICustomProductService
    {
        #region Methods

        IEnumerable<Tuple<Product, string>> GetProductWarehousePairs(Product[] products);

        #endregion
    }
}