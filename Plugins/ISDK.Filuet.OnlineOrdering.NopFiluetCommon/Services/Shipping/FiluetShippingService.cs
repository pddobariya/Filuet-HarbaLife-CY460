using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.CustomOrders;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping
{
    public class FiluetShippingService : IFiluetShippingService
    {
        #region Fields 

        private readonly IRepository<FiluetFusionShippingComputationOption> _filuetFusionShippingComputationOptionRepository;
        private readonly IRepository<FiluetFusionShippingComputationOptionCustomerData> _filuetFusionShippingComputationOptionCustomerDataRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly IPluginService _pluginService;

        #endregion

        #region Ctor 

        public FiluetShippingService(
            IRepository<FiluetFusionShippingComputationOption> filuetFusionShippingComputationOptionRepository,
            IRepository<FiluetFusionShippingComputationOptionCustomerData> filuetFusionShippingComputationOptionCustomerDataRepository,
             IGenericAttributeService genericAttributeService,
             ISettingService settingService,
             IStoreContext storeContext,
             IWorkContext workContext,
             IStaticCacheManager cacheManager,
             IOrderService orderService,
             IShoppingCartService shoppingCartService,
             IProductService productService,
             IPluginService pluginService)
        {
            _filuetFusionShippingComputationOptionRepository = filuetFusionShippingComputationOptionRepository;
            _filuetFusionShippingComputationOptionCustomerDataRepository = filuetFusionShippingComputationOptionCustomerDataRepository;
            _genericAttributeService = genericAttributeService;
            _settingService = settingService;
            _storeContext = storeContext;
            _workContext = workContext;
            _cacheManager = cacheManager;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _pluginService = pluginService;
        }

        #endregion

        #region Utility

        private static object syncSelectedObject = new object();
        private CacheKey GetCacheKey(int customerId, string s)
        {
            return new CacheKey(string.Format(
                NopFiluetCommonDefaults.FiluetFusionShippingComputationOptionCustomerDataCacheKey,
                customerId.ToString()) + s);
        }

        #endregion

        #region Methods 

        public async Task<FiluetFusionShippingComputationOption> GetShippingComputationOptionByCustomerIdAsync(int customerId)
        {
            var key = GetCacheKey(customerId, "GetShippingComputationOptionByCustomerIdAsync");
            return await Task.FromResult(_cacheManager.Get(key,
                () =>
                {
                    lock (syncSelectedObject)
                    {
                        return _filuetFusionShippingComputationOptionCustomerDataRepository.Table.Join(
                                    _filuetFusionShippingComputationOptionRepository.Table,
                                    x => x.FiluetFusionShippingComputationOptionId, x => x.Id,
                                    (x, y) => new { ffshcocd = x, ffshco = y })
                                .Where(p => p.ffshcocd.CustomerId == customerId && !p.ffshco.Deleted && p.ffshcocd.IsSelected)
                                .Select(x => x.ffshco).FirstOrDefault();
                    }
                }));
        }

        public async Task<FiluetFusionShippingComputationOption> GetShippingComputationOptionByIdAsync(int id)
        {
            return (await GetAllShippingComputationOptionsAsync()).First(ffshco => ffshco.Id == id);
        }

        public async Task<List<FiluetFusionShippingComputationOption>> GetAllShippingComputationOptionsAsync()
        {
            return await _cacheManager.GetAsync(NopEntityCacheDefaults<FiluetFusionShippingComputationOption>.AllCacheKey,
                () => _filuetFusionShippingComputationOptionRepository.Table.Where(p => !p.Deleted).ToList());
        }

        public async Task<List<FiluetFusionShippingComputationOption>> GetShippingComputationOptionsByContriesAsync(string[] contries = null)
        {
            if (contries != null && contries.Length > 0)
            {
                return (await GetAllShippingComputationOptionsAsync())
                    .Where(p => contries.Contains(p.CountryCode))
                    .ToList();
            }
            else
            {
                return await GetAllShippingComputationOptionsAsync();
            }
        }

        public async Task<List<FiluetFusionShippingComputationOptionCustomerData>> GetShippingComputationOptionCustomerDataListByCustomerIdAsync(int customerId)
        {
            var key = GetCacheKey(customerId, "O");
          
            var featuredProductIds = await _cacheManager.GetAsync(key, async () =>
            {
                var filuetFusionShippingComputationOptionCustomerData1 = from p in _filuetFusionShippingComputationOptionCustomerDataRepository.Table
                                                                          join c in _filuetFusionShippingComputationOptionRepository.Table on p.FiluetFusionShippingComputationOptionId equals c.Id
                                                                          where p.CustomerId == customerId && c.Deleted == false
                                                                          select p;
                return await filuetFusionShippingComputationOptionCustomerData1.ToListAsync();
            });

            return featuredProductIds;
             


        }

        public async Task<FiluetFusionShippingComputationOptionCustomerData> AddShippingComputationOptionCustomerDataAsync(FiluetFusionShippingComputationOptionCustomerData customerOption)
        {
            if (customerOption.IsSelected)
            {
                await DeselectAllShippingComputationOptionsOfCustomerAsync(customerOption);
            }
            var key = GetCacheKey(customerOption.CustomerId, "L");

            var cached = await GetShippingComputationOptionCustomerDataListByCustomerIdAsync(customerOption.CustomerId);
            

            _filuetFusionShippingComputationOptionCustomerDataRepository.Insert(customerOption);

            cached.Add(customerOption);

            await _cacheManager.SetAsync(key, cached);

            return customerOption;
        }

        public async Task<FiluetFusionShippingComputationOptionCustomerData> UpdateShippingComputationOptionCustomerDataaAsync(FiluetFusionShippingComputationOptionCustomerData customerOption)
        {
            if (customerOption.IsSelected)
            {
                await DeselectAllShippingComputationOptionsOfCustomerAsync(customerOption);
            }

            var customerOptions = await GetShippingComputationOptionCustomerDataListByCustomerIdAsync(customerOption.CustomerId);
            var index = customerOptions.FindIndex(co => co.Id == customerOption.Id);

            _filuetFusionShippingComputationOptionCustomerDataRepository.Update(customerOption);
            customerOptions[index] = customerOption;
            var key = GetCacheKey(customerOption.CustomerId, "L-1");
            await _cacheManager.SetAsync(key, customerOptions);
            return customerOption;
        }

        public async Task<FiluetFusionShippingComputationOptionCustomerData> GetShippingComputationOptionCustomerDataByCustomerIdAndOptionIdAsync(
            int customerId, int optionId)
        {
            return (await GetShippingComputationOptionCustomerDataListByCustomerIdAsync(customerId))
                .FirstOrDefault(p => p.FiluetFusionShippingComputationOptionId == optionId);
        }

        public async Task<FiluetFusionShippingComputationOptionCustomerData> GetSelectedShippingComputationOptionCustomerDataAsync(int customerId)
        {
            return (await GetShippingComputationOptionCustomerDataListByCustomerIdAsync(customerId))
                .FirstOrDefault(p => p.IsSelected);
        }

        public async Task<FiluetFusionShippingComputationOptionModel> GetSelectedShippingComputationOptionModelAsync(Customer customer, Order order = null)
        {
            var filuetFusionShippingComputationOptionCustomerData = await GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
            var settings = await _settingService.LoadSettingAsync<FiluetCorePluginSettings>(await _storeContext.GetActiveStoreScopeConfigurationAsync());
            var wareHouse = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.SelectedShippingWareHouse) ?? (filuetFusionShippingComputationOptionCustomerData != null ? (await GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData.FiluetFusionShippingComputationOptionId))?.WarehouseCode.Split(';')[0] : null) ?? settings.Warehouse;
            var freightCode = await GetFreightCodeAsync(customer, order);
            return new FiluetFusionShippingComputationOptionModel
            {
                CountryCode = (filuetFusionShippingComputationOptionCustomerData != null ? await GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData.FiluetFusionShippingComputationOptionId) : null)?.CountryCode ?? settings.CountryCode,
                WarehouseCode = string.IsNullOrEmpty(wareHouse) ? settings.Warehouse : wareHouse,
                ProcessingLocationCode = (filuetFusionShippingComputationOptionCustomerData != null ? await GetShippingComputationOptionByIdAsync(filuetFusionShippingComputationOptionCustomerData.FiluetFusionShippingComputationOptionId) : null)?.ProcessingLocationCode ?? settings.ProcessingLocationCode,
                FreightCode = freightCode,
                City = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.City),
                Address = freightCode == FreightCodes.NOF ? await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.NofAddress) : await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.Address),
                PhoneNumber = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.SelectedShippingPhoneNumber)
            };
        }

        public async Task<string> GetFreightCodeAsync(Customer customer, Order order = null)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            ICustomOrderService customOrderService = null;
            var serviceScopeFactory = EngineContext.Current.Resolve<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                customOrderService = serviceScope.ServiceProvider.GetService<ICustomOrderService>();
            }
            var customFreightCode = await customOrderService?.GetCustomFreightCodeAsync(order, customer);
            if (!string.IsNullOrWhiteSpace(customFreightCode))
            {
                return customFreightCode;
            }

            List<Product> orderedProducts;
            CategoryTypeEnum orderType;

            if (order != null)
            {
                orderType = await order.GetOrderTypeAsync();
                orderedProducts = await (await _orderService.GetOrderItemsAsync(order.Id)).SelectAwait(async x => (await _orderService.GetProductByOrderItemIdAsync(x.Id))).ToListAsync();
            }
            else
            {
                var cartItems = (await _shoppingCartService.GetShoppingCartAsync(customer)).ToList();
                orderType = await cartItems.GetOrderTypeAsync();
                orderedProducts = await cartItems.SelectAwait(async x => await _productService.GetProductByIdAsync(x.ProductId)).ToListAsync();
            }

            var isNof = orderType == CategoryTypeEnum.Maintenance || orderType == CategoryTypeEnum.Ticket;
            if (isNof)
            {
                return FreightCodes.NOF;
            }

            string freightCode = FreightCodes.PU;
            var selectedShippingMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.SelectedShippingMethodSystemName);
            if (!string.IsNullOrEmpty(selectedShippingMethodSystemName))
            {
                var shippingPlugin = (await _pluginService.GetPluginsAsync<IFusionShippingProvider>(customer: customer)).ToList().FirstOrDefault(p => p.PluginDescriptor.SystemName == selectedShippingMethodSystemName);
                if (shippingPlugin != null)
                {
                    await _workContext.SetCurrentCustomerAsync(customer);
                    freightCode = await shippingPlugin.FreightCode;
                }
            }

            return freightCode;
        }

        public async Task<string> GetFreeShippingSkuAsync(Customer customer)
        {
            var selectedCustomerOption = await GetSelectedShippingComputationOptionCustomerDataAsync(customer.Id);
            switch (selectedCustomerOption.FiluetFusionShippingComputationOption.CountryCode)
            {
                case CountryCodes.LV:
                    return FreeShippingSKUs.LV;
                case CountryCodes.EE:
                    return FreeShippingSKUs.EE;
                case CountryCodes.LT:
                    return FreeShippingSKUs.LT;
                default:
                    return FreeShippingSKUs.LV;
            }
        }

        public async Task<List<string>> GetFreeShippingSkusAsync()
        {
            return await Task.FromResult(new List<string>()
            {
                FreeShippingSKUs.LV,
                FreeShippingSKUs.EE,
                FreeShippingSKUs.LT
            });
        }

        public async Task<string> GetWareHouse()
        {
            return (await GetSelectedShippingComputationOptionModelAsync(await _workContext.GetCurrentCustomerAsync())).WarehouseCode;
        }

        private async Task DeselectAllShippingComputationOptionsOfCustomerAsync(FiluetFusionShippingComputationOptionCustomerData customerOption)
        {
            var optionsForCustomer = await GetShippingComputationOptionCustomerDataListByCustomerIdAsync(customerOption.CustomerId);
            var selectedOptions = await optionsForCustomer
               .WhereAwait(async p => p.IsSelected
                       && p.Id != customerOption.Id && !(await GetShippingComputationOptionByIdAsync(p.FiluetFusionShippingComputationOptionId)).Deleted)
               .ToListAsync();

            foreach (var selectedOption in selectedOptions)
            {
                selectedOption.IsSelected = false;
                _filuetFusionShippingComputationOptionCustomerDataRepository.Update(selectedOption);
            }
        }

        private async Task<bool> IsShippingInformationProviderAsync(Customer customer)
        {
            var selectedShippingMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer, CustomerAttributeNames.SelectedShippingMethodSystemName);
            var shippingInformationProviders = (await _pluginService.GetPluginsAsync<IShippingInformationProvider>(customer: customer, storeId: 0)).ToList();
            return shippingInformationProviders.Any(p => p.PluginDescriptor.SystemName == selectedShippingMethodSystemName);
        }

        public async Task<string> GetWareHouseAsync()
        {
            return (await GetSelectedShippingComputationOptionModelAsync(await _workContext.GetCurrentCustomerAsync())).WarehouseCode;
        }

        #endregion
    }
}
