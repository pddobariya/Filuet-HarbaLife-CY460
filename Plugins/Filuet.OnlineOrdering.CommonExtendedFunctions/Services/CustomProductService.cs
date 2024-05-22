using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace Filuet.OnlineOrdering.CommonExtendedFunctions.Services
{
    public class CustomProductService : ICustomProductService
    {
        #region Filelds

        private readonly string _wareHouse;

        #endregion

        #region Ctor

        public CustomProductService(IFiluetShippingService filuetShippingService)
        {
            _wareHouse = filuetShippingService.GetWareHouse().Result;
        }

        #endregion

        #region Methods

        public IEnumerable<Tuple<Product, string>> GetProductWarehousePairs(Product[] products)
        {
            foreach (var product in products)
            {
                yield return new Tuple<Product, string>(product, _wareHouse);
            }
        }

        #endregion
    }
}