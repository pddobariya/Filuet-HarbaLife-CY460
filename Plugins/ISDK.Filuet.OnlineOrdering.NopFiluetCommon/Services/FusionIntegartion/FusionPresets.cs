using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Fusion;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.DualMonth;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.FusionIntegartion
{
    public class FusionPresets : IFusionPresets
    {
        #region Fileds 

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IDualMonthsService _dualMonthsService;
        private readonly IDiscountService _discountService;
        private readonly FiluetCorePluginSettings _settings;
        private readonly IFiluetShippingService _filuetShippingService;
        private readonly IWorkContext _workContext;
        private readonly IDistributorService _distributorService;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;

        #endregion

        #region Ctor

        public FusionPresets(IGenericAttributeService genericAttributeService,
                             IDualMonthsService dualMonthsService,
                             IFiluetShippingService filuetShippingService,
                             IWorkContext workContext,
                             IDistributorService distributorService,
                             IDiscountService discountService,
                             FiluetCorePluginSettings settings,
                             IStoreContext storeContext,
                             ICustomerService customerService,
                             IShoppingCartService shoppingCartService)
        {
            _genericAttributeService = genericAttributeService;
            _dualMonthsService = dualMonthsService;
            _filuetShippingService = filuetShippingService;
            _workContext = workContext;
            _distributorService = distributorService;
            _discountService = discountService;
            _settings = settings;
            _storeContext = storeContext;
            _customerService = customerService;
            _shoppingCartService = shoppingCartService;
        }

        #endregion

        #region Methods 

        public async Task<FusionServiceParams> GetFusionCallParamsAsync(Customer customer, Order order = null)
        {
            var shippingCalcOpt = await _filuetShippingService.GetSelectedShippingComputationOptionModelAsync(customer, order);
            var orderCategory = await GetOrderSpecificCategoryAsync(customer, order);
            var distributorDetailedProfile = await _distributorService.GetDistributorDetailedProfileAsync(customer);

            bool? invoiceWithShipment = await _genericAttributeService.HasCustomAttributeAsync(customer, CustomerAttributeNames.IsShipInvoiceWithOrder)
                ? await _genericAttributeService.GetAttributeAsync<bool?>(customer,CustomerAttributeNames.IsShipInvoiceWithOrder) : null;
            string orderMonthStr = order == null ? await _dualMonthsService.GetOrderMonthOfCustomerAsync(customer) : await _dualMonthsService.GetOrderMonthOfOrderAsync(order);
            string shipCity = shippingCalcOpt.City ?? distributorDetailedProfile?.Addresses?.MailingAddress?.City;
            string shipZip = await _genericAttributeService.GetAttributeAsync<string>(customer,OrderAttributeNames.SelectedShippingZipCode);//shippingCalcOpt.ShippingZipCode ?? distributorDetailedProfile?.Addresses?.MailingAddress?.PostalCode;
            string shipAddress = shippingCalcOpt.Address ?? distributorDetailedProfile?.Addresses?.MailingAddress?.FullAddress;
            string shipPostamat = await _genericAttributeService.GetAttributeAsync<string>(customer,NopCustomerDefaults.SelectedPickupPointAttribute);
            string shipTime = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DeliveryTime);
            string shipPhone = shippingCalcOpt.PhoneNumber ?? await _genericAttributeService.GetAttributeAsync<string>(customer,"Phone")
                ?? "371-1234567";
            if (!shipPhone.Contains('-'))
            {
                shipPhone = shipPhone.Insert(0, "-");
            }
            string recentOrderNumber = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.RecentGetShoppingCartTotalOrderNumber);
            var fullName =
                await _genericAttributeService.GetAttributeAsync<string>(customer,
                    CustomerAttributeNames.SelectedShippingFullname);
            var discounts =  await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToShipping);

            if (await discounts.AnyAwaitAsync(async x=> (await _discountService.ValidateDiscountAsync(x, customer)).IsValid && x.DiscountPercentage == Convert.ToDecimal(100)))
            {
                if (!string.IsNullOrWhiteSpace(_settings.FreightCodeForDiscountAMB)) shippingCalcOpt.FreightCode = _settings.FreightCodeForDiscountAMB.Trim();
            }

            return new FusionServiceParams
            {
                DistributorId = await customer.GetDistributorIdAsync(),
                ProcessingLocationCode = string.IsNullOrWhiteSpace(shippingCalcOpt.ProcessingLocationCode) ? null : shippingCalcOpt.ProcessingLocationCode,
                WarehouseCode = string.IsNullOrWhiteSpace(shippingCalcOpt.WarehouseCode) ? null : shippingCalcOpt.WarehouseCode,
                FreightCode = string.IsNullOrWhiteSpace(shippingCalcOpt.FreightCode) ? null : shippingCalcOpt.FreightCode,
                OrderMonth = string.IsNullOrWhiteSpace(orderMonthStr) ? null : orderMonthStr,
                CountryCode = string.IsNullOrWhiteSpace(shippingCalcOpt.CountryCode) ? null : shippingCalcOpt.CountryCode.ToCountryCode(),
                City = string.IsNullOrWhiteSpace(shipCity) ? null : shipCity,
                Zipcode = string.IsNullOrWhiteSpace(shipZip) ? null : shipZip,
                Address = string.IsNullOrWhiteSpace(shipAddress) ? null : shipAddress,
                PostamatId = string.IsNullOrWhiteSpace(shipPostamat) ? null : shipPostamat,
                TimeSlot = string.IsNullOrWhiteSpace(shipTime) ? null : shipTime,
                PhoneNumber = string.IsNullOrWhiteSpace(shipPhone) ? null : shipPhone,
                Fullname = fullName ?? await customer.GetFullNameAsync(),
                OrderNumber = string.IsNullOrWhiteSpace(recentOrderNumber) ? null : recentOrderNumber,
                OrderCategory = orderCategory,
                OrderType = NopFiluetCommon.Constants.OrderTypes.IE,
                OrderTypeId = await GetOrderTypeAsync(orderCategory, shippingCalcOpt.CountryCode, GetOrderTypeId(shippingCalcOpt), customer),
                InvoiceWithShipment = invoiceWithShipment
            };
        }

        private async Task<string> GetOrderSpecificCategoryAsync(Customer customer, Order order)
        {
            if (order != null)
            {
                return await GetOrderCategoryasync(order);
            }

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer,
                  ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);
          
            if (cart != null && cart.Any())
            {
                return await GetOrderCategoryAsync(customer);
            }

            return null;
        }

        private async Task<string> GetOrderCategoryasync(Order order)
        {
            CategoryTypeEnum categoryType = await order.GetOrderTypeAsync();
            return MapOrderCategory(categoryType, await _customerService.GetCustomerByIdAsync(order.CustomerId) ?? await _workContext.GetCurrentCustomerAsync());
        }

        private async Task<string> GetOrderCategoryAsync(Customer customer)
        {
            var shoppingCartItems = await _shoppingCartService.GetShoppingCartAsync(customer,
                  ShoppingCartType.ShoppingCart, storeId: (await _storeContext.GetCurrentStoreAsync()).Id);

            CategoryTypeEnum categoryType = await (await shoppingCartItems.ToListAsync()).GetOrderTypeAsync();
            return MapOrderCategory(categoryType, customer);
        }

        private string MapOrderCategory(CategoryTypeEnum categoryType, Customer customer)
        {
            switch (categoryType)
            {
                case CategoryTypeEnum.Maintenance:
                    return OrderCategories.APF;
                case CategoryTypeEnum.BIZWORK:
                    return OrderCategories.ETO;
                case CategoryTypeEnum.Ticket:
                    return OrderCategories.RSO;
            }
            return OrderCategories.RSO;
        }

        private async Task<string> GetOrderTypeAsync(string orderCategory, string shipCountry, string defaultOrderTypeId, Customer customer)
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

            if (orderCategory != OrderCategories.APF || ! await customer.IsCustomerFromBalticCountryAsync())
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
            }

            return null;
        }

        #endregion
    }
}