using Filuet.OnlineOrdering.CyExtendedFunctions.Controllers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.ShoppingCart;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Date;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.OnlineOrdering.CyExtendedFunctions.Services
{
    public class CyFiluetShoppingCartService : FiluetShoppingCartService
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<ShoppingCartItem> _sciRepository;

        #endregion

        #region Ctor

        public CyFiluetShoppingCartService(CatalogSettings catalogSettings,
            IAclService aclService, 
            IActionContextAccessor actionContextAccessor, 
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService, 
            ICurrencyService currencyService, 
            ICustomerService customerService, 
            IDateRangeService dateRangeService, 
            IDateTimeHelper dateTimeHelper, 
            IGenericAttributeService genericAttributeService, 
            ILocalizationService localizationService, 
            IPermissionService permissionService, 
            IPriceCalculationService priceCalculationService, 
            IPriceFormatter priceFormatter, 
            IProductAttributeParser productAttributeParser, 
            IProductAttributeService productAttributeService, 
            IProductService productService, 
            IRepository<ShoppingCartItem> sciRepository,
            IShippingService shippingService, 
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext, 
            IStoreService storeService,
            IStoreMappingService storeMappingService, 
            IUrlHelperFactory urlHelperFactory, 
            IUrlRecordService urlRecordService, 
            IWorkContext workContext,
            OrderSettings orderSettings, 
            ShoppingCartSettings shoppingCartSettings,
            ILogger logger, ICategoryChecker categoryChecker, 
            ICategoryService categoryService, 
            IEventPublisher eventPublisher,
            FusionValidationService fusionValidationService) 
            : base(catalogSettings,
                  aclService,
                  actionContextAccessor,
                  checkoutAttributeParser,
                  checkoutAttributeService,
                  currencyService,
                  customerService,
                  dateRangeService, 
                  dateTimeHelper, 
                  genericAttributeService, 
                  localizationService,
                  permissionService,
                  priceCalculationService, 
                  priceFormatter,
                  productAttributeParser,
                  productAttributeService,
                  productService, 
                  sciRepository,
                  shippingService,
                  staticCacheManager,
                  storeContext, 
                  storeService,
                  storeMappingService,
                  urlHelperFactory,
                  urlRecordService, 
                  workContext,
                  orderSettings,
                  shoppingCartSettings,
                  logger, 
                  categoryChecker,
                  categoryService,
                  eventPublisher, 
                  fusionValidationService)
        {
            _genericAttributeService = genericAttributeService;
            _sciRepository = sciRepository;
        }

        #endregion

        #region Method

        public override async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            var idToDelete = await _genericAttributeService.GetAttributeAsync<long>(shoppingCartItem,
                CyExtendedFunctionsController.Sku3798IdAttribute);
           await base.DeleteShoppingCartItemAsync((int)idToDelete, resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
           await  base.DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
        }

        public override async Task<IList<string>> UpdateShoppingCartItemAsync(Customer customer, int shoppingCartItemId, string attributesXml,
            decimal customerEnteredPrice, DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, int quantity = 1,
            bool resetCheckoutData = true)
        {
            var shoppingCartItem = _sciRepository.Table.FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem != null)
            {
                var idToDelete = await _genericAttributeService.GetAttributeAsync<long>(shoppingCartItem,
                    CyExtendedFunctionsController.Sku3798IdAttribute);
               await base.DeleteShoppingCartItemAsync((int)idToDelete, resetCheckoutData, false);
            }
            return await base.UpdateShoppingCartItemAsync(customer, shoppingCartItemId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, resetCheckoutData);
        }

        #endregion
    }
}
