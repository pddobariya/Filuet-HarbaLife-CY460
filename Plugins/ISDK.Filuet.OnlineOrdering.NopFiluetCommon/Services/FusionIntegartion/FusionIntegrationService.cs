using Filuet.Hrbl.Ordering.Abstractions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.CustomCategory;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion
{
    public class FusionIntegrationService : IFusionIntegrationService
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly ILogger _logger;
        private readonly IShoppingCartProxyService _shoppingCartProxyService;
        private readonly IStockBalanceProxyService _stockBalanceProxyService;
        private readonly IOrderService _orderService;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly IWorkContext _workContext;
        private readonly IDistributorService _distributorService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IDefaultShippingSettingsService _defaultShippingSettingsService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICategoryChecker _categoryChecker;
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _categoryService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly TaxSettings _taxSettings;
        private readonly IAddressService _addressService;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        #endregion

        #region Ctor

        public FusionIntegrationService(
            IGenericAttributeService genericAttributeService,
            IDualMonthsService dualMonthsService,
            ILogger logger, 
            IShoppingCartProxyService shoppingCartProxyService,
            IStockBalanceProxyService stockBalanceProxyService,
            IOrderService orderService,
            IFiluetShippingService filuetShippingService,
            IWorkContext workContext, 
            IDistributorService distributorService,
            ISettingService settingService, 
            IStoreContext storeContext,
            IProductService productService, 
            IDefaultShippingSettingsService defaultShippingSettingsService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICategoryChecker categoryChecker,
            ICustomerService customerService, 
            ICategoryService categoryService, 
            IShoppingCartService shoppingCartService, 
            TaxSettings taxSettings,
            IAddressService addressService,
            IRepository<GenericAttribute> genericAttributeRepository)
        {
            _genericAttributeService = genericAttributeService;
            _dualMonthsService = dualMonthsService;
            _logger = logger;
            _shoppingCartProxyService = shoppingCartProxyService;
            _stockBalanceProxyService = stockBalanceProxyService;
            _orderService = orderService;
            _filuetShippingService = filuetShippingService;
            _workContext = workContext;
            _distributorService = distributorService;
            _settingService = settingService;
            _storeContext = storeContext;
            _productService = productService;
            _defaultShippingSettingsService = defaultShippingSettingsService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _categoryChecker = categoryChecker;
            _customerService = customerService;
            _categoryService = categoryService;
            _shoppingCartService = shoppingCartService;
            _taxSettings = taxSettings;
            _addressService = addressService;
            _genericAttributeRepository = genericAttributeRepository;
        }
        #endregion

        #region Methods

        private async Task<FusionServiceParamsModel> GetFusionCallParamsAsync(Customer customer, Order order = null)
        {
            var shippingCalcOpt = await _filuetShippingService.GetSelectedShippingComputationOptionModelAsync(customer, order);
            var orderCategory = await GetOrderSpecificCategoryAsync(customer, order);
            var distributorDetailedProfile = await _distributorService.GetDistributorDetailedProfileAsync(customer);

            bool? invoiceWithShipment = await _genericAttributeService.HasCustomAttributeAsync(customer, CustomerAttributeNames.IsShipInvoiceWithOrder)
                ? await _genericAttributeService.GetAttributeAsync<bool?>(customer,CustomerAttributeNames.IsShipInvoiceWithOrder) : null;
            string orderMonthStr = order == null ? await _dualMonthsService.GetOrderMonthOfCustomerAsync(customer) : await _dualMonthsService.GetOrderMonthOfOrderAsync(order);
            string shipCity = shippingCalcOpt.City ?? distributorDetailedProfile?.Addresses?.MailingAddress?.City;
            string shipZip = shippingCalcOpt.ShippingZipCode ?? await _genericAttributeService.GetAttributeAsync<string>(customer,OrderAttributeNames.SelectedShippingZipCode);//shippingCalcOpt.ShippingZipCode ?? distributorDetailedProfile?.Addresses?.MailingAddress?.PostalCode;
            string shipAddress = shippingCalcOpt.Address ?? distributorDetailedProfile?.Addresses?.MailingAddress?.FullAddress;
            string shipPostamat = await _genericAttributeService.GetAttributeAsync<string>(customer,NopCustomerDefaults.SelectedPickupPointAttribute);
            var shipTime = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.DeliveryTime);

            string recentOrderNumber = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.RecentGetShoppingCartTotalOrderNumber);
            var fullName =
             await _genericAttributeService.GetAttributeAsync<string>(customer,
                    CustomerAttributeNames.SelectedShippingFullname);

            return new FusionServiceParamsModel
            {
                DistributorId = await customer.GetDistributorIdAsync(),
                DsType = !string.IsNullOrEmpty(distributorDetailedProfile?.DsType) && distributorDetailedProfile?.DsType.ToLower() == "pm" ? "PC" : string.Empty,
                ProcessingLocation = string.IsNullOrWhiteSpace(shippingCalcOpt.ProcessingLocationCode) ? null : shippingCalcOpt.ProcessingLocationCode,
                WarehouseCode = string.IsNullOrWhiteSpace(shippingCalcOpt.WarehouseCode) ? null : shippingCalcOpt.WarehouseCode,
                FreightCode = string.IsNullOrWhiteSpace(shippingCalcOpt.FreightCode) ? null : shippingCalcOpt.FreightCode,
                OrderMonth = string.IsNullOrWhiteSpace(orderMonthStr) ? null : orderMonthStr,
                CountryCode = string.IsNullOrWhiteSpace(shippingCalcOpt.CountryCode) ? null : shippingCalcOpt.CountryCode.ToCountryCode(),
                City = string.IsNullOrWhiteSpace(shipCity) ? null : shipCity,
                PostalCode = string.IsNullOrWhiteSpace(shipZip) ? "." : shipZip,
                Address = string.IsNullOrWhiteSpace(shipAddress) ? null : shipAddress,
                PostamatId = string.IsNullOrWhiteSpace(shipPostamat) ? null : shipPostamat,
                TimeSlot = string.IsNullOrWhiteSpace(shipTime) ? null : shipTime,
                CustomerName = fullName ?? await customer.GetFullNameAsync(),
                OrderNumber = string.IsNullOrWhiteSpace(recentOrderNumber) ? null : recentOrderNumber,
                OrderTypeCode = orderCategory,
                OrderType = OrderTypes.IE,
                OrderTypeId = GetOrderType(orderCategory, shippingCalcOpt.CountryCode, GetOrderTypeId(shippingCalcOpt), customer),
                InvoiceWithShipment = invoiceWithShipment
            };
        }

        public async Task<ShoppingCartTotalModel> GetShoppingCartTotalAsync(Customer customer, Order order)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);
            IEnumerable<OrderItem> newOrderItems = order != null ? orderItems : null;
            if (newOrderItems == null || !newOrderItems.Any())
            {
                return new ShoppingCartTotalModel();
            }

            FusionServiceParamsModel parameters = await  GetFusionCallParamsAsync(customer, order);
            return  await GetShoppingCartTotal(parameters, customer,await newOrderItems.ToOrderItemModelListAsync(parameters.WarehouseCode));
        }

        public async Task<ShoppingCartTotalModel> GetShoppingCartTotalAsync(Customer customer, IEnumerable<ShoppingCartItem> cartItems = null)
        {
            if (cartItems == null)
            {
                cartItems = await _shoppingCartService.GetShoppingCartAsync(customer);
            }
            if (cartItems == null || !cartItems.Any())
            {
                return new ShoppingCartTotalModel();
            }
            FusionServiceParamsModel parameters = await GetFusionCallParamsAsync(customer);
            return await GetShoppingCartTotal(parameters, customer, await cartItems.ToOrderItemModelListAsync(parameters.WarehouseCode));
        }

        private async Task<ShoppingCartTotalModel> GetShoppingCartTotal(FusionServiceParamsModel parameters, Customer customer, List<OrderItemModel> orderItems)
        {
            var store = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(store);
            if (!string.IsNullOrWhiteSpace(parameters.OrderNumber))
            {
                var olderOrders = await _orderService.SearchOrdersAsync(0, 0, customer.Id);
                if (await olderOrders.AnyAwaitAsync(async x => await _genericAttributeService.GetAttributeAsync<string>(x, OrderAttributeNames.FusionOrderNumber) == parameters.OrderNumber))
                {
                    parameters.OrderNumber = null;
                    await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.RecentGetShoppingCartTotalOrderNumber, "");
                }
            }

            var outParams = new FusionServiceParamsOut(parameters, orderItems);

            await _logger.InsertLogAsync(LogLevel.Information,
                $"[PERF DEBUG] GetShoppingCartTotal request. CustomerId={customer.Id}",
                JsonConvert.SerializeObject(outParams));

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            ShoppingCartTotalModel res = await _shoppingCartProxyService.GetShoppingCartTotalAsync(parameters.DistributorId,
                parameters.ProcessingLocation ?? settings.ProcessingLocationCode,
                parameters.WarehouseCode ?? settings.Warehouse,
                parameters.OrderMonth,
                parameters.FreightCode ?? "PU",
                parameters.CountryCode ?? settings.CountryCode,
                parameters.PostalCode,
                parameters.Address,
                parameters.City,
                orderItems,
                parameters.OrderNumber,
                parameters.OrderTypeCode,
                parameters.OrderType,
                settings.CurrencyCode.ToString());

            stopwatch.Stop();

            if (orderItems.Count() == res.ShoppingCartLines.Count())
            {
                foreach (var shoppingCartLine in res.ShoppingCartLines)
                {
                    var orderItem = orderItems.Where(oi => oi.Sku.Name == shoppingCartLine.Sku.Name).FirstOrDefault();

                    if (orderItem == null)
                        await _logger.InsertLogAsync(LogLevel.Error, $"GetShoppingCartTotal response. CustomerId={customer.Id}.", $"Wrong orderItem:{JsonConvert.SerializeObject(shoppingCartLine)}", customer);
                    if (orderItem != null && orderItem.Count != shoppingCartLine.Count)
                    {
                        await _logger.InsertLogAsync(LogLevel.Error,
                            "GetShoppingCartTotal response. CustomerId={customer.Id}.",
                            $"Wrong count OrderItem.Count= {orderItem.Count} and CartLine.Count= {shoppingCartLine.Count}.{JsonConvert.SerializeObject(shoppingCartLine)}", customer);
                    }
                }
            }
            else
            {
                await _logger.InsertLogAsync(LogLevel.Error,
                    "GetShoppingCartTotal response",
                    $"Wrong count OrderItems.Count= {orderItems.Count()} and CartLines.Count= {res.ShoppingCartLines.Count()}.", customer);
            }


            //res.DeliveryFlatFee = await customer.GetAttributeAsync<decimal>(CustomerAttributeNames.DeliveryFlatFee);
            res.DeliveryFlatFee = await _genericAttributeService.GetAttributeAsync<decimal>(customer, CustomerAttributeNames.DeliveryFlatFee);

            await _logger.InsertLogAsync(LogLevel.Information,
                $"[PERF DEBUG] GetShoppingCartTotal response. CustomerId={customer.Id}. Process time: {stopwatch.ElapsedMilliseconds} ms.",
                JsonConvert.SerializeObject(res));

            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.RecentGetShoppingCartTotalOrderNumber, res.OrderNumber);

            return res;
        }

        public async Task SaveOrderToFusionAsync(Order savedOrder, ShoppingCartTotalModel cartTotal)
        {
            var currentCustomer = await _customerService.GetCustomerByIdAsync(savedOrder.CustomerId);
            var logMessageShort = string.Empty;
            var logMessageFull = string.Empty;
            string orderNumber = null;
            var recentOrderNumber = cartTotal.OrderNumber;
            var isSubmittedToFusion = false;
            FusionServiceParamsModel fusionParameters = await GetFusionCallParamsAsync(currentCustomer, savedOrder);

            //TODO: change submit date
            #region SubmitOrder

            try
            {
                var storeContext = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(storeContext);
                var paymentType = await _genericAttributeService.GetAttributeAsync<string>(savedOrder,CoreGenericAttributes.PaymentTypeAttribute);
               // var paymentType = await savedOrder.GetAttributeAsync<string>(CoreGenericAttributes.PaymentTypeAttribute);
                var fusionSubmit = await _genericAttributeService.GetAttributeAsync<bool>(savedOrder, OrderAttributeNames.IsFusionSubmitOrderSuccess, storeContext);

                if (fusionSubmit)
                    return;

                OrderPaymentModel orderPaymentModel = new OrderPaymentModel
                {
                    AppliedDate = DateTime.UtcNow.AddHours(settings.HoursShift),
                    CurrencyCode = settings.CurrencyCode.ToString(),
                    PaymentAmount = cartTotal.TotalDue,
                    PaymentReceived = cartTotal.TotalDue,
                    PaymentDate = DateTime.UtcNow.AddHours(settings.HoursShift),
                    ApprovalNumber = savedOrder.AuthorizationTransactionId,
                    Paycode = string.IsNullOrEmpty(paymentType) ? "CARD" : paymentType,
                    PaymentMethodName = string.IsNullOrEmpty(paymentType) ? "CARD" : paymentType,
                    PaymentType = string.IsNullOrEmpty(paymentType) ? "CARD" : paymentType,
                    PaymentMethodId = null // for countries LV, LT, EE, RU
                };

                string notes = (fusionParameters.InvoiceWithShipment == true ? "INVOICE WITH SHIPMENT\r\n" : "INVOICE WITH DISTRIBUTOR\r\n") + await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, CustomerAttributeNames.ShippingNotes);

                await _logger.InsertLogAsync(LogLevel.Information,
                    $"[PERF DEBUG] SubmitOrder request. CustomerId={currentCustomer.Id}. OrderId={savedOrder.Id}",
                    JsonConvert.SerializeObject(fusionParameters), currentCustomer);

                IPromotionWoker promotionWoker = null;
                var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
                using (var serviceScope = serviceScopeFactory.CreateScope())
                {
                    promotionWoker = serviceScope.ServiceProvider.GetService<IPromotionWoker>();
                }

                if (promotionWoker != null)
                {
                    await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.VolumePoints, cartTotal.VolumePoints);
                    await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.DiscountPercent, cartTotal.DiscountPercent);
                    await promotionWoker.RunAsync(savedOrder);
                    //if (await promotionWoker.PromotionNames.AnyAwaitAsync(async promName => await savedOrder.GetAttributeAsync<bool>(promName)))
                    if (await promotionWoker.PromotionNames.AnyAwaitAsync(async promName => await _genericAttributeService.GetAttributeAsync<bool>(savedOrder,promName)))
                    {
                        notes = string.Join(",\r\n", notes, "promotion");
                    }
                }

                //11/16/2018 temporary send notes as TimeSlot since TimeSlot is currently passed as ShippingInstructions to Fusion
                if (notes.Length > 132) notes = notes.Substring(0, 132);
                fusionParameters.TimeSlot = notes;

                var stopwatch = new Stopwatch();
                stopwatch.Start();



                var submitRequestPayment = GetSubmitRequestPayment(orderPaymentModel);

                SubmitOrderModel submitOrderModel = new SubmitOrderModel()
                {
                    FusionServiceParams = fusionParameters,
                    Notes = notes,
                    OrderPaymentModel = orderPaymentModel,
                    OrderWebDate = DateTime.UtcNow.AddHours(settings.HoursShift),
                    ShoppingCartTotalModel = cartTotal,
                    PaymentStatus = submitRequestPayment.PaymentStatus
                };

                SubmitRequestHeader submitRequestHeader = GetSubmitRequestHeader(submitOrderModel);

                var submitOrderLines = GetSubmitOrderLines(fusionParameters.OrderTypeCode, cartTotal.ShoppingCartLines);

                SubmitOrderResultModel res = await _shoppingCartProxyService.SubmitOrderAsync(submitRequestPayment, submitRequestHeader, submitOrderLines);

                stopwatch.Stop();

                if (res.IsSuccess)
                {
                    var orderJSON = JsonConvert.SerializeObject(new
                    {
                        fusionParameters,
                        cartTotal,
                        cartTotal.ShoppingCartLines,
                        submitRequestPayment,
                        notes,
                        invShipFlag = fusionParameters.InvoiceWithShipment ?? false ? "3" : "2",
                        res.OrderNumber,
                        submitOrderModel.OrderWebDate
                    });

                    if (!string.IsNullOrWhiteSpace(orderJSON))
                    {
                        await _genericAttributeService.SaveAttributeAsync(savedOrder, CoreGenericAttributes.OrderJsonStringAttribute, orderJSON);
                    }

                    foreach (var cartLine in cartTotal.ShoppingCartLines)
                    {
                        var product = await _productService.GetProductBySkuAsync(cartLine.Sku.Name);
                        var categories = await _categoryChecker.GetCategoriesByProductIdAsync(product.Id);
                        foreach (var category in categories)
                        {
                           // var categoryType = await category.GetAttributeAsync<CategoryTypeEnum>(CategoryAttributeNames.CategoryType);
                            var categoryType = await _genericAttributeService.GetAttributeAsync<CategoryTypeEnum>(category,CategoryAttributeNames.CategoryType);
                            if (categoryType == CategoryTypeEnum.Ticket)
                            {
                                var totalQuantity = cartLine.Count +
                                    //await currentCustomer.GetAttributeAsync<int>(CoreGenericAttributes.TicketAttribute + cartLine.Sku.Name);
                                    await _genericAttributeService.GetAttributeAsync<int>(currentCustomer,CoreGenericAttributes.TicketAttribute + cartLine.Sku.Name);
                                await _genericAttributeService.SaveAttributeAsync<int>(currentCustomer,
                                    CoreGenericAttributes.TicketAttribute + cartLine.Sku.Name, totalQuantity);
                            }
                        }
                    }
                }

                //clear order number after submit so it isn't used in the next get shopping cart total call
                var attrForEntity = await _genericAttributeService.GetAttributesForEntityAsync(currentCustomer.Id, nameof(Customer));
                var attr = attrForEntity.FirstOrDefault(attr => attr.Key == CustomerAttributeNames.RecentGetShoppingCartTotalOrderNumber);
                if (attr != null)
                {
                    await _genericAttributeService.DeleteAttributeAsync(attr);
                }

                await _logger.InsertLogAsync(LogLevel.Information,
                    $"[PERF DEBUG] SubmitOrder response. CustomerId={currentCustomer.Id}. OrderId={savedOrder.Id}. Process time: {stopwatch.ElapsedMilliseconds} ms.",
                    JsonConvert.SerializeObject(res), currentCustomer);

                orderNumber = res.OrderNumber;
                if (!res.IsSuccess)
                {
                    foreach (var error in res.Errors)
                    {
                        if (!string.IsNullOrWhiteSpace(logMessageFull))
                        {
                            logMessageFull += "; ";
                        }
                        logMessageFull += error;
                    }

                    logMessageShort = $"[PERF DEBUG] [{nameof(FusionUtils)}.{nameof(SaveOrderToFusionAsync)}]. Order id: {savedOrder.Id}. " +
                        "Order has been unsuccessfully submitted to SubmitOrder Fusion method. Errors returned by Fusion.";
                    await _logger.InsertLogAsync(LogLevel.Error, logMessageShort, logMessageFull, currentCustomer);
                }
                else
                {
                    isSubmittedToFusion = true;
                    await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.FusionOrderNumber, orderNumber);

                    var orderJSON_SO = await _genericAttributeService
                        .GetAttributeAsync<string>(savedOrder, CoreGenericAttributes.OrderJsonStringAttribute);
                    await _orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        Note = "ORDER JSON: " + orderJSON_SO,
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow,
                        OrderId = savedOrder.Id
                    });

                    await _logger.InformationAsync($"[PERF DEBUG] [{nameof(FusionUtils)}.{nameof(SaveOrderToFusionAsync)}]. Order id: {savedOrder.Id}: " +
                        "Order has been successfully submited to SubmitOrder Fusion method.", customer: currentCustomer);
                }

            }
            catch (Exception exc)
            {
                logMessageShort = $"[{nameof(FusionUtils)}.{nameof(SaveOrderToFusionAsync)}] Order id: {savedOrder.Id}: Error calling SubmitOrder Fusion method.";
                logMessageFull = exc.ToString();
                await _logger.ErrorAsync(logMessageShort, exc, currentCustomer);
            }

            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.IsFusionSubmitOrderSuccess, isSubmittedToFusion);

            #endregion

            #region Save OrderNotes

            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                Note = $"[DEBUG]RESULT OF CALLING FUSION: {logMessageShort};{logMessageFull}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = savedOrder.Id
            });

            var orderNotes = await _orderService.GetOrderNotesByOrderIdAsync(savedOrder.Id);

            if (!isSubmittedToFusion)
            {
                var fusionFailedOrderNote = orderNotes.FirstOrDefault(x => x.Note == NopFiluetCommonDefaults.FusionFailedOrderNotesMessage);
                if (fusionFailedOrderNote != null)
                {
                    await _orderService.DeleteOrderNoteAsync(fusionFailedOrderNote);
                }

                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = $"{NopFiluetCommonDefaults.FusionFailedOrderNotesMessage}:{logMessageFull}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }
            else
            {
                var fusionFailedOrderNote = orderNotes.FirstOrDefault(x => x.Note == NopFiluetCommonDefaults.FusionSuccessOrderNotesMessage);
                if (fusionFailedOrderNote != null)
                {
                    await _orderService.DeleteOrderNoteAsync(fusionFailedOrderNote);
                }

                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = NopFiluetCommonDefaults.FusionSuccessOrderNotesMessage,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }
            if (!string.IsNullOrWhiteSpace(orderNumber))
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = $"ORACLE ORDER NUMBER: {orderNumber}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                Note = $"ORDER MONTH: {fusionParameters.OrderMonth}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = savedOrder.Id
            });
            #region monthVP

            var monthVp = await SetMonthVPAsync(currentCustomer, cartTotal.VolumePoints, fusionParameters.OrderMonth);
            await _orderService.InsertOrderNoteAsync(new OrderNote
            {
                Note = $"ORDER MONTH VP: {monthVp}",
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = savedOrder.Id
            });
            #endregion


            if (!string.IsNullOrWhiteSpace(recentOrderNumber))
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = $"LAST GET PRICING DETAILS ORDER NUMBER: {recentOrderNumber}",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }

            var distributorVolume = await _distributorService.GetDistributorVolumeAsync(currentCustomer);

            var orderMonthVolume = distributorVolume?.Tv;
            var discountRate = cartTotal.DiscountPercent;
            var orderVolumePoints = cartTotal.VolumePoints;
            var productEarnbase = cartTotal.ShoppingCartLines.Sum(x => x.TotalEarnbase ?? 0);
            var basePrice = cartTotal.AmountBase;//as sub total
            var deliveryFlatFee = cartTotal.DeliveryFlatFee;
            var totalDue = cartTotal.TotalDue; //as total
            var taxAmount = cartTotal.TotalTaxAmount;
            var shippingCost = cartTotal.FreightCharge;
            var discountedBasePrice = cartTotal.DiscountedBasePrice;

            //log price and discount data
            var pricingDetails = new List<string>();

            pricingDetails.Add($"ORDER MONTH VOLUME: {orderMonthVolume:0.00}");
            pricingDetails.Add($"DISCOUNT RATE: {discountRate:0.00}");


            pricingDetails.Add($"VOLUME POINTS: {orderVolumePoints:0.00}");
            pricingDetails.Add($"PRODUCT EARN BASE: {productEarnbase:0.00}");
            pricingDetails.Add($"BASE PRICE: {basePrice:0.00}");


            pricingDetails.Add($"DISCOUNTED BASE PRICE: {discountedBasePrice:0.00}");
            pricingDetails.Add($"FREIGHT & HANDLING CHARGE: {shippingCost:0.00}");
            pricingDetails.Add($"TAX AMOUNT: {taxAmount:0.00}");
            pricingDetails.Add($"TOTAL DUE: {totalDue:0.00}");
            pricingDetails.Add($"FLAT FEE: {deliveryFlatFee:0.00}");

            pricingDetails.Add($"TOTAL AMOUNT (W/O TAX): {cartTotal.TotalAmount:0.00}");
            pricingDetails.Add($"DISCOUNT AMOUNT: {cartTotal.Discount:0.00}");

            var orderItems = await _orderService.GetOrderItemsAsync(savedOrder.Id);

            decimal catalogTotal = 0.0M;
            decimal catalogVPs = 0.0M;

            foreach (var orderItem in orderItems)
            {
                var product = await _productService.GetProductByIdAsync(orderItem.ProductId);
                catalogTotal += orderItem.Quantity * product.Price;
                //catalogVPs += orderItem.Quantity * await product.GetAttributeAsync<decimal>(ProductAttributeNames.VolumePoints);
                catalogVPs += orderItem.Quantity * await _genericAttributeService.GetAttributeAsync<decimal>(product,ProductAttributeNames.VolumePoints);
            }


            pricingDetails.Add($"TOTAL DUE (CATALOG): {catalogTotal:0.00}");
            pricingDetails.Add($"VOLUME POINTS (CATALOG): {catalogVPs:0.00}");

            foreach (var pricingEntry in pricingDetails)
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = pricingEntry,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }
            //log codes
            var codeData = new List<string>();
            codeData.Add($"COUNTRY CODE SENT TO ORACLE: {fusionParameters.CountryCode}");
            codeData.Add($"PROCESSING LOCATION CODE SENT TO ORACLE: {fusionParameters.ProcessingLocation}");
            codeData.Add($"FREIGHT CODE SENT TO ORACLE: {fusionParameters.FreightCode}");
            codeData.Add($"WAREHOUSE CODE SENT TO ORACLE: {fusionParameters.WarehouseCode}");
            codeData.Add($"POSTAMAT CODE SENT TO ORACLE: {fusionParameters.PostamatId}");
            foreach (var codeEntry in codeData)
            {
                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = codeEntry,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }

            #endregion

            var shippingCalcOpt = await _filuetShippingService.GetSelectedShippingComputationOptionModelAsync(currentCustomer, savedOrder);

            //customer attributes
            var customerData = new List<string>();
            var distributorProfile = await _distributorService.GetDistributorProfileAsync(currentCustomer);

            var cop = distributorProfile.CountryOfProcessing ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var com = distributorProfile.MailingCountryCode ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var cor = distributorProfile.CountryOfResidence ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;

            var freightCode = shippingCalcOpt.FreightCode;
            var cos = shippingCalcOpt.CountryCode ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipCarrier = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CustomerAttributeNames.SelectedShippingMethodSystemName)
                ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;

            if (freightCode == FreightCodes.NOF)
            {
                shipCarrier = NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            }

            var shipCity = shippingCalcOpt.City ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipZip = shippingCalcOpt.ShippingZipCode ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipAddress = shippingCalcOpt.Address ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipPostamatId = (await _genericAttributeService.GetAttributeAsync<PickupPoint>(currentCustomer,NopCustomerDefaults.SelectedPickupPointAttribute))?.Id
                ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipFullname = await currentCustomer.GetFullNameAsync() ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipPhone = shippingCalcOpt.PhoneNumber ?? NopFiluetCommonDefaults.EmptyDisplayPlaceholder;
            var shipTime = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CustomerAttributeNames.DeliveryTime);

            var shipComment = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer,CustomerAttributeNames.ShippingNotes);
            var invoiceWithShipment = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer,CustomerAttributeNames. IsShipInvoiceWithOrder);
            var weight = await _genericAttributeService.GetAttributeAsync<decimal>(currentCustomer,CustomerAttributeNames.DeliveryWeight);
           
            customerData.Add($"COUNTRY OF PROCESSING: {cop}");
            customerData.Add($"COUNTRY OF MAILING: {com}");
            customerData.Add($"COUNTRY OF RESIDENCE: {cor}");
            customerData.Add($"INVOICE WITH SHIPMENT: {invoiceWithShipment}");
            customerData.Add($"SHIPPING COUNTRY: {cos}");
            customerData.Add($"SHIPPING CARRIER: {shipCarrier}");
            customerData.Add($"SHIPPING CITY: {shipCity}");
            customerData.Add($"SHIPPING ZIPCODE: {shipZip}");
            customerData.Add($"SHIPPING ADDRESS: {shipAddress}");
            customerData.Add($"SHIPPING FULLNAME: {shipFullname}");
            customerData.Add($"SHIPPING PHONE NUMBER: {shipPhone}");
            customerData.Add($"SHIPPING POSTAMAT ID: {shipPostamatId}");
            customerData.Add($"SHIPPING TIME: {shipTime}");
            customerData.Add($"SHIPPING COMMENT: {shipComment}");
            customerData.Add($"SELECTED SHIPPING OPTION: {shippingCalcOpt.Id}");
            customerData.Add($"SHIPPING WEIGHT: {weight}");

            foreach (var entry in customerData)
            {

                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = entry,
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = savedOrder.Id
                });
            }

            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingCountry, cos);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingCarrier, shipCarrier);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingCity, shipCity);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingZipCode, shipZip);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingComment, shipComment);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingAddress, shipAddress);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingFullname, shipFullname);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingPhoneNumber, shipPhone);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingPostamatId, shipPostamatId);

            var savedShipTime = await _genericAttributeService.GetAttributeAsync<string>(savedOrder,OrderAttributeNames.SelectedShippingTimeSlot);
            if (string.IsNullOrWhiteSpace(savedShipTime))
            {
                await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.SelectedShippingTimeSlot, shipTime);
            }
            var isShipInvoice = await _genericAttributeService.GetAttributeAsync<bool>(currentCustomer,CustomerAttributeNames.IsShipInvoiceWithOrder);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.IsShipInvoiceWithOrder, isShipInvoice);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.RecentGetShoppingCartTotalOrderNumber, recentOrderNumber);

            var orderMonth = await _dualMonthsService.GetOrderMonthOfCustomerAsync(currentCustomer);

            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.OrderMonth, orderMonth);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.OrderMonthVolume, orderMonthVolume);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.DiscountPercent, discountRate);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.VolumePoints, orderVolumePoints);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.ProductEarnbase, productEarnbase);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.AmountBase, basePrice);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.DeliveryFlatFee, deliveryFlatFee);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.TotalDue, totalDue);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.TotalTaxAmount, taxAmount);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.FreightCharge, shippingCost);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.DiscountedBasePrice, discountedBasePrice);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.FreightCode, freightCode);
            await _genericAttributeService.SaveAttributeAsync(savedOrder, OrderAttributeNames.ShippingWeight, weight);

            //clear customer attributes
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.DeliveryCompanyName, (string)null);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.DeliveryRateName, (string)null);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.Address, (string)null);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, CustomerAttributeNames.City, (string)null);
        }

        public async Task<ShoppingCartTotalModel> GetShoppingCartTotalOffline(Customer customer, IEnumerable<ShoppingCartItem> cartItems = null)
        {
            decimal totalAmount = 0;
            double totalVPs = 0;
            decimal deliveryFlatFee = await _genericAttributeService.GetAttributeAsync<decimal>(customer, CustomerAttributeNames.DeliveryFlatFee);

            if (cartItems == null)
            {
                cartItems = await _shoppingCartService.GetShoppingCartAsync(customer);
            }
            if (customer != null && cartItems != null && cartItems.Any())
            {
                var subTotalIncludingTax =await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                var (_, _, _, subTotalWithoutDiscountBase, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cartItems.ToList(), subTotalIncludingTax);
                totalAmount = subTotalWithoutDiscountBase;
                totalVPs = (await Task.WhenAll(cartItems.Select(async x =>
                {
                    var product = await _productService.GetProductByIdAsync(x.ProductId);
                    var volumePoints = await _genericAttributeService.GetAttributeAsync<double>(product, ProductAttributeNames.VolumePoints);
                    return x.Quantity * volumePoints;
                }))).Sum();
               // totalVPs = Convert.ToDouble(cartItems.Sum(x => x.Quantity * (_genericAttributeService.GetAttributeAsync<double>(_productService.GetProductByIdAsync(x.ProductId).Result,ProductAttributeNames.VolumePoints).Result)));
            }
            return new ShoppingCartTotalModel()
            {
                DeliveryFlatFee = deliveryFlatFee,
                TotalAmount = totalAmount,
                TotalDue = totalAmount,
                VolumePoints = totalVPs
            };
        }

        private async Task<string> GetOrderCategoryAsync(Order order)
        {
            CategoryTypeEnum categoryType = await order.GetOrderTypeAsync();
            var customerById = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            return await MapOrderCategoryAsync(categoryType, customerById ?? currentCustomer);
        }

        private async Task<string> GetOrderCategoryAsync(Customer customer)
        {
            var shoppingCart = await _shoppingCartService.GetShoppingCartAsync(customer);
            CategoryTypeEnum categoryType = await shoppingCart.ToList().GetOrderTypeAsync();
            return await MapOrderCategoryAsync(categoryType, customer);
        }

        private async Task<string> MapOrderCategoryAsync(CategoryTypeEnum categoryType, Customer customer)
        {
            switch (categoryType)
            {
                case CategoryTypeEnum.Maintenance:
                    if (await customer.IsCustomerFromBalticCountryAsync())
                    {
                        return OrderCategories.APF;
                    }
                    else
                    {
                        return OrderCategories.RSO;
                    }
                case CategoryTypeEnum.Ticket:
                    if (await customer.IsCustomerFromBalticCountryAsync())
                    {
                        return OrderCategories.ETO;
                    }
                    else
                    {
                        return OrderCategories.RSO;
                    }
            }
            return OrderCategories.RSO;
        }

        private async Task<string> GetOrderSpecificCategoryAsync(Customer customer, Order order)
        {
            if (order != null)
            {
                return await GetOrderCategoryAsync(order);
            }

            if (await _shoppingCartService.GetShoppingCartAsync(customer) != null && (await _shoppingCartService.GetShoppingCartAsync(customer)).Any())
            {
                return await GetOrderCategoryAsync(customer);
            }

            return null;
        }

        private string GetOrderType(string orderCategory, string shipCountry, string defaultOrderTypeId, Customer customer)
        {
            if (orderCategory == OrderCategories.ETO)
            {
                switch (shipCountry)
                {
                    case CountryCodes.LV:
                        return OrderTypeIds.LVETO;
                    case CountryCodes.LT:
                        return OrderTypeIds.LTETO;
                    case CountryCodes.EE:
                        return OrderTypeIds.EEETO;
                    default:
                        return defaultOrderTypeId;
                }
            }

            if (orderCategory != OrderCategories.APF)
            {
                return defaultOrderTypeId;
            }

            switch (shipCountry)
            {
                case CountryCodes.LV:
                    return OrderTypeIds.LVAPF;
                case CountryCodes.LT:
                    return OrderTypeIds.LTAPF;
                case CountryCodes.EE:
                    return OrderTypeIds.EEAPF;
                default:
                    return defaultOrderTypeId;
            }
        }

        private string GetOrderTypeId(FiluetFusionShippingComputationOptionModel selectedOption)
        {
            switch (selectedOption.CountryCode)
            {
                case CountryCodes.UZ:
                    return OrderTypeIds.UZ;
                case CountryCodes.AZ:
                    return OrderTypeIds.AZ;
                case CountryCodes.LT:
                    return OrderTypeIds.LTShipTo;
                case CountryCodes.LV:
                    return OrderTypeIds.LVShipTo;
                case CountryCodes.EE:
                    return OrderTypeIds.EEShipTo;
                case CountryCodes.KZ:
                    return OrderTypeIds.KVShipTo;
                case CountryCodes.CY:
                    return OrderTypeIds.CY;
            }

            return null;
        }

        private async Task<decimal> SetMonthVPAsync(Customer customer, double cartTotalVolumePoints, string requestParamsOrderMonth)
        {
            var totalVolumePoints = await _genericAttributeService.GetAttributeAsync<decimal>(customer, CustomerAttributeNames.MonthVolumePoints + requestParamsOrderMonth);

            totalVolumePoints += (decimal)cartTotalVolumePoints;

            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.MonthVolumePoints + requestParamsOrderMonth, totalVolumePoints);

            return totalVolumePoints;
        }

        private SubmitRequestPayment GetSubmitRequestPayment(OrderPaymentModel payment)
        {
            var flag = payment.Paycode == "WIRE";

            var orderPaymentStatus = payment.Paycode == "WIRE" ? "Underpaid" : "Paid";
            var approvalNumber = flag ? null : payment.ApprovalNumber;
            //Фьюжн не может принимать ИД транзакции больше 30 символов
            if (approvalNumber?.Length > 30)
            {
                approvalNumber = approvalNumber.Substring(0, 30);
            }

            return new SubmitRequestPayment()
            {
                AppliedDate = flag || !payment.AppliedDate.HasValue ? DateTime.Now : payment.AppliedDate.Value,
                ApprovalNumber = approvalNumber,
                ClientRefNumber = string.Empty,
                CreditCard = new OrderSubmitPaymentCreditCard
                {
                    CardExpiryDate = DateTime.Now.AddYears(1),
                    CardHolderName = flag ? null : "CARD HOLDER",
                    CardNumber = flag ? null : "0B11074741560000",
                    CardType = flag ? null : "VI",
                    TrxApprovalNumber = approvalNumber
                },
                CurrencyCode = payment.CurrencyCode,
                Date = flag || !payment.PaymentDate.HasValue ? DateTime.UtcNow : payment.PaymentDate.Value,
                Paycode = payment.Paycode,
                PaymentAmount = payment.PaymentAmount ?? 0,
                PaymentMethodId = payment.PaymentMethodId,
                PaymentMethodName = payment.PaymentMethodName,
                PaymentReceived = flag || !payment.PaymentReceived.HasValue ? 0 : payment.PaymentReceived.Value,
                PaymentStatus = orderPaymentStatus,
                PaymentType = payment.PaymentType
            };
        }

        private SubmitRequestHeader GetSubmitRequestHeader(SubmitOrderModel submitOrderModel)
        {
            FusionServiceParamsModel fusionServiceParams = submitOrderModel.FusionServiceParams;
            ShoppingCartTotalModel shoppingCart = submitOrderModel.ShoppingCartTotalModel;

            var freightCode = fusionServiceParams?.FreightCode;
            bool isPudo = freightCode.Equals("BLD") || freightCode.Equals("BLO") || freightCode.Equals("BLP") || freightCode.Equals("C2P") || freightCode.Equals("C2D");

            return new SubmitRequestHeader()
            {
                Address1 = fusionServiceParams.Address,
                Address2 = string.Empty,
                Balance = 0,
                ChrAttribute3 = isPudo ? "Y" : "N",
                ChrAttribute4 = isPudo ? fusionServiceParams.PostamatId : null,
                ChrAttribute5 = isPudo ? fusionServiceParams.PhoneNumber : null,
                ChrAttribute6 = "QR",
                City = fusionServiceParams.City,
                CountryCode = fusionServiceParams.CountryCode,
                CustomerName = fusionServiceParams.CustomerName,
                DiscountAmount = 0,
                DistributorId = fusionServiceParams.DistributorId,
                ExternalOrderNumber = shoppingCart.OrderNumber,
                InvShipFlag = fusionServiceParams.InvoiceWithShipment ?? false ? "3" : "2",
                Notes = submitOrderModel.Notes,
                OrderConfirmEmail = null,
                OrderDate = DateTime.UtcNow,
                OrderDiscountPercent = Convert.ToDecimal(shoppingCart.DiscountPercent),
                OrderMonth = DateTime.ParseExact(fusionServiceParams.OrderMonth, "yy.MM", CultureInfo.CurrentCulture),
                OrderPaymentStatus = submitOrderModel.PaymentStatus,
                OrderPurpose = fusionServiceParams.DsType,
                OrderSource = "KIOSK",
                OrderSubType = string.Empty,
                OrderTypeCode = fusionServiceParams.OrderTypeCode,
                OrderTypeId = int.Parse(fusionServiceParams.OrderTypeId),
                OrgId = ProxyDataProvider.GetOrgId(fusionServiceParams.ProcessingLocation),
                Phone = fusionServiceParams.PhoneNumber,
                PickupName = string.Empty,
                PostalCode = fusionServiceParams.PostalCode,
                PricingDate = DateTime.UtcNow,
                ProcessingLocation = fusionServiceParams.ProcessingLocation,
                Province = string.Empty,
                SMSAction = string.Empty,
                SMSNumber = string.Empty,
                SMSRole = string.Empty,
                SalesChannelCode = "INTERNET",
                ShippingInstructions = fusionServiceParams.TimeSlot,
                ShippingMethodCode = freightCode,
                SlidingDiscount = 0,
                State = string.Empty,
                TaxAmount = shoppingCart.TotalTaxAmount,
                TotalAmountPaid = submitOrderModel.PaymentStatus.ToUpper() == "UNDERPAID"
                ? 0m
                : (submitOrderModel.OrderPaymentModel.PaymentAmount ?? 0m),
                TotalDue = shoppingCart.TotalDue,
                TotalRetailPrice = 99,
                TotalVolume = Convert.ToDecimal(shoppingCart.CurrentOrderVolumePoints),
                WareHouseCode = fusionServiceParams.WarehouseCode,
                WillCallFlag = "N"
            };

        }

        private SubmitRequestOrderLine[] GetSubmitOrderLines(string orderTypeCode, IEnumerable<ShoppingCartLineModel> orderItems)
        {
            string productType = "P";
            if (string.Equals("ETO", orderTypeCode, StringComparison.InvariantCultureIgnoreCase))
                productType = "E";
            return orderItems.Select(item => new SubmitRequestOrderLine
            {
                Amount = item.LineAmount ?? 0,
                EarnBase = item.Earnbase ?? 0,
                ProductType = productType,
                Quantity = item.Count,
                Sku = item.Sku.Name,
                TotalDiscountedPrice = item.TotalDiscountedPrice ?? 0,
                TotalRetailPrice = item.TotalRetailPrice ?? 0,
                UnitVolume = item.UnitVolume ?? 0
            }).ToArray();
        }

        public async Task ReSubmitOrderAsync(Order order)
        {
            var fusionSubmitOrderSuccess = (await _genericAttributeRepository.GetAllAsync(q => q.Where(ga => ga.KeyGroup == nameof(Order) &&
            ga.Key == OrderAttributeNames.IsFusionSubmitOrderSuccess && ga.Value == "true" && ga.EntityId == order.Id)))?.FirstOrDefault();

            if (order.PaymentStatus == PaymentStatus.Paid && fusionSubmitOrderSuccess == null)
            {
                var customerByIdAsync = await _customerService.GetCustomerByIdAsync(order.CustomerId);
                ShoppingCartTotalModel cartTotal =
                    await GetShoppingCartTotalAsync(
                        customerByIdAsync, order);
                await SaveOrderToFusionAsync(order, cartTotal);
                order.OrderShippingInclTax = await _genericAttributeService.GetAttributeAsync<decimal>(customerByIdAsync,CustomerAttributeNames.DeliveryPrice);
                order.OrderTax = cartTotal.TotalTaxAmount;
            }
        }

        #endregion
    }
}
