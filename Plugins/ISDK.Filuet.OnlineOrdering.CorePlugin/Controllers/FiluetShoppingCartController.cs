using ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Infrastructure.Cash;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.ShoppingCart;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Components;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Models.Media;
using Nop.Web.Models.ShoppingCart;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    /// <summary>
    /// ExtendedShoppingCartController
    /// </summary>
    public class FiluetShoppingCartController : ShoppingCartController
    {
        #region Fields

        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly FusionValidationService _fusionValidationService;
        private readonly IFiluetShippingCartService _filuetShippingCartService;
        private readonly IWebHelper _webHelper;
        private readonly MediaSettings _mediaSettings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly FiluetCorePluginSettings _filuetCorePluginSettings;
        private readonly IDistributorService _distributorService;
        private readonly INopUrlHelper _nopUrlHelper;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ShoppingCartSettings _shoppingCartSettings;


        #endregion

        #region Ctor

        public FiluetShoppingCartController(
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            INopFileProvider fileProvider,
            INopUrlHelper nopUrlHelper,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            IShoppingCartService shoppingCartService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings,
            ShippingSettings shippingSettings,
            IPriceCalculationService priceCalculationService,
            ILogger logger,
            IStaticCacheManager cacheManager,
            FusionValidationService fusionValidationService,
            IFiluetShippingCartService filuetShippingCartService,
            IOrderService orderService,
            IDualMonthsService dualMonthsService,
            FiluetCorePluginSettings filuetCorePluginSettings,
            IDistributorService distributorService)
            : base(
                  captchaSettings,
                  customerSettings,
                  checkoutAttributeParser,
                  checkoutAttributeService,
                  currencyService,
                  customerActivityService,
                  customerService,
                  discountService,
                  downloadService,
                  genericAttributeService,
                  giftCardService,
                  htmlFormatter,
                  localizationService,
                  fileProvider,
                  nopUrlHelper,
                  notificationService,
                  permissionService,
                  pictureService,
                  priceFormatter,
                  productAttributeParser,
                  productAttributeService,
                  productService,
                  shippingService,
                  shoppingCartModelFactory,
                  shoppingCartService,
                  staticCacheManager,
                  storeContext,
                  taxService,
                  urlRecordService,
                  webHelper,
                  workContext,
                  workflowMessageService,
                  mediaSettings,
                  orderSettings,
                  shoppingCartSettings,
                  shippingSettings)
        {
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _productService = productService;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _productAttributeService = productAttributeService;
            _productAttributeParser = productAttributeParser;
            _taxService = taxService;
            _currencyService = currencyService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _fusionValidationService = fusionValidationService;
            _filuetShippingCartService = filuetShippingCartService;
            _webHelper = webHelper;
            _mediaSettings = mediaSettings;
            _logger = logger;
            _orderService = orderService;
            _dualMonthsService = dualMonthsService;
            _filuetCorePluginSettings = filuetCorePluginSettings;
            _distributorService = distributorService;
            _priceCalculationService = priceCalculationService;
            _logger = logger;
            _cacheManager = cacheManager;
            _fusionValidationService = fusionValidationService;
            _filuetShippingCartService = filuetShippingCartService;
            _orderService = orderService;
            _dualMonthsService = dualMonthsService;
            _filuetCorePluginSettings = filuetCorePluginSettings;
            _distributorService = distributorService;
            _nopUrlHelper = nopUrlHelper;
            _urlRecordService = urlRecordService;
            _customerActivityService = customerActivityService;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Methods

        [JetBrains.Annotations.AspMvcSuppressViewError]
        public override async Task<IActionResult> Cart()
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            FiluetShoppingCartModel model = null;
            var isDebtor = await PrepareDebtorCart(currentCustomer);
            IActionResult baseResult = await base.Cart();
            //List<ShoppingCartItem> cart = GetUserCart();
            var cart = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), ShoppingCartType.ShoppingCart, store.Id);
            ShoppingCartModel baseModel = (ShoppingCartModel)((ViewResult)baseResult).Model;
            string baseSerialized = JsonConvert.SerializeObject(baseModel);
            model = JsonConvert.DeserializeObject<FiluetShoppingCartModel>(baseSerialized);

            IResidentChecker residentChecker = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                residentChecker = serviceScope.ServiceProvider.GetService<IResidentChecker>();
            }

            model.IsNotResident = residentChecker?.NeedToCheckIfAuthenticatedCustomerIsResident == true && (await _customerService.GetCustomerRolesAsync(currentCustomer)).Any(cr =>
                cr.SystemName == CommonConstants.IsNotResidentCustomerRole);
            if (model.IsNotResident)
            {
                var landingToken = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CommonConstants.LandingToken) ?? Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                model.LandingToken = landingToken;
                await _genericAttributeService.SaveAttributeAsync(currentCustomer, CommonConstants.LandingToken, landingToken);
            }
            model.Errors = new List<string>();
            model.ShowApfPayMessage = isDebtor;
            var volumePoints = (await Task.WhenAll(cart.Select(async x =>
            {
                var product = await _productService.GetProductByIdAsync(x.ProductId);
                var volumePoints = await _genericAttributeService.GetAttributeAsync<double>(product, ProductAttributeNames.VolumePoints);
                return x.Quantity * volumePoints;
            }))) .Sum();

            var distributorLimits = await _fusionValidationService.ValidateDistributorLimitsAsync(currentCustomer, cart, volumePoints);
            if (!distributorLimits.IsValid)
            {
                string warningMessage = string.Format(await ErrorMessages.OrderLimitExceedEditCart.ToLocalizedStringAsync(), distributorLimits.ExceedanceAmount.HasValue ? distributorLimits.ExceedanceAmount.Value : 0);
                model.Errors.Add(warningMessage);
                model.CartIsValid = false;
            }

            var isCartValid = await _filuetShippingCartService.IsCartValid(currentCustomer, cart.ToList(), (_filuetCorePluginSettings.MonthVpLimit, _filuetCorePluginSettings.OneOrderVpLimit));

            model.IsCartValid = isCartValid;

            var lastOrder = (await _orderService.SearchOrdersAsync(
                    storeId: (await _storeContext.GetCurrentStoreAsync()).Id,
                    customerId: currentCustomer.Id))
                .FirstOrDefault();

            if (lastOrder != null && lastOrder.PaymentStatus != Nop.Core.Domain.Payments.PaymentStatus.Paid)
            {
                var wasRepeatOrderDialogShown = await _genericAttributeService.GetAttributeAsync<bool>(lastOrder, OrderAttributeNames.WasRepeatOrderDialogShown);
                if (!wasRepeatOrderDialogShown)
                {
                    model.ShowRepeatOrderDialog = true;
                    model.LastOrderId = lastOrder.Id;

                    await _genericAttributeService.SaveAttributeAsync(lastOrder, OrderAttributeNames.WasRepeatOrderDialogShown, true);
                }
            }

            return View(model);
        }

        [HttpPost, ActionName("Cart")]
        [FormValueRequired("checkout")]
        public override async Task<IActionResult> StartCheckout(IFormCollection form)
        {
            if (form.ContainsKey("OrderMonth") && await _dualMonthsService.GetDualMonthAllowedAsync())
            {
                var selectedLimitsMonthTimestamp = long.Parse(form["OrderMonth"]);
                var monthDate = FromUnixTimestamp(selectedLimitsMonthTimestamp);
                await _dualMonthsService.UpdateSelectedLimitsAsync(await _workContext.GetCurrentCustomerAsync(), monthDate);
            }

            return await base.StartCheckout(form);
        }

        private DateTime FromUnixTimestamp(long ut)
        {
            DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            start = start.AddSeconds(ut);
            return start;
        }

        [JetBrains.Annotations.AspMvcSuppressViewError]
        // handle cart update to perform required Fusion checks         
        [HttpPost, ActionName("Cart")]
        [FormValueRequired("updatecart")]
        public override async Task<IActionResult> UpdateCart(IFormCollection form)
        {
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();

            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(currentCustomer);

            var cart = shoppingCart.Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart).ToList();

            Func<double> getVP = () =>
            {
                var volumePoints = cart.Sum(x => x.Quantity * _genericAttributeService.GetAttributeAsync<double>(_productService.GetProductByIdAsync(x.ProductId).Result, ProductAttributeNames.VolumePoints).Result);
                return volumePoints;
            };

            Func<int> getQuantity = () =>
            {
                return cart
                    .Sum(x => x.Quantity);
            };

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart))
            {
                return RedirectToRoute("HomePage");
            }

            try
            {
                List<int> idsToRemove = !string.IsNullOrEmpty(form["removefromcart"])
                    ? ((string)form["removefromcart"]).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList()
                    : new List<int>();

                // current warnings <cart item identifier, warnings>
                var innerWarnings = new Dictionary<int, IList<string>>();
                var modifiedOrderItems = new List<OrderItemModel>();

                List<ShoppingCartItem> newCart = new();

                var profile = _distributorService.GetDistributorDetailedProfileAsync(currentCustomer);

                foreach (ShoppingCartItem sci in cart)
                {
                    bool remove = idsToRemove.Contains(sci.Id);
                    if (remove)
                    {
                        var apfProduct = await ApfApfShoppingCartHelper.GetApfProductAsync(currentCustomer);
                        if (sci.ProductId == apfProduct?.Id && !(await currentCustomer.IsDebtorAsync()))
                        {
                            await _genericAttributeService.SaveAttributeAsync(currentCustomer,
                                CoreGenericAttributes.ApfMessageAcceptedAttribute, false);
                        }
                        await _shoppingCartService.DeleteShoppingCartItemAsync(sci, ensureOnlyActiveCheckoutAttributes: true);
                    }
                    else
                    {
                        var formKey = $"itemquantity{sci.Id}";

                        if (form.Keys.Contains(formKey))
                        {
                            int oldQuantity = sci.Quantity;
                            if (int.TryParse(form[formKey], out int newQuantity))
                            {
                                var currSciWarnings = await _shoppingCartService.UpdateShoppingCartItemAsync(currentCustomer,
                                   sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                                   sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                                   newQuantity, true);

                                if (currSciWarnings.Count == 0)
                                {
                                    sci.Quantity = newQuantity;
                                }

                                innerWarnings.Add(sci.Id, currSciWarnings);

                                break;
                            }
                        }
                    }
                }

                await PrepareDebtorCart(currentCustomer);


                // if quantity changed
                FiluetShoppingCartModel model = null;
                // check the limits

                double volumePoints = getVP();
                int quantity = getQuantity();

                DistributorLimitsModel distributorLimits = null;

                distributorLimits = await _fusionValidationService.ValidateDistributorLimitsAsync(currentCustomer, cart, volumePoints);

                if (!distributorLimits.IsValid)
                {
                    string warningMessage = string.Format(await ErrorMessages.OrderLimitExceedEditCart.ToLocalizedStringAsync(), distributorLimits.ExceedanceAmount.HasValue ? distributorLimits.ExceedanceAmount.Value : 0);
                    model = await GetShoppingCartModelWithMessages(new List<string>(), new List<string> { warningMessage }, cart);
                    model.CartIsValid = false;
                }

                if (model == null)
                {

                    // parse and save checkout attributes
                    await ParseAndSaveCheckoutAttributesAsync(cart, form);

                    var baseModel = new ShoppingCartModel();

                    await _logger.InformationAsync("test6", null, currentCustomer);

                    baseModel = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(baseModel, cart);

                    // update current warnings
                    foreach (var kvp in innerWarnings)
                    {
                        var sciId = kvp.Key;
                        var warnings = kvp.Value;

                        // find model
                        var sciModel = baseModel.Items.FirstOrDefault(x => x.Id == sciId);
                        if (sciModel != null)
                        {
                            foreach (string warning in warnings)
                            {
                                if (!sciModel.Warnings.Contains(warning))
                                {
                                    sciModel.Warnings.Add(warning);
                                }
                            }
                        }
                    }

                    model = PluginMapper.Mapper.Map<FiluetShoppingCartModel>(baseModel);
                }

                model.IsCartValid = await _filuetShippingCartService.IsCartValid(currentCustomer, cart, (_filuetCorePluginSettings.MonthVpLimit, _filuetCorePluginSettings.OneOrderVpLimit));
                return Json(model);
            }
            catch (Exception exc)
            {
                await _logger.ErrorAsync("[FiluetShoppingCart] Cart()/Update() Error.", exc);

                return RedirectToRoute("HomePage");
            }
        }

        public async Task<IActionResult> ClearCart()
        {
            (await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync())).ForEach(async sci => await _shoppingCartService.DeleteShoppingCartItemAsync(sci));
            await _customerService.ResetCheckoutDataAsync(await _workContext.GetCurrentCustomerAsync(), _storeContext.GetCurrentStore().Id);
            await _customerService.UpdateCustomerAsync(await _workContext.GetCurrentCustomerAsync());
            return Redirect(_webHelper.GetStoreLocation());
        }

        [HttpPost]
        public async Task<IActionResult> AddApfToCart()
        {
            var result = ApfApfShoppingCartHelper.ReplaceShoppingCartWithApfItemAsync(await _workContext.GetCurrentCustomerAsync());

            return new JsonResult(result);
        }

        public override async Task<IActionResult> ProductDetails_AttributeChange(int productId, bool validateAttributeConditions,
            bool loadPicture, IFormCollection form)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return new NullJsonResult();

            var errors = new List<string>();
            string attributeXml = await _productAttributeParser.ParseProductAttributesAsync(product, form, errors);

            // rental attributes
            DateTime? rentalStartDate = null;
            DateTime? rentalEndDate = null;
            if (product.IsRental)
            {
                _productAttributeParser.ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
            }

            // sku, mpn, gtin, vp
            var formatProductAttributes = product.FormatProductAttributes(attributeXml, _productAttributeParser);
            var combination = await _productAttributeParser.FindProductAttributeCombinationAsync(product, attributeXml);

            // price
            string price = string.Empty;
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices) && !product.CustomerEntersPrice)
            {
                // we do not calculate price of "customer enters price" option is enabled
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                var finalPrice = await _priceCalculationService.GetFinalPriceAsync(product,
                    await _workContext.GetCurrentCustomerAsync(), currentStore, combination?.OverriddenPrice, 0, true, 1, rentalStartDate, rentalEndDate);
                //decimal taxRate;
                var finalPriceWithDiscountBase = await _taxService.GetProductPriceAsync(product, finalPrice.finalPrice);
                decimal finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase.price, await _workContext.GetWorkingCurrencyAsync());
                price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
            }

            // stock
            var stockAvailability = await _productService.FormatStockMessageAsync(product, attributeXml);

            // conditional attributes
            var enabledAttributeMappingIds = new List<int>();
            var disabledAttributeMappingIds = new List<int>();
            if (validateAttributeConditions)
            {
                var attributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
                foreach (var attribute in attributes)
                {
                    var conditionMet = await _productAttributeParser.IsConditionMetAsync(attribute, attributeXml);
                    if (conditionMet.HasValue)
                    {
                        if (conditionMet.Value)
                            enabledAttributeMappingIds.Add(attribute.Id);
                        else
                            disabledAttributeMappingIds.Add(attribute.Id);
                    }
                }
            }

            // picture. used when we want to override a default product picture when some attribute is selected
            var pictureFullSizeUrl = "";
            var pictureDefaultSizeUrl = "";
            if (loadPicture)
            {
                // just load (return) the first found picture (in case if we have several distinct attributes with associated pictures)
                // actually we're going to support pictures associated to attribute combinations (not attribute values) soon. it'll more flexible approach
                var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributeXml);
                var attributeValueWithPicture = attributeValues.FirstOrDefault(x => x.PictureId > 0);
                if (attributeValueWithPicture != null)
                {
                    var productAttributePictureCacheKey = string.Format(FiluetNopModelCacheDefaults.PRODUCTATTRIBUTE_PICTURE_MODEL_KEY,
                                    attributeValueWithPicture.PictureId,
                                    _webHelper.IsCurrentConnectionSecured(),
                                    _storeContext.GetCurrentStore().Id);
                    var pictureModel = await _cacheManager.Get(new CacheKey(productAttributePictureCacheKey), async () =>
                    {
                        var valuePicture = await _pictureService.GetPictureByIdAsync(attributeValueWithPicture.PictureId);
                        return valuePicture == null ? new PictureModel() :
                            new PictureModel
                            {
                                FullSizeImageUrl = (await _pictureService.GetPictureUrlAsync(valuePicture)).Url,
                                ImageUrl = (await _pictureService.GetPictureUrlAsync(valuePicture, _mediaSettings.ProductDetailsPictureSize)).Url
                            };
                    });
                    pictureFullSizeUrl = pictureModel.FullSizeImageUrl;
                    pictureDefaultSizeUrl = pictureModel.ImageUrl;
                }
            }

            // vp
            decimal? vp = formatProductAttributes.Vp;
            string vpStr = vp.HasValue ? string.Format(
                await _localizationService.GetResourceAsync("ISDK.Filuet.OnlineOrdering.ShoppingPlugin.Resources.VolumePoints"), vp)
                : string.Empty;

            return Json(new
            {
                gtin = formatProductAttributes.Gtin,
                mpn = formatProductAttributes.ManufacturerPartNumber,
                sku = formatProductAttributes.Sku,
                vp = vpStr,
                price = price,
                stockAvailability = stockAvailability,
                enabledattributemappingids = enabledAttributeMappingIds.ToArray(),
                disabledattributemappingids = disabledAttributeMappingIds.ToArray(),
                pictureFullSizeUrl = pictureFullSizeUrl,
                pictureDefaultSizeUrl = pictureDefaultSizeUrl,
                message = errors.Any() ? errors.ToArray() : null
            });
        }

        private async Task<bool> PrepareDebtorCart(Customer customer)
        {
            bool isDeptor = await customer.IsDebtorAsync();

            if (isDeptor || await _genericAttributeService.GetAttributeAsync<bool>(customer,CoreGenericAttributes.ApfMessageAcceptedAttribute))
            {
                var replaced = await ApfApfShoppingCartHelper.ReplaceShoppingCartWithApfItemAsync(customer);
                if (!replaced)
                {
                    await _logger.ErrorAsync("Customer is a debtor but APF product is missed.");
                }
            }
            else
            {
                await ApfApfShoppingCartHelper.RemoveApfItemAsync(customer);
            }
            return isDeptor;
        }

        private async Task<FiluetShoppingCartModel> GetShoppingCartModelWithMessages(IList<string> errors, IList<string> warnings, IList<ShoppingCartItem> cart)
        {
            var model = new FiluetShoppingCartModel();

            ShoppingCartModel baseModel = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(model, cart);
            string baseSerialized = JsonConvert.SerializeObject(baseModel);
            model = JsonConvert.DeserializeObject<FiluetShoppingCartModel>(baseSerialized);

            model.Warnings = warnings == null ? new List<string>() : warnings;
            model.Errors = errors == null ? new List<string>() : errors;
            return await Task.FromResult(model);
        }


        //private List<ShoppingCartItem> GetUserCart()
        //{
        //    List<ShoppingCartItem> cart = _shoppingCartService.GetShoppingCartAsync(_workContext.GetCurrentCustomerAsync().Result).Result
        //        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //        .ToList();
        //    return cart;
        //}

        //private void DeleteFromUserCart(IEnumerable<StockBalanceItemModel> items, out IList<string> deleted, out IList<string> reduced)
        //{
        //    var shoppingCartItems = GetUserCart();
        //    deleted = new List<string>();
        //    reduced = new List<string>();
        //    foreach (var item in items)
        //    {
        //        var shoppingCartItem = shoppingCartItems.FirstOrDefault(sci => _productService.GetProductByIdAsync(sci.ProductId).Result.Sku == item.Sku.Name);
        //        if (shoppingCartItem != null)
        //        {
        //            if (item.StockQty == 0)
        //            {
        //                _shoppingCartService.DeleteShoppingCartItemAsync(shoppingCartItem);
        //                deleted.Add(item.Sku.Name);
        //            }
        //            else
        //            {
        //                shoppingCartItem.Quantity = item.StockQty;
        //                _shoppingCartService.UpdateShoppingCartItemAsync(_workContext.GetCurrentCustomerAsync().Result, shoppingCartItem.Id, shoppingCartItem.AttributesXml, shoppingCartItem.CustomerEnteredPrice);
        //                reduced.Add(item.Sku.Name);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region For Custom Change

        //add product to cart using AJAX
        //currently we use this method on catalog pages (category/manufacturer/etc)
        [HttpPost]
        public override async Task<IActionResult> AddProductToCart_Catalog(int productId, int shoppingCartTypeId,
            int quantity, bool forceredirection = false)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog.No product found with the specified ID [ {productId}]");
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });
            }

            var redirectUrl = await _nopUrlHelper.RouteGenericUrlAsync<Product>(new { SeName = await _urlRecordService.GetSeNameAsync(product) });

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog. ProductType is not ProductType.SimpleProduct ID [ {productId}]");
                return Json(new
                {
                    redirect = redirectUrl,
                    message = "ProductType is not simple"
                });

            }
            //products with "minimum order quantity" more than a specified qty
            if (product.OrderMinimumQuantity > quantity)
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog. Product with 'minimum order quantity' ID [ {productId}]");
                //we cannot add to the cart such products from category pages
                //it can confuse customers. That's why we redirect customers to the product details page
                return Json(new
                {
                    redirect = redirectUrl,
                    message = "products with 'minimum order quantity' more than a specified qty"
                });
            }

            if (product.CustomerEntersPrice)
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog.cannot be added to the cart (requires a customer to enter price) ID [ {productId}]");
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = redirectUrl,
                    message = "Requires customer enter price"
                });
            }

            if (product.IsRental)
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog IsRental ID [ {productId}]");
                //rental products require start/end dates to be entered
                return Json(new
                {
                    redirect = redirectUrl,
                    message = "rental products require start/end dates to be entered"
                });
            }

            var allowedQuantities = _productService.ParseAllowedQuantities(product);
            if (allowedQuantities.Length > 0)
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog allowedQuantities ID [ {productId}]");
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = redirectUrl,
                    message = "cannot be added to the cart (requires a customer to select a quantity from dropdownlist)"
                });

            }

            //allow a product to be added to the cart when all attributes are with "read-only checkboxes" type
            var productAttributes = await _productAttributeService.GetProductAttributeMappingsByProductIdAsync(product.Id);
            if (productAttributes.Any(pam => pam.AttributeControlType != AttributeControlType.ReadonlyCheckboxes))
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog productAttributes ID [ {productId}]");
                //product has some attributes. let a customer see them
                return Json(new
                {
                    redirect = redirectUrl,
                    message = "allow a product to be added to the cart when all attributes are with 'read - only checkboxes' type"
                });
            }

            //creating XML for "read-only checkboxes" attributes
            var attXml = await productAttributes.AggregateAwaitAsync(string.Empty, async (attributesXml, attribute) =>
            {
                var attributeValues = await _productAttributeService.GetProductAttributeValuesAsync(attribute.Id);
                foreach (var selectedAttributeId in attributeValues
                    .Where(v => v.IsPreSelected)
                    .Select(v => v.Id)
                    .ToList())
                {
                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                        attribute, selectedAttributeId.ToString());
                }

                return attributesXml;
            });

            //get standard warnings without attribute validations
            //first, try to find existing shopping cart item
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, cartType, store.Id);
            var shoppingCartItem = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(cart, cartType, product);
            //if we already have the same product in the cart, then use the total quantity to validate
            var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
            var addToCartWarnings = await _shoppingCartService
                .GetShoppingCartItemWarningsAsync(customer, cartType,
                product, store.Id, string.Empty,
                decimal.Zero, null, null, quantityToValidate, false, shoppingCartItem?.Id ?? 0, true, false, false, false);
            if (addToCartWarnings.Any())
            {
                await _logger.InformationAsync($"AddProductToCart_Catalog productId {product.Id} addToCartWarnings {string.Join(';', addToCartWarnings)}");
                //cannot be added to the cart
                //let's display standard warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //now let's try adding product to the cart (now including product attribute validation, etc)
            addToCartWarnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                product: product,
                shoppingCartType: cartType,
                storeId: store.Id,
                attributesXml: attXml,
                quantity: quantity);
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart
                //but we do not display attribute and gift card warnings here. let's do it on the product details page
                var warnings = string.Join(";\n", addToCartWarnings.Distinct());
                var warningMessage = await _localizationService.GetResourceAsync("ShoppingCartController.AddToShoppingCart.Warnings");
                return Json(new
                {
                    redirect = redirectUrl,
                    message = $"{warningMessage}: {warnings}"
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        await _customerActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            await _logger.InformationAsync($"AddProductToCart_Catalog.addToCartWarnings.Wishlist");
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                                message = "redirect to the wishlist page"
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id);

                        var updatetopwishlistsectionhtml = string.Format(await _localizationService.GetResourceAsync("Wishlist.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));
                        return Json(new
                        {
                            success = true,
                            message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        await _customerActivityService.InsertActivityAsync("PublicStore.AddToShoppingCart",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            await _logger.InformationAsync($"AddProductToCart_Catalog.addToCartWarnings.shoppingcartpage");
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                                message = "redirect to the shopping cart page"
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

                        var updatetopcartsectionhtml = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? await RenderViewComponentToStringAsync(typeof(FlyoutShoppingCartViewComponent))
                            : string.Empty;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml
                        });
                    }
            }
        }

        #endregion
    }
}