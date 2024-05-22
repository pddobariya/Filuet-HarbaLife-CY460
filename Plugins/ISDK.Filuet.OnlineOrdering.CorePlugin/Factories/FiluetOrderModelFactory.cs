using ISDK.Filuet.OnlineOrdering.CorePlugin.Helpers;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Models.Order;
using ISDK.Filuet.OnlineOrdering.CorePlugin.Services;
using ISDK.Filuet.OnlineOrdering.FiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Factories
{
    public class FiluetOrderModelFactory : IFiluetOrderModelFactory
    {
        #region Fields

        private readonly IFiluetOrderService _filuetOrderService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        private readonly IOrderStatusService _orderStatusService;
        private readonly IStatusService _statusService;  
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public FiluetOrderModelFactory(
            IFiluetOrderService filuetOrderService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper, 
            ILogger logger, 
            IOrderStatusService orderStatusService,
            IStatusService statusService,
            IGenericAttributeService genericAttributeService)
        {
            _filuetOrderService = filuetOrderService;
            _storeContext = storeContext;
            _workContext = workContext;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _webHelper = webHelper;
            _logger = logger;
            _orderStatusService = orderStatusService;
            _statusService = statusService;
            _genericAttributeService = genericAttributeService;
        }

        #endregion

        #region Methods

        public virtual async Task<FiluetCustomerOrderListModel> PrepareCustomerOrderListModel(OrderPagingFilteringModel command)
        {
            if (command == null)
                command = new OrderPagingFilteringModel();
            var model = new FiluetCustomerOrderListModel();

            PrepareSortingOptions(model.PagingFilteringContext, command);
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                true,
                "15,30,50",
                15);

            model.PagingFilteringContext.SearchButtonLocation =
                _webHelper.ModifyQueryString(_webHelper.RemoveQueryString(_webHelper.GetThisPageUrl(true), "q"), "q", "");

            model.PagingFilteringContext.q = command.q;

            var orders = _filuetOrderService.SearchOrders(storeId: _storeContext.GetCurrentStore().Id
                , customerId: (await _workContext.GetCurrentCustomerAsync()).Id
                , osIds: new List<int> {(int) OrderStatus.Processing, (int) OrderStatus.Complete}
                , orderBy: (OrderSortingEnum?)model.PagingFilteringContext.OrderBy
                , pageIndex: command.PageNumber - 1
                , pageSize: command.PageSize
                , fusionOrderId: command.q);

            foreach (var order in orders)
            {
                var orderModel = new FiluetCustomerOrderListModel.FiluetOrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone),
                    OrderStatusEnum = order.OrderStatus,
                    PaymentStatus = await _localizationService.GetLocalizedEnumAsync(order.PaymentStatus),
                    ShippingStatus = await _localizationService.GetLocalizedEnumAsync(order.ShippingStatus),
                    IsReturnRequestAllowed = await _orderProcessingService.IsReturnRequestAllowedAsync(order),
                    CustomOrderNumber = order.CustomOrderNumber,
                    FusionOrderNumber =await order.GetFusionOrderNumberAsync(),
                    VolumePoints = string.IsNullOrWhiteSpace(await _genericAttributeService.GetAttributeAsync<string>(order,OrderAttributeNames.VolumePoints)) ? "--" :await _genericAttributeService.GetAttributeAsync<string>(order,OrderAttributeNames.VolumePoints)
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, (await _workContext.GetWorkingLanguageAsync()).Id);

                model.Orders.Add(orderModel);
            }

            try
            {
                var orderIds = model.Orders.Select(order => order.Id).ToArray();
                var orderStatusDtos = (await _orderStatusService.GetOrderStatusesByOrderIdsAsync(orderIds)).OrderByDescending(p => p.StatusDate);
                var workingLanguage = await _workContext.GetWorkingLanguageAsync();
                var statusLocaleStrings = await _orderStatusService.GetStatusLocaleStringsAsync(workingLanguage.Id);
                var allStatuses = await _statusService.GetStatusesAsync(query => query);

                foreach (var filuetOrderDetailsModel in model.Orders)
                {
                    var statusId = orderStatusDtos.FirstOrDefault(p => p.OrderId == filuetOrderDetailsModel.Id)?.StatusId;

                    if (!statusId.HasValue)
                    {
                        filuetOrderDetailsModel.OrderStatus = "---";
                        filuetOrderDetailsModel.OrderStatusClass = "";

                        continue;
                    }


                    filuetOrderDetailsModel.OrderStatus = statusLocaleStrings.FirstOrDefault(p => p.StatusId == statusId.Value)?.StatusName;
                    var externalStatusName = allStatuses.FirstOrDefault(p => p.Id == statusId.Value)?.ExternalStatusName;
                    filuetOrderDetailsModel.OrderStatusClass = FiluetOrderStatusHelper.GetOrderStatusClass(externalStatusName);
                }
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Ошибка получения статусов заказов", e);
            }

            model.PagingFilteringContext.LoadPagedList(orders);

            return model;
        }

        private async void PrepareSortingOptions(OrderPagingFilteringModel pagingFilteringModel,
            OrderPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException(nameof(pagingFilteringModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            //set the order by position by default
            pagingFilteringModel.OrderBy = command.OrderBy;
            command.OrderBy = (int)OrderSortingEnum.Position;

            //get active sorting options
            var activeSortingOptionsIds = Enum.GetValues(typeof(OrderSortingEnum)).Cast<int>().ToList();
            if (!activeSortingOptionsIds.Any())
                return;

            //order sorting options
            var orderedActiveSortingOptions = activeSortingOptionsIds.ToList();

            pagingFilteringModel.AllowProductSorting = true;
            command.OrderBy = pagingFilteringModel.OrderBy ?? orderedActiveSortingOptions.FirstOrDefault();

            //prepare available model sorting options
            var currentPageUrl = _webHelper.RemoveQueryString(_webHelper.GetThisPageUrl(true), "q");
            foreach (var option in orderedActiveSortingOptions)
            {
                pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync((OrderSortingEnum)option),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "orderby", option.ToString()),
                    Selected = option == command.OrderBy
                });
            }
        }

        private void PreparePageSizeOptions(OrderPagingFilteringModel pagingFilteringModel, OrderPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException(nameof(pagingFilteringModel));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        if (int.TryParse(pageSizes.FirstOrDefault(), out int temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.RemoveQueryString(_webHelper.RemoveQueryString(currentPageUrl, "pagenumber"), "q");

                    foreach (var pageSize in pageSizes)
                    {
                        if (!int.TryParse(pageSize, out int temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = _webHelper.ModifyQueryString(sortUrl, "pagesize", pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.First().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        #endregion
    }
}
