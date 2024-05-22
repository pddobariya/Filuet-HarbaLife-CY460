using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class FiluetShippingCartService : IFiluetShippingCartService
    {
        #region Fields

        private string[] categoriesCountry = { "EE", "LV", "LT", "RU" };

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly IShippingWidgetService _shippingWidgetService;
        private readonly ICategoryService _categoryService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public FiluetShippingCartService(
            string[] categoriesCountry,
            ILogger logger,
            IWorkContext workContext,
            IFiluetShippingService filuetShippingService,
            IShippingWidgetService shippingWidgetService,
            ICategoryService categoryService,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IGenericAttributeService genericAttributeService, 
            ILocalizationService localizationService)
        {
            this.categoriesCountry = categoriesCountry;
            _logger = logger;
            _workContext = workContext;
            _filuetShippingService = filuetShippingService;
            _shippingWidgetService = shippingWidgetService;
            _categoryService = categoryService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public async Task<string[]> IsCartValid()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            var filuetFusionShippingComputationOption = await _filuetShippingService.GetShippingComputationOptionByCustomerIdAsync(currentCustomer.Id);
            if (filuetFusionShippingComputationOption is null)
            {
                await _logger.InformationAsync($"FiluetShippingCartService: filuetFusionShippingComputationOption is null. Customer Id: {currentCustomer.Id}");
                // Крч эта штука где-то записывает в БД данные которые не успела записать. Осталось от предыдущего разраба. В идеале бы найти где что как и вызывать в нужное время а не тут.
                await _shippingWidgetService.GetShippingComputationOptionsAsync();
                filuetFusionShippingComputationOption =await _filuetShippingService.GetShippingComputationOptionByCustomerIdAsync(currentCustomer.Id);
            }

            var countiesCodes = (await _categoryService.GetAllCategoriesAsync(showHidden: true)).Where(x => categoriesCountry.Contains(x.Name)).ToArray();
            var nowCountryId = countiesCodes.FirstOrDefault(x => x.Name == filuetFusionShippingComputationOption.CountryCode)?.Id??0;
            var countiesCodesIds = countiesCodes.Select(x => x.Id);

            var shoppingCartItems = await (await _shoppingCartService.GetShoppingCartAsync(currentCustomer)).WhereAwait(async sci =>
            {
                var categories = (await _categoryService.GetProductCategoriesByProductIdAsync(sci.ProductId, true)).Select(x => x.CategoryId);
                if (categories.Any(x => countiesCodesIds.Contains(x)))
                    return !categories.Any(x => x == nowCountryId);
                return false;
            }).ToListAsync();

            var skus = await shoppingCartItems.SelectAwait(async x => (await _productService.GetProductByIdAsync(x.ProductId)).Sku).ToArrayAsync();
            if (skus.Any())
                return new[]
                {
                    string.Format(await _localizationService.GetResourceAsync("Cart.Dialogiscartvalid") + " {0}",
                        string.Join(",", skus))
                };

            return Array.Empty<string>();
        }

        public async Task<string[]> IsCartValid(Customer currentCustomer, List<ShoppingCartItem> cart, (double monthLimit, double oneOrderLimit) limits)
        {
            var filuetFusionShippingComputationOption =await _filuetShippingService.GetShippingComputationOptionByCustomerIdAsync(currentCustomer.Id);
            if (filuetFusionShippingComputationOption is null)
            {
                await _logger.InformationAsync($"FiluetShippingCartService: filuetFusionShippingComputationOption is null. Customer Id: {currentCustomer.Id}");
                // Крч эта штука где-то записывает в БД данные которые не успела записать. Осталось от предыдущего разраба. В идеале бы найти где что как и вызывать в нужное время а не тут.
                await _shippingWidgetService.GetShippingComputationOptionsAsync();
                filuetFusionShippingComputationOption =await _filuetShippingService.GetShippingComputationOptionByCustomerIdAsync(currentCustomer.Id);
            }

            double cartVp = 0;

            var weHaveDublicate = cart.GroupBy(x => x.ProductId).Select(x => new { ProductId = x.Key, Count = x.Count() }).Any(x => x.Count > 1);
            if (weHaveDublicate)
            {
                return
                        new[]
                        {
                            $"{await _localizationService.GetResourceAsync("Cart.DialogisWeHaveDublicateProducts")}"
                        };
            }

            foreach (var item in cart)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                cartVp +=await _genericAttributeService.GetAttributeAsync<double>(product,ProductAttributeNames.VolumePoints) * item.Quantity;
            }

            if (cartVp > limits.oneOrderLimit)
            {
                return new[] { 
                    string.Format(await _localizationService.GetResourceAsync("Cart.OneOrderVpLimit"), limits.oneOrderLimit) 
                };
            }

            if (limits.monthLimit > 0)
            {
                var selectedLimitsYear = await _genericAttributeService.GetAttributeAsync<int>(currentCustomer, CustomerAttributeNames.SelectedLimitsYear);
                var selectedLimitsMonth = await _genericAttributeService.GetAttributeAsync<int>(currentCustomer, CustomerAttributeNames.SelectedLimitsMonth);
                var monthVpStr = await _genericAttributeService.GetAttributeAsync<double>(currentCustomer, CustomerAttributeNames.MonthVolumePoints + selectedLimitsYear.ToString("0000").Remove(0, 2) + "." + selectedLimitsMonth.ToString("00"));

                if (monthVpStr + cartVp > limits.monthLimit)
                {
                    return new[] { 
                        string.Format(await _localizationService.GetResourceAsync("Cart.MonthVpLimit"), limits.monthLimit - monthVpStr) 
                    };
                }
            }

            var countiesCodes = (await _categoryService.GetAllCategoriesAsync(showHidden: true)).Where(x => categoriesCountry.Contains(x.Name)).ToArray();
            var nowCountryId = countiesCodes.FirstOrDefault(x => x.Name == filuetFusionShippingComputationOption.CountryCode)?.Id ?? 0;
            var countiesCodesIds = countiesCodes.Select(x => x.Id);

            var shoppingCartItems = await (await _shoppingCartService.GetShoppingCartAsync(currentCustomer)).WhereAwait(async sci =>
            {
                var categories = (await _categoryService.GetProductCategoriesByProductIdAsync(sci.ProductId, true)).Select(x => x.CategoryId);
                if (categories.Any(x => countiesCodesIds.Contains(x)))
                    return !categories.Any(x => x == nowCountryId);
                return false;
            }).ToListAsync();

            var skus = await shoppingCartItems.SelectAwait(async x => (await _productService.GetProductByIdAsync(x.ProductId)).Sku).ToArrayAsync();
            if (skus.Any())
                return new[]
                {
                    string.Format(await _localizationService.GetResourceAsync("Cart.Dialogiscartvalid") + " {0}",
                        string.Join(",", skus))
                };

            return Array.Empty<string>();
        }

        #endregion
    }
}