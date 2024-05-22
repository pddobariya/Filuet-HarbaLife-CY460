using ISDK.Filuet.OnlineOrdering.CorePlugin.Factories;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Helpers;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure.Mapper;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Models.Media;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Controllers
{
    /// <summary>
    /// FiluetOrderController
    /// </summary>
    public class FiluetOrderController : OrderController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IFiluetOrderModelFactory _filuetOrderModelFactory;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IProductService _productService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly MediaSettings _mediaSettings;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IStatusService _statusService;
        private readonly IShipmentService _shipmentService;
        private readonly IGenericAttributeService _genericAttributeService;


        #endregion

        #region Ctor

        public FiluetOrderController(
            ICustomerService customerService,
            IOrderModelFactory orderModelFactory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IShipmentService shipmentService,
            IWebHelper webHelper,
            IWorkContext workContext,
            RewardPointsSettings rewardPointsSettings,
            ILogger logger,
            IFiluetOrderModelFactory filuetOrderModelFactory,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IPictureService pictureService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IProductService productService,
            ILocalizedEntityService localizedEntityService,
            MediaSettings mediaSettings,
            IOrderStatusService orderStatusService,
            IStatusService statusService,
            IGenericAttributeService genericAttributeService) 
            : base(
                customerService,
                orderModelFactory,
                orderProcessingService,
                orderService, 
                paymentService, 
                pdfService, 
                shipmentService,
                webHelper,
                workContext,
                rewardPointsSettings)
        {
            _customerService = customerService;
            _orderService = orderService;
            _filuetOrderModelFactory = filuetOrderModelFactory;
            _logger = logger;
            _settingService = settingService;
            _workContext = workContext;
            _orderModelFactory = orderModelFactory;
            _staticCacheManager = staticCacheManager;
            _pictureService = pictureService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _mediaSettings = mediaSettings;
            _localizationService = localizationService;
            _productService = productService;
            _localizedEntityService = localizedEntityService;
            _orderStatusService = orderStatusService;
            _statusService = statusService;
            _shipmentService = shipmentService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        [AspMvcSuppressViewError]
        public override async Task<IActionResult> Details(int orderId)
        {
            Order order = await _orderService.GetOrderByIdAsync(orderId);
            Customer customer = await _workContext.GetCurrentCustomerAsync();
            if (order == null || order.Deleted || customer.Id != order.CustomerId)
            {
                return new UnauthorizedResult();
            }
            string fusionOrderNumber =await order.GetFusionOrderNumberAsync();

            var baseModel = await _orderModelFactory.PrepareOrderDetailsModelAsync(order);

            var model = AutoMapperConfiguration.Mapper.Map<FiluetOrderDetailsModel>(baseModel);

            var currentStore = await _storeContext.GetCurrentStoreAsync();
            var pictureSize = _mediaSettings.ProductThumbPictureSize;

            foreach (var item in model.Items)
            {
                var lang = await _workContext.GetWorkingLanguageAsync();
                //prepare picture model
                var orderPictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopFiluetCommonDefaults.OrderPictureModelKey, item,
                    pictureSize, true, lang, _webHelper.IsCurrentConnectionSecured(),
                    currentStore);

                var product = await _productService.GetProductByIdAsync(item.ProductId);
                item.ShortDescription = await _localizedEntityService.GetLocalizedValueAsync(lang.Id, item.ProductId, product.GetType().Name, "ShortDescription");
                item.PictureModel = await _staticCacheManager.GetAsync(orderPictureCacheKey, async () =>
                {
                    var pictures = await _pictureService.GetPicturesByProductIdAsync(item.ProductId, 1);
                    string imageUrl;

                    var picture = pictures.FirstOrDefault();
                    (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, pictureSize);

                    var pictureModel = new PictureModel
                    {
                        ImageUrl = imageUrl,
                        Title = string.Format(await _localizationService
                            .GetResourceAsync("Media.Product.ImageLinkTitleFormat.Details"), item.ProductName),
                        AlternateText = string.Format(await _localizationService
                            .GetResourceAsync("Media.Product.ImageAlternateTextFormat.Details"), item.ProductName)
                    };

                    return pictureModel;
                });
            }

            if ((await _settingService.LoadSettingAsync<FiluetCorePluginSettings>()).ShowOrderStatuses)
            {
                try
                {
                    var statuses = await _statusService.GetStatusesAsync();
                    var workingLanguage = await _workContext.GetWorkingLanguageAsync();
                    var statusLocaleStrings = await _orderStatusService.GetStatusLocaleStringsAsync(workingLanguage.Id);
                    var orderStatuses = await _orderStatusService.GetOrderStatusesByOrderIdAsync(model.Id);

                    model.OrderStatusDtos = statuses.OrderBy(p => p.Id).Select(p =>
                    {
                        return new OrderStatusDto
                        {
                            Status = statusLocaleStrings.FirstOrDefault(m => m.StatusId == p.Id)?.StatusName,
                            StatusDate = orderStatuses.FirstOrDefault(n => n.StatusId == p.Id)?.StatusDate,
                            OrderStatusClass = FiluetOrderStatusHelper.GetOrderStatusClass(p.ExternalStatusName)
                        };
                    });
                }
                catch (Exception e)
                {
                    await _logger.ErrorAsync("Ошибка получения статусов заказа", e);
                }
            }

            var shipments = await _shipmentService.GetShipmentsByOrderIdAsync(orderId);
            string trackNumber = shipments.Count > 0 ? shipments.First().TrackingNumber : string.Empty;

            model.FusionOrderId = fusionOrderNumber;
            model.CustomerName =await _customerService.GetCustomerFullNameAsync(customer);

            //init shipping and invoice data
            model.ShipToAddress =await _genericAttributeService.GetAttributeAsync<string>(order,OrderAttributeNames.SelectedShippingAddress);
            model.DeliveryOperator =await _genericAttributeService.GetAttributeAsync<string>(order,OrderAttributeNames.SelectedShippingCarrier);
            model.ShipToCity =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingCity);
            model.ShipToCountryCode =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingCountry);
            model.ShipToZipcode =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingZipCode);
            model.ShipToFullname =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingFullname);
            model.ShipToPhone =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingPhoneNumber);
            model.ShipToPostamatId = trackNumber; //order.GetAttribute<string>(OrderAttributeNames.SelectedShippingPostamatId);
            model.ShipToTimeslot =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingTimeSlot);
            model.IsShipInvoiceWithOrder =await _genericAttributeService.GetAttributeAsync<bool>(order, OrderAttributeNames.IsShipInvoiceWithOrder);
            model.Comment =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.SelectedShippingComment);

            //init totals data
            model.OrderTotals.FlatFee = null;

            var store = await _storeContext.GetCurrentStoreAsync();

            decimal flatFee = await _genericAttributeService.GetAttributeAsync(
                order, OrderAttributeNames.DeliveryFlatFee, store.Id, -1M);

            if (flatFee > 0)
                model.OrderTotals.FlatFee =await flatFee.FormatPriceAsync();
                
            model.OrderTotals.OrderTotal = model.OrderTotal;
            model.OrderTotals.OrderMonth =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.OrderMonth);

            var orderMonthVolumeString =await _genericAttributeService.GetAttributeAsync<string>(order, OrderAttributeNames.OrderMonthVolume);
            if (double.TryParse(orderMonthVolumeString, out double orderMonthVolume))
            {
                model.OrderTotals.OrderMonthVolume =await orderMonthVolume.FormatPriceAsync(true);
            }
            model.OrderTotals.OrderVolumePoints =await(await _genericAttributeService.GetAttributeAsync<double>(order, OrderAttributeNames.VolumePoints)).FormatPriceAsync(true);
            model.OrderTotals.Tax = await(await _genericAttributeService.GetAttributeAsync<decimal>(order, OrderAttributeNames.TotalTaxAmount)).FormatPriceAsync();
            model.OrderTotals.ProductEarnBase =await(await _genericAttributeService.GetAttributeAsync<decimal>(order, OrderAttributeNames.ProductEarnbase)).FormatPriceAsync();
            model.OrderTotals.Shipping =await(await _genericAttributeService.GetAttributeAsync<decimal>(order, OrderAttributeNames.FreightCharge)).FormatPriceAsync();
            model.OrderTotals.DiscountRate =(await _genericAttributeService.GetAttributeAsync<double>(order, OrderAttributeNames.DiscountPercent)).FormatPercent();
            model.OrderTotals.DiscountedBasePrice =await(await _genericAttributeService.GetAttributeAsync<decimal>(order, OrderAttributeNames.DiscountedBasePrice)).FormatPriceAsync();
            model.OrderTotals.SubTotal =await(await _genericAttributeService.GetAttributeAsync<decimal>(order, OrderAttributeNames.AmountBase)).FormatPriceAsync();
            model.OrderTotals.DeliveryPrice =await order.OrderShippingInclTax.FormatPriceAsync();

            return View("Details", model);
        }

        [NonAction]
        public override async Task<IActionResult> CustomerOrders()
        {
            return await CustomerOrders(null);
        }

        [AspMvcSuppressViewError]
        public async Task<IActionResult> CustomerOrders(OrderPagingFilteringModel command)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return Challenge();

            var model = await _filuetOrderModelFactory.PrepareCustomerOrderListModel(command);
            return View("CustomerOrders", model);
        }

        #endregion
    }
}