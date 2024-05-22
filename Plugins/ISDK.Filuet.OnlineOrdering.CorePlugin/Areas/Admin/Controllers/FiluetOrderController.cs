using ISDK.Filuet.OnlineOrdering.CorePlugin.Areas.Admin.Models;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Conventions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Areas.Admin.Controllers
{
    [NameControllerModelConvention("Order")]
    public class FiluetOrderController : OrderController
    {
        #region Fields

        private readonly IPriceFormatter _priceFormatter;
        private readonly IAddressService _addressService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IOrderModelFactory _orderModelFactory;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly AddressSettings _addressSettings;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICountryService _countryService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IFiluetOrderService _filuetOrderService;
        private readonly IFusionIntegrationService _fusionIntegrationService;

        #endregion

        #region Ctor

        public FiluetOrderController(IAddressAttributeParser addressAttributeParser,
            IAddressService addressService,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IEncryptionService encryptionService, 
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IGiftCardService giftCardService,
            IImportManager importManager, 
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderModelFactory orderModelFactory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IPaymentService paymentService,
            IPdfService pdfService,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            IProductAttributeFormatter productAttributeFormatter, 
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService, 
            IProductService productService,
            IShipmentService shipmentService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext, 
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            OrderSettings orderSettings,
            IPriceFormatter priceFormatter,
            IStoreService storeService,
            AddressSettings addressSettings, 
            IBaseAdminModelFactory baseAdminModelFactory,
            CatalogSettings catalogSettings,
            ICountryService countryService,
            IPaymentPluginManager paymentPluginManager,
            IFiluetOrderService filuetOrderService,
            IFusionIntegrationService fusionIntegrationService)
            : base(addressAttributeParser,
                  addressService, 
                  customerActivityService, 
                  customerService, 
                  dateTimeHelper, 
                  downloadService,
                  encryptionService,
                  eventPublisher,
                  exportManager,
                  giftCardService,
                  importManager,
                  localizationService, 
                  notificationService,
                  orderModelFactory,
                  orderProcessingService,
                  orderService, 
                  paymentService,
                  pdfService,
                  permissionService,
                  priceCalculationService,
                  productAttributeFormatter,
                  productAttributeParser,
                  productAttributeService, 
                  productService, 
                  shipmentService,
                  shippingService, 
                  shoppingCartService,
                  storeContext,
                  workContext,
                  workflowMessageService, 
                  orderSettings)
        {
            _priceFormatter = priceFormatter;
            _addressService = addressService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderModelFactory = orderModelFactory;
            _orderService = orderService;
            _permissionService = permissionService;
            _productService = productService;
            _workContext = workContext;
            _storeService = storeService;
            _addressSettings = addressSettings;
            _baseAdminModelFactory = baseAdminModelFactory;
            _catalogSettings = catalogSettings;
            _countryService = countryService;
            _paymentPluginManager = paymentPluginManager;
            _filuetOrderService = filuetOrderService;
            _fusionIntegrationService = fusionIntegrationService;
        }

        #endregion

        #region Methods

        public override async Task<IActionResult> LoadOrderStatistics(string period)
        {
            ILogger logger = EngineContext.Current.Resolve<ILogger>();
            await logger.InformationAsync("DEBUG: custom order stats");
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return Content("");

            //a vendor doesn't have access to this report
            if (await _workContext.GetCurrentVendorAsync() != null)
                return Content("");

            var result = new List<object>();

            var nowDt = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
            var timeZone = await _dateTimeHelper.GetCurrentTimeZoneAsync();

            var culture = new CultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);
            
            List<int> filteredOrderStatuses = new List<int>();
            filteredOrderStatuses.Add((int)OrderStatus.Processing);
            filteredOrderStatuses.Add((int)OrderStatus.Complete);

            switch (period)
            {
                case "year":
                    //year statistics
                    var yearAgoDt = nowDt.AddYears(-1).AddMonths(1);
                    var searchYearDateUser = new DateTime(yearAgoDt.Year, yearAgoDt.Month, 1);
                    if (!timeZone.IsInvalidTime(searchYearDateUser))
                    {
                        for (int i = 0; i <= 12; i++)
                        {
                            result.Add(new
                            {
                                date = searchYearDateUser.Date.ToString("Y", culture),
                                value = (await _orderService.SearchOrdersAsync(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchYearDateUser.AddMonths(1),
                                        timeZone), osIds: filteredOrderStatuses,
                                    pageIndex: 0,
                                    pageSize: 1)).TotalCount.ToString()
                            });

                            searchYearDateUser = searchYearDateUser.AddMonths(1);
                        }
                    }
                    break;

                case "month":
                    //month statistics
                    var monthAgoDt = nowDt.AddDays(-30);
                    var searchMonthDateUser = new DateTime(monthAgoDt.Year, monthAgoDt.Month, monthAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchMonthDateUser))
                    {
                        for (int i = 0; i <= 30; i++)
                        {
                            result.Add(new
                            {
                                date = searchMonthDateUser.Date.ToString("M", culture),
                                value = (await _orderService.SearchOrdersAsync(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchMonthDateUser.AddDays(1),
                                        timeZone), osIds: filteredOrderStatuses,
                                    pageIndex: 0,
                                    pageSize: 1)).TotalCount.ToString()
                            });

                            searchMonthDateUser = searchMonthDateUser.AddDays(1);
                        }
                    }
                    break;

                case "week":
                default:
                    //week statistics
                    var weekAgoDt = nowDt.AddDays(-7);
                    var searchWeekDateUser = new DateTime(weekAgoDt.Year, weekAgoDt.Month, weekAgoDt.Day);
                    if (!timeZone.IsInvalidTime(searchWeekDateUser))
                    {
                        for (int i = 0; i <= 7; i++)
                        {
                            result.Add(new
                            {
                                date = searchWeekDateUser.Date.ToString("d dddd", culture),
                                value = (await _orderService.SearchOrdersAsync(
                                    createdFromUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser, timeZone),
                                    createdToUtc: _dateTimeHelper.ConvertToUtcTime(searchWeekDateUser.AddDays(1),
                                        timeZone), osIds: filteredOrderStatuses,
                                    pageIndex: 0,
                                    pageSize: 1)).TotalCount.ToString()
                            });

                            searchWeekDateUser = searchWeekDateUser.AddDays(1);
                        }
                    }
                    break;
            }

            return Json(result);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("firstAdminOrderButton")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> FirstAdminOrderButton(int id, OrderModel model)
        {
            IFiluetAdminOrderButtons serviceButtons = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                serviceButtons = serviceScope.ServiceProvider.GetService<IFiluetAdminOrderButtons>();
            }

            if (serviceButtons == null)
            {
                _notificationService.Notification(NotifyType.Warning, "Not found admin order button");
                return RedirectToAction("Edit", "Order", new { id });
            }

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                _notificationService.Notification(NotifyType.Warning, "Not found information to send in 1S");
                return RedirectToAction("List");
            }

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            await serviceButtons.FirstButton(order, model);          

            return RedirectToAction("Edit", "Order", new { id });
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnReSubmitToOracle")]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> ReSubmitToOracle(int id, OrderModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("Edit", "Order", new { id });

            try
            {
                await _fusionIntegrationService.ReSubmitOrderAsync(order);

                await _orderService.InsertOrderNoteAsync(new OrderNote
                {
                    Note = "ReSubmit Manual SUCCESS",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    OrderId = order.Id
                });

                _notificationService.Notification(NotifyType.Success, "Resubmit done");

            }
            catch 
            {
                _notificationService.Notification(NotifyType.Error, "Resubmit failed");
            }

            return RedirectToAction("Edit", "Order", new { id });
        }

        public override async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get an order with the specified id
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.Deleted)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null && !await HasAccessToOrderAsync(order))
                return RedirectToAction("List");

            //prepare model
            var model = await _orderModelFactory.PrepareOrderModelAsync(null, order);

            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Areas/Admin/Views/FiluetOrder/Edit.cshtml", model);
        }

        public override async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await PrepareOrderSearchModelAsync(new FiluetOrderSearchModel
            {
                OrderStatusIds = orderStatuses,
                PaymentStatusIds = paymentStatuses,
                ShippingStatusIds = shippingStatuses
            });

            return View("~/Plugins/ISDK.Filuet.OnlineOrdering.CorePlugin/Areas/Admin/Views/FiluetOrder/List.cshtml", model);
        }

        private async Task<FiluetOrderSearchModel> PrepareOrderSearchModelAsync(FiluetOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = await _workContext.GetCurrentVendorAsync() != null;
            searchModel.BillingPhoneEnabled = _addressSettings.PhoneEnabled;

            //prepare available order, payment and shipping statuses
            await _baseAdminModelFactory.PrepareOrderStatusesAsync(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            await _baseAdminModelFactory.PreparePaymentStatusesAsync(searchModel.AvailablePaymentStatuses);
            if (searchModel.AvailablePaymentStatuses.Any())
            {
                if (searchModel.PaymentStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                    searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
            }

            await _baseAdminModelFactory.PrepareShippingStatusesAsync(searchModel.AvailableShippingStatuses);
            if (searchModel.AvailableShippingStatuses.Any())
            {
                if (searchModel.ShippingStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.ShippingStatusIds.Select(id => id.ToString());
                    searchModel.AvailableShippingStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableShippingStatuses.FirstOrDefault().Selected = true;
            }

            //prepare available stores
            await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);

            //prepare available vendors
            await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);

            //prepare available warehouses
            await _baseAdminModelFactory.PrepareWarehousesAsync(searchModel.AvailableWarehouses);

            //prepare available payment methods
            searchModel.AvailablePaymentMethods = (await _paymentPluginManager.LoadAllPluginsAsync()).Select(method =>
                new SelectListItem { Text = method.PluginDescriptor.FriendlyName, Value = method.PluginDescriptor.SystemName }).ToList();
            searchModel.AvailablePaymentMethods.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = string.Empty });

            //prepare available billing countries
            searchModel.AvailableCountries = (await _countryService.GetAllCountriesForBillingAsync(showHidden: true))
                .Select(country => new SelectListItem { Text = country.Name, Value = country.Id.ToString() }).ToList();
            searchModel.AvailableCountries.Insert(0, new SelectListItem { Text = await _localizationService.GetResourceAsync("Admin.Common.All"), Value = "0" });

            //prepare grid
            searchModel.SetGridPageSize();

            searchModel.HideStoresList = _catalogSettings.IgnoreStoreLimitations || searchModel.AvailableStores.SelectionIsNotPossible();

            return searchModel;
        }

        [NonAction]
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task<IActionResult> OrderList(OrderSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _orderModelFactory.PrepareOrderListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> OrderList(FiluetOrderSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await PrepareOrderListModelAsync(searchModel);

            return Json(model);
        }

        protected async Task<OrderListModel> PrepareOrderListModelAsync(FiluetOrderSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter orders
            var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var paymentStatusIds = (searchModel.PaymentStatusIds?.Contains(0) ?? true) ? null : searchModel.PaymentStatusIds.ToList();
            var shippingStatusIds = (searchModel.ShippingStatusIds?.Contains(0) ?? true) ? null : searchModel.ShippingStatusIds.ToList();
            if (await _workContext.GetCurrentVendorAsync() != null)
                searchModel.VendorId = (await _workContext.GetCurrentVendorAsync()).Id;
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);
            var product = await _productService.GetProductByIdAsync(searchModel.ProductId);
            var filterByProductId = product != null && (await _workContext.GetCurrentVendorAsync() == null || product.VendorId == (await _workContext.GetCurrentVendorAsync()).Id)
                ? searchModel.ProductId : 0;

            //get orders
            var orders = await _filuetOrderService.SearchOrdersAsync(storeId: searchModel.StoreId,
                vendorId: searchModel.VendorId,
                productId: filterByProductId,
                warehouseId: searchModel.WarehouseId,
                paymentMethodSystemName: searchModel.PaymentMethodSystemName,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                billingPhone: searchModel.BillingPhone,
                billingEmail: searchModel.BillingEmail,
                billingLastName: searchModel.BillingLastName,
                billingCountryId: searchModel.BillingCountryId,
                orderNotes: searchModel.OrderNotes,
                containOrderNumber: searchModel.CustomOrderNumber,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new OrderListModel().PrepareToGridAsync(searchModel, orders, () =>
            {
                //fill in model values from the entity
                return orders.SelectAwait(async order =>
                {
                    var billingAddress = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

                    //fill in model values from the entity
                    var orderModel = new OrderModel
                    {
                        Id = order.Id,
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatusId = order.PaymentStatusId,
                        ShippingStatusId = order.ShippingStatusId,
                        CustomerEmail = billingAddress.Email,
                        CustomerFullName = $"{billingAddress.FirstName} {billingAddress.LastName}",
                        CustomerId = order.CustomerId,
                        CustomOrderNumber = order.CustomOrderNumber
                    };

                    //convert dates to the user time
                    orderModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = (await _storeService.GetStoreByIdAsync(order.StoreId))?.Name ?? "Deleted";
                    orderModel.OrderStatus = await _localizationService.GetLocalizedEnumAsync(order.OrderStatus);
                    orderModel.PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus);
                    orderModel.ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus);
                    orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(order.OrderTotal, true, false);

                    return orderModel;
                });
            });

            return model;
        }

        #endregion
    }
}

