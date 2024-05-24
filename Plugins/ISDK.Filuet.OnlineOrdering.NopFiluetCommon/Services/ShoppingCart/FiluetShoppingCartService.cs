using DocumentFormat.OpenXml.Spreadsheet;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using MailKit;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
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
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.ShoppingCart
{
    /// <summary>
    /// Extends the existing shopping cart service
    /// </summary>
    public class FiluetShoppingCartService : ShoppingCartService, IShoppingCartService
    {
        #region Fields

        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ILogger _logger;
        private readonly ICategoryChecker _categoryChecker;
        private readonly IEventPublisher _eventPublisher;
        private readonly FusionValidationService _fusionValidationService;

        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly OrderSettings _orderSettings;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICurrencyService _currencyService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDateRangeService _dateRangeService;

        #endregion

        #region Ctor

        public FiluetShoppingCartService(
            CatalogSettings catalogSettings,
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
            ILogger logger,
            ICategoryChecker categoryChecker,
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
                  shoppingCartSettings)
        {
            _aclService = aclService;
            _checkoutAttributeParser = checkoutAttributeParser;
            _currencyService = currencyService;
            _customerService = customerService;
            _dateRangeService = dateRangeService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _priceFormatter = priceFormatter;
            _productAttributeParser = productAttributeParser;
            _productAttributeService = productAttributeService;
            _productService = productService;
            _sciRepository = sciRepository;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _logger = logger;
            _categoryChecker = categoryChecker;
            _eventPublisher = eventPublisher;
            _fusionValidationService = fusionValidationService;
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Adds a new shopping cart item
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="product">Product</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store id</param>
        /// <param name="attributesXml">Attributes xml</param>
        /// <param name="customerEnteredPrice">Customer entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="automaticallyAddRequiredProductsIfEnabled">Automatically add required products if enabled</param>
        /// <returns>Warnings</returns>
        public override async Task<IList<string>> AddToCartAsync(Customer customer, Product product, ShoppingCartType shoppingCartType, int storeId, string attributesXml = null, decimal customerEnteredPrice = 0, DateTime? rentalStartDate = null, DateTime? rentalEndDate = null, int quantity = 1, bool automaticallyAddRequiredProductsIfEnabled = true)
        {
            _logger.Information("DEBUG: Extended Shopping Cart called");
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            List<string> warnings = new List<string>();

            if (product.Sku == await ApfApfShoppingCartHelper.GetApfProductSkuAsync(customer))
            {
                await AddShoppingCartItemAsync(product, quantity, storeId, attributesXml, rentalStartDate, rentalEndDate,
                     customerEnteredPrice, shoppingCartType, customer);
                return warnings;
            }

            try
            {
                if (await customer.GetCantBuyFlagAsync())
                {
                    warnings.Add(await _localizationService.GetResourceAsync("HBL.Baltic.OnlineOrdering.ShoppingPlugin.Resources.CantBuy"));
                    return warnings;
                }
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"FiluetShoppingCartService.AddToCartAsync CustomerId = {customer?.Id}", ex);
            }

            var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);

            //check limits on add

            double volumePoints = (await Task.WhenAll(cart.Select(async x =>
            {
                var product = await _productService.GetProductByIdAsync(x.ProductId);
                var volumePoints = await _genericAttributeService.GetAttributeAsync<double>(product, ProductAttributeNames.VolumePoints);
                return x.Quantity * volumePoints;
            }))).Sum();
            await _logger.InformationAsync(string.Format("Cart before add: size - {0}, VPs - {1}", cart.Count, volumePoints));
            volumePoints += Convert.ToDouble(await _genericAttributeService.GetAttributeAsync<double>(product, ProductAttributeNames.VolumePoints) * quantity);
            await _logger.InformationAsync(string.Format("Cart after add: size - {0}, VPs - {1}", cart.Count + 1, volumePoints));

            //Check if ticket
            if (await (await _categoryChecker.GetCategoriesByProductIdAsync(product.Id))
                .AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) == CategoryTypeEnum.Ticket))
            {
                var totalQuantity = quantity +
                                    await _genericAttributeService.GetAttributeAsync<int>(customer, CoreGenericAttributes.TicketAttribute + product.Sku);
                if (totalQuantity > 2)
                {
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.QuantityOfTicketsExceed"));
                    return warnings;
                }
                if (await cart.AnyAwaitAsync(async item => await (await _categoryChecker.GetCategoriesByProductIdAsync(item.ProductId)).AllAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) != CategoryTypeEnum.Ticket)))
                {
                    (await GetShoppingCartAsync(customer)).ForEach(async sci => await DeleteShoppingCartItemAsync(sci));
                    await AddShoppingCartItemAsync(product, quantity, storeId, attributesXml, rentalStartDate, rentalEndDate,
                        customerEnteredPrice, shoppingCartType, customer);
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.OnlyTickets"));
                    return warnings;
                }
            }
            else if (await cart.AnyAwaitAsync(async item => await (await _categoryChecker.GetCategoriesByProductIdAsync(item.ProductId)).AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) == CategoryTypeEnum.Ticket)))
            {
                (await GetShoppingCartAsync(customer)).ForEach(async sci => await DeleteShoppingCartItemAsync(sci));
                await AddShoppingCartItemAsync(product, quantity, storeId, attributesXml, rentalStartDate, rentalEndDate,
                    customerEnteredPrice, shoppingCartType, customer);
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.OnlyProducts"));
                return warnings;
            }

            DistributorLimitsModel distributorLimits = null;

            distributorLimits = distributorLimits = await _fusionValidationService.ValidateDistributorLimitsAsync(customer, await GetShoppingCartAsync(customer), volumePoints);

            if (!distributorLimits.IsValid)
            {
                string warningMessage = string.Format(await ErrorMessages.OrderLimitExceedNotUpdated.ToLocalizedStringAsync(), distributorLimits.ExceedanceAmount.HasValue ? distributorLimits.ExceedanceAmount.Value : 0);
                warnings.Add(warningMessage);
                return warnings;
            }
            bool isDebtor = await customer.IsDebtorAsync();

            if (isDebtor && !await (await _categoryChecker.GetCategoriesByProductIdAsync(product.Id)).AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) == CategoryTypeEnum.Maintenance))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ApfOnlyAllowed"));
                return warnings;
            }

            if (shoppingCartType == ShoppingCartType.ShoppingCart && !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart, customer))
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }
            if (shoppingCartType == ShoppingCartType.Wishlist && !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist, customer))
            {
                warnings.Add("Wishlist is disabled");
                return warnings;
            }
            if (customer.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            await _customerService.ResetCheckoutDataAsync(customer, storeId);

            var shoppingCartItem = await FindShoppingCartItemInTheCartAsync(cart,
                shoppingCartType, product, attributesXml, customerEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                int newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                    storeId, attributesXml,
                    customerEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, automaticallyAddRequiredProductsIfEnabled));

                if (!warnings.Any())
                {
                    int oldQuantity = shoppingCartItem.Quantity;
                    shoppingCartItem.AttributesXml = attributesXml;
                    shoppingCartItem.Quantity = newQuantity;
                    shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

                    await _sciRepository.UpdateAsync(shoppingCartItem);
                    // await _customerService.UpdateCustomerAsync(customer);

                    //Update paired stock items
                    await UpdatePairedStockItems(shoppingCartItem, oldQuantity, newQuantity);

                    //event notification
                    _eventPublisher.EntityUpdated(shoppingCartItem);
                }
            }
            else
            {
                bool shoppingCartContainsDifferentCategory = await (await GetShoppingCartAsync(customer)).AnyAwaitAsync(
                    async x => !await _categoryChecker.ProductsCompatibleAsync(await _productService.GetProductByIdAsync(x.ProductId), product));

                if (shoppingCartContainsDifferentCategory)
                {
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ContainsDifferentCategory"));
                    return warnings;
                }

                warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product,
                    storeId, attributesXml, customerEnteredPrice,
                    rentalStartDate, rentalEndDate,
                    quantity, automaticallyAddRequiredProductsIfEnabled));

                if (!warnings.Any())
                {
                    //maximum items validation
                    switch (shoppingCartType)
                    {
                        case ShoppingCartType.ShoppingCart:
                            {
                                if (cart.Count >= this._shoppingCartSettings.MaximumShoppingCartItems)
                                {
                                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), this._shoppingCartSettings.MaximumShoppingCartItems));
                                    return warnings;
                                }
                            }
                            break;
                        case ShoppingCartType.Wishlist:
                            {
                                if (cart.Count >= this._shoppingCartSettings.MaximumWishlistItems)
                                {
                                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumWishlistItems"), this._shoppingCartSettings.MaximumWishlistItems));
                                    return warnings;
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    shoppingCartItem = await AddShoppingCartItemAsync(product, quantity, storeId, attributesXml, rentalStartDate, rentalEndDate,
                        customerEnteredPrice, shoppingCartType, customer);
                }
            }

            return warnings;
        }

        private async Task<ShoppingCartItem> AddShoppingCartItemAsync(Product product, int quantity, int storeId, string attributesXml, DateTime? rentalStartDate, DateTime? rentalEndDate, decimal customerEnteredPrice, ShoppingCartType shoppingCartType, Customer customer)
        {
            DateTime now = DateTime.UtcNow;
            var shoppingCartItem = new ShoppingCartItem
            {
                ShoppingCartType = shoppingCartType,
                StoreId = storeId,
                ProductId = product.Id,
                AttributesXml = attributesXml,
                CustomerEnteredPrice = customerEnteredPrice,
                Quantity = quantity,
                RentalStartDateUtc = rentalStartDate,
                RentalEndDateUtc = rentalEndDate,
                CreatedOnUtc = now,
                UpdatedOnUtc = now,
                CustomerId = customer.Id
            };

            var shoppingCartItems = await GetShoppingCartAsync(customer);

            if (shoppingCartItems != null && shoppingCartItems.Select(x => x.ProductId).ToList().Any())
                await _logger.InsertLogAsync(LogLevel.Debug, $"Get ShoppingCartItem customer, Quantity {shoppingCartItems.Select(x => x.Quantity).Sum()} ,product id :{string.Join(",", shoppingCartItems.Select(x => x.ProductId).ToList())}", customer: customer);

            if (!shoppingCartItems.Any(x => x.ProductId == shoppingCartItem.ProductId))
            {
                await _logger.InsertLogAsync(LogLevel.Debug, $"Add ShoppingCartItem , Quantity {shoppingCartItem.Quantity}, ProductId {shoppingCartItem.ProductId}", customer: customer);

                await _sciRepository.InsertAsync(shoppingCartItem);
                shoppingCartItems = await GetShoppingCartAsync(customer);
            }

            //updated "HasShoppingCartItems" property used for performance optimization
            //customer.HasShoppingCartItems = shoppingCartItems.Any();
            customer.HasShoppingCartItems = !IsCustomerShoppingCartEmpty(customer);
            await _customerService.UpdateCustomerAsync(customer);

            //Update paired stock items
            await UpdatePairedStockItems(shoppingCartItem, 0, quantity);


            //event notification
            await _eventPublisher.EntityInsertedAsync(shoppingCartItem);

            return shoppingCartItem;
        }

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current customer</param>
        public override async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            if (shoppingCartItem == null)
            {
                throw new ArgumentNullException("shoppingCartItem");
            }
            _logger.Information(string.Format("DEBUG: DeleteShoppingCartItem - sci ID: {0}", shoppingCartItem.Id));

            

            Customer customer = await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId);
            if (customer == null)
            {
                _logger.Information("DEBUG: DeleteShoppingCartItem - customer not found");
            }

            int storeId = shoppingCartItem.StoreId;
            _logger.Information(string.Format("DEBUG: DeleteShoppingCartItem - storeId: {0}", storeId));
            //reset checkout data
            if (resetCheckoutData)
            {
                await _customerService.ResetCheckoutDataAsync(await _customerService.GetCustomerByIdAsync(shoppingCartItem.CustomerId), shoppingCartItem.StoreId);
            }

            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
            // reducing the quantity of paired stock items
            if (product != null && product.RequireOtherProducts)
            {
                // find all required items                
                var ids = _productService.ParseRequiredProductIds(product);

                var cart = (await GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync()))
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart && ids.Contains(sci.ProductId))
                        .ToList();

                // reduce the quantity of each item
                foreach (var item in cart)
                {
                    await UpdateShoppingCartItemAsync(
                        customer: await _workContext.GetCurrentCustomerAsync(),
                        shoppingCartItemId: item.Id,
                        attributesXml: string.Empty,
                        customerEnteredPrice: decimal.Zero,
                        quantity: item.Quantity - shoppingCartItem.Quantity,
                        resetCheckoutData: resetCheckoutData);
                }
            }

            ShoppingCartItem dbItem = await _sciRepository.GetByIdAsync(shoppingCartItem.Id);

            if (dbItem == null)
            {
                _logger.Information("DEBUG: DeleteShoppingCartItem - db item not found");
            }
            //delete item
            if (dbItem != null)
            {
                await _sciRepository.DeleteAsync(shoppingCartItem);
                //_sciRepository.Delete(dbItem);
                await _logger.InsertLogAsync(LogLevel.Debug, $"Delete ShoppingCartItem , ProductId {dbItem.ProductId}, Product Quantity{dbItem.Quantity}", customer: customer);

            }
            else
                await _logger.InsertLogAsync(LogLevel.Debug, $"Delete ShoppingCartItem, ShoppingCartItem is null", customer: customer);

            if (customer != null)
            {
                //var cart = await GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync());
                //reset "HasShoppingCartItems" property used for performance optimization
                //customer.HasShoppingCartItems = cart.Any();
                customer.HasShoppingCartItems = !IsCustomerShoppingCartEmpty(customer);
                await _customerService.UpdateCustomerAsync(customer);

                //validate checkout attributes
                if (ensureOnlyActiveCheckoutAttributes &&
                    //only for shopping cart items (ignore wishlist)
                    shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
                {
                    var cart = await GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, storeId);

                    await _logger.InsertLogAsync(LogLevel.Debug, $"Delete Cart Item After Save {cart.Sum(x=>x.Quantity)}");
                    cart = cart
                           .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                           .ToList();

                    var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.CheckoutAttributes, storeId);
                    checkoutAttributesXml = await _checkoutAttributeParser.EnsureOnlyActiveAttributesAsync(checkoutAttributesXml, cart);
                    await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.CheckoutAttributes, checkoutAttributesXml, storeId);
                }
            }
            //event notification
            _eventPublisher.EntityDeleted(shoppingCartItem);
        }

        public override async Task<IList<string>> UpdateShoppingCartItemAsync(Customer customer,
            int shoppingCartItemId, string attributesXml,
            decimal customerEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool resetCheckoutData = true)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var warnings = new List<string>();

            var shoppingCartItem = (await GetShoppingCartAsync(customer)).FirstOrDefault(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem != null)
            {
                if (await (await _categoryChecker.GetCategoriesByProductIdAsync(shoppingCartItem.ProductId))
                    .AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) == CategoryTypeEnum.Ticket))
                {
                    var totalQuantity = quantity +
                    await _genericAttributeService.GetAttributeAsync<int>(customer, CoreGenericAttributes.TicketAttribute +
                                                                   (await _productService.GetProductByIdAsync(shoppingCartItem
                                                                       .ProductId)).Sku);
                    if (totalQuantity > 2)
                    {
                        warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.QuantityOfTicketsExceed"));
                        return warnings;
                    }
                }

                if (resetCheckoutData)
                {
                    //reset checkout data
                    await _customerService.ResetCheckoutDataAsync(customer, shoppingCartItem.StoreId);
                }
                if (quantity > 0)
                {
                    //check warnings
                    warnings.AddRange(await GetShoppingCartItemWarningsAsync(customer, shoppingCartItem.ShoppingCartType,
                        await _productService.GetProductByIdAsync(shoppingCartItem.ProductId), shoppingCartItem.StoreId,
                        attributesXml, customerEnteredPrice,
                        rentalStartDate, rentalEndDate, quantity, false, shoppingCartItemId));
                    if (!warnings.Any())
                    {
                        //if everything is OK, then update a shopping cart item
                        int oldQuantity = shoppingCartItem.Quantity;

                        shoppingCartItem.Quantity = quantity;
                        shoppingCartItem.AttributesXml = attributesXml;
                        shoppingCartItem.CustomerEnteredPrice = customerEnteredPrice;
                        shoppingCartItem.RentalStartDateUtc = rentalStartDate;
                        shoppingCartItem.RentalEndDateUtc = rentalEndDate;
                        shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;
                        await _sciRepository.UpdateAsync(shoppingCartItem);
                        await _customerService.UpdateCustomerAsync(customer);

                        await _logger.InsertLogAsync(LogLevel.Debug, $"Update ShoppingCartItem , ProductId {shoppingCartItem.ProductId}, Quantity {shoppingCartItem.Quantity}", customer: customer);


                        //event notification
                        await _eventPublisher.EntityUpdatedAsync(shoppingCartItem);

                        //Update paired stock items
                        await UpdatePairedStockItems(shoppingCartItem, oldQuantity, quantity);
                    }
                }
                else
                {
                    //delete a shopping cart item
                    await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, true);
                }
            }

            return warnings;
        }

        protected override async Task<IList<string>> GetRequiredProductWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
            int storeId, int quantity, bool addRequiredProducts, int shoppingCartItemId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (product == null)
                throw new ArgumentNullException("product");

            var warnings = new List<string>();

            if (product.RequireOtherProducts)
            {
                var ids = _productService.ParseRequiredProductIds(await _productService.GetProductByIdAsync(product.Id));
                foreach (var id in ids)
                {
                    var RequireProduct = await _productService.GetProductByIdAsync(id);
                    var stockQuantity = await _productService.GetTotalStockQuantityAsync(RequireProduct);
                    if (stockQuantity == 0)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("NopFiluetCommon.Products.Availability.OutOfStock"), RequireProduct.Sku));
                    }
                    else if (quantity > stockQuantity)
                    {
                        warnings.Add(string.Format(await _localizationService.GetResourceAsync("NopFiluetCommon.ShoppingCart.ProductUpdateWarning"), stockQuantity, product.Sku));
                    }
                }

            }
            return warnings;
        }

        private async Task UpdatePairedStockItems(ShoppingCartItem shoppingCartItem, int oldQuantity, int newQuantity)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            int storeId = (await _storeContext.GetCurrentStoreAsync()).Id;
            var product = await _productService.GetProductByIdAsync(shoppingCartItem.ProductId);
            List<string> warnings = new List<string>();
            if (product.RequireOtherProducts)
            {
                var ids = _productService.ParseRequiredProductIds(await _productService.GetProductByIdAsync(shoppingCartItem.ProductId));
                var cart = await GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync());

                foreach (int id in ids)
                {
                    var requiredCartItem = cart.FirstOrDefault(sci => sci.ProductId == id);
                    if (requiredCartItem != null)
                    {
                        warnings.AddRange(await UpdateShoppingCartItemAsync(
                             customer: customer,
                             shoppingCartItemId: requiredCartItem.Id,
                             attributesXml: string.Empty,
                             customerEnteredPrice: decimal.Zero,
                             quantity: requiredCartItem.Quantity + newQuantity - oldQuantity));
                        if (warnings.Any())
                        {
                            var Product = await _productService.GetProductByIdAsync(requiredCartItem.ProductId);
                            string warningMessage = $"Product sku: {Product.Sku}-{string.Join(", ", warnings)}";
                            await _logger.InsertLogAsync(LogLevel.Information, warningMessage, warningMessage, customer);
                        }
                    }
                    else
                    {
                        var requiredProduct = await _productService.GetProductByIdAsync(id);
                        if (requiredProduct != null)
                        {
                            warnings.AddRange(await AddToCartAsync(
                              customer: customer,
                              product: requiredProduct,
                              shoppingCartType: ShoppingCartType.ShoppingCart,
                              storeId: storeId,
                              quantity: newQuantity));
                            if (warnings.Any())
                            {
                                string warningMessage = $"Product Sku: {requiredProduct.Sku}-{string.Join(", ", warnings)}";
                                await _logger.InsertLogAsync(LogLevel.Information, warningMessage, string.Format($"Product SKU : {requiredProduct.Sku} cannot be added into cart due to {string.Join(",", warnings)} which added automaticaly added with Product SKU : {product.Sku}"), customer);
                            }
                        }
                    }
                }
            }
        }

        public override async Task<IList<string>> GetShoppingCartItemWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product, int storeId,
            string attributesXml, decimal customerEnteredPrice, DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null, int quantity = 1, bool addRequiredProducts = true, int shoppingCartItemId = 0,
            bool getStandardWarnings = true, bool getAttributesWarnings = true, bool getGiftCardWarnings = true,
            bool getRequiredProductWarnings = true, bool getRentalWarnings = true)
        {
            if (await (await _categoryChecker.GetCategoriesByProductIdAsync(product.Id))
                .AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) ==
                    CategoryTypeEnum.Ticket || await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(x, CategoryAttributeNames.CategoryType) ==
                    CategoryTypeEnum.Maintenance))
                getStandardWarnings = false;

            return await base.GetShoppingCartItemWarningsAsync(customer, shoppingCartType, product, storeId, attributesXml, customerEnteredPrice, rentalStartDate, rentalEndDate, quantity, addRequiredProducts, shoppingCartItemId, getStandardWarnings, getAttributesWarnings, getGiftCardWarnings, getRequiredProductWarnings, getRentalWarnings);
        }

        // Override Service for The process skips when RequireOtherProducts are not published.
        protected override async Task<IList<string>> GetStandardWarningsAsync(Customer customer, ShoppingCartType shoppingCartType, Product product,
            string attributesXml, decimal customerEnteredPrice, int quantity, int shoppingCartItemId, int storeId)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var warnings = new List<string>();

            //deleted
            if (product.Deleted)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductDeleted"));
                return warnings;
            }

            //published
            if (!product.Published)
            {
                var requireProducts = await _productService.SearchProductsAsync();
                var requireProduct = product.Id.ToString();
                var RequireOtherProducts = requireProducts.Any(x => x.RequiredProductIds?.Contains(requireProduct) == true);
                // The process skips when RequireOtherProducts are not published
                if (!RequireOtherProducts)
                {
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
                }
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                warnings.Add("This is not simple product");
            }

            //ACL
            if (!await _aclService.AuthorizeAsync(product, customer))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
            }

            //Store mapping
            if (!await _storeMappingService.AuthorizeAsync(product, storeId))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.ProductUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.DisableBuyButton)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.BuyingDisabled"));
            }

            //disabled "add to wishlist" button
            if (shoppingCartType == ShoppingCartType.Wishlist && product.DisableWishlistButton)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.WishlistDisabled"));
            }

            //call for price
            if (shoppingCartType == ShoppingCartType.ShoppingCart && product.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceProducts || _workContext.OriginalCustomerIfImpersonated == null))
            {
                warnings.Add(await _localizationService.GetResourceAsync("Products.CallForPrice"));
            }

            //customer entered price
            if (product.CustomerEntersPrice)
            {
                if (customerEnteredPrice < product.MinimumCustomerEnteredPrice ||
                    customerEnteredPrice > product.MaximumCustomerEnteredPrice)
                {
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    var minimumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MinimumCustomerEnteredPrice, currentCurrency);
                    var maximumCustomerEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(product.MaximumCustomerEnteredPrice, currentCurrency);
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CustomerEnteredPrice.RangeError"),
                        await _priceFormatter.FormatPriceAsync(minimumCustomerEnteredPrice, false, false),
                        await _priceFormatter.FormatPriceAsync(maximumCustomerEnteredPrice, false, false)));
                }
            }

            //quantity validation
            var hasQtyWarnings = false;
            if (quantity < product.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MinimumQuantity"), product.OrderMinimumQuantity));
                hasQtyWarnings = true;
            }

            if (quantity > product.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumQuantity"), product.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }

            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

            var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (product.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        //do nothing
                        break;
                    case ManageInventoryMethod.ManageStock:
                        if (product.BackorderMode == BackorderMode.NoBackorders)
                        {
                            var maximumQuantityCanBeAdded = await _productService.GetTotalStockQuantityAsync(product);

                            warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, maximumQuantityCanBeAdded));

                            if (warnings.Any())
                                return warnings;

                            //validate product quantity with non combinable product attributes
                            var productAttributeMappings = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                            if (productAttributeMappings?.Any() == true)
                            {
                                var onlyCombinableAttributes = productAttributeMappings.All(mapping => !mapping.IsNonCombinable());
                                if (!onlyCombinableAttributes)
                                {
                                    var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                                    var totalAddedQuantity = cart
                                        .Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId)
                                        .Sum(product => product.Quantity);

                                    totalAddedQuantity += quantity;

                                    //counting a product into bundles
                                    foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                    {
                                        var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                        foreach (var attributeValue in attributeValues)
                                        {
                                            if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                                totalAddedQuantity += bundle.Quantity * attributeValue.Quantity;
                                        }
                                    }

                                    warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalAddedQuantity, maximumQuantityCanBeAdded));
                                }
                            }

                            if (warnings.Any())
                                return warnings;

                            //validate product quantity and product quantity into bundles
                            if (string.IsNullOrEmpty(attributesXml))
                            {
                                var cart = await GetShoppingCartAsync(customer, shoppingCartType, storeId);
                                var totalQuantityInCart = cart.Where(item => item.ProductId == product.Id && item.Id != shoppingCartItemId && string.IsNullOrEmpty(item.AttributesXml))
                                    .Sum(product => product.Quantity);

                                totalQuantityInCart += quantity;

                                foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                {
                                    var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(bundle.AttributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToProduct && attributeValue.AssociatedProductId == product.Id)
                                            totalQuantityInCart += bundle.Quantity * attributeValue.Quantity;
                                    }
                                }

                                warnings.AddRange(await GetQuantityProductWarningsAsync(product, totalQuantityInCart, maximumQuantityCanBeAdded));
                            }
                        }

                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributesXml);
                        if (combination != null)
                        {
                            //combination exists
                            //let's check stock level
                            if (!combination.AllowOutOfStockOrders)
                                warnings.AddRange(await GetQuantityProductWarningsAsync(product, quantity, combination.StockQuantity));
                        }
                        else
                        {
                            //combination doesn't exist
                            if (product.AllowAddingOnlyExistingAttributeCombinations)
                            {
                                //maybe, is it better  to display something like "No such product/combination" message?
                                var productAvailabilityRange = await _dateRangeService.GetProductAvailabilityRangeByIdAsync(product.ProductAvailabilityRangeId);
                                var warning = productAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                                    : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                                        await _localizationService.GetLocalizedAsync(productAvailabilityRange, range => range.Name));
                                warnings.Add(warning);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //availability dates
            var availableStartDateError = false;
            if (product.AvailableStartDateTimeUtc.HasValue)
            {
                var availableStartDateTime = DateTime.SpecifyKind(product.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
                {
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }

            if (!product.AvailableEndDateTimeUtc.HasValue || availableStartDateError)
                return warnings;

            var availableEndDateTime = DateTime.SpecifyKind(product.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
            if (availableEndDateTime.CompareTo(DateTime.UtcNow) < 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
            }

            return warnings;
        }

        #endregion
    }
}
