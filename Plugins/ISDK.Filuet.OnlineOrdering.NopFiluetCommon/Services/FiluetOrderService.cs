using ISDK.Filuet.OnlineOrdering.FiluetCommon.Enums;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services
{
    public class FiluetOrderService : IFiluetOrderService
    {
        #region Fields
        private readonly IRepository<Order> _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IAddressService _addressService;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductWarehouseInventory> _productWarehouseInventoryRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Address> _addressRepository;
        #endregion

        #region Ctor
        public FiluetOrderService(
    IRepository<Order> orderRepository,
    IOrderService orderService,
    IProductService productService,
    IAddressService addressService,
    IRepository<OrderItem> orderItemRepository,
    IRepository<Product> productRepository,
    IRepository<ProductWarehouseInventory> productWarehouseInventoryRepository,
    IRepository<OrderNote> orderNoteRepository,
    IRepository<Address> addressRepository
    )
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _productService = productService;
            _addressService = addressService;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
            _productWarehouseInventoryRepository = productWarehouseInventoryRepository;
            _orderNoteRepository = orderNoteRepository;
            _addressRepository = addressRepository;
    }
        #endregion

        #region Methods

        public IPagedList<Order> SearchOrders(int storeId = 0, int vendorId = 0, int customerId = 0, int productId = 0, int affiliateId = 0, int warehouseId = 0, int billingCountryId = 0, string paymentMethodSystemName = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null, string billingPhone = null, string billingEmail = null, string billingLastName = "", string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, OrderSortingEnum? orderBy = OrderSortingEnum.Position, string fusionOrderId = null)
        {
            var query = _orderRepository.Table;
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (vendorId > 0)
                query = query.ToArray().Where(o => _orderService.GetOrderItemsAsync(o.Id).Result.Any(orderItem => _productService.GetProductByIdAsync(orderItem.ProductId).Result.VendorId == vendorId)).AsQueryable();
            if (productId > 0)
                query = query.ToArray().Where(o => _orderService.GetOrderItemsAsync(o.Id).Result.Any(orderItem => orderItem.ProductId == productId)).AsQueryable();

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query.ToArray()
                    .Where(o => _orderService.GetOrderItemsAsync(o.Id).Result
                    .Any(orderItem =>
                    {
                        var product = _productService.GetProductByIdAsync(orderItem.ProductId).Result;
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        return (product.ManageInventoryMethodId ==
                         manageStockInventoryMethodId &&
                         product.UseMultipleWarehouses &&
                         _productService.GetAllProductWarehouseInventoryRecordsAsync(product.Id).Result.Any(pwi => pwi.WarehouseId == warehouseId))
                            ||
                            //"Use multiple warehouses" disabled
                            //we use standard "warehouse" property
                            ((product.ManageInventoryMethodId != manageStockInventoryMethodId ||
                              !product.UseMultipleWarehouses) &&
                             product.WarehouseId == warehouseId);
                    })).AsQueryable();
            }

            if (billingCountryId > 0)
                query = query.ToArray().Where(o =>
                {
                    var address = _addressService.GetAddressByIdAsync(o.BillingAddressId).Result;
                    return address != null &&
                           address.CountryId == billingCountryId;
                }).AsQueryable();
            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (!string.IsNullOrEmpty(billingPhone))
                query = query.ToArray().Where(o =>
                {
                    var address = _addressService.GetAddressByIdAsync(o.BillingAddressId).Result;
                    return address != null && !string.IsNullOrEmpty(address.PhoneNumber) &&
                           address.PhoneNumber.Contains(billingPhone);
                }).AsQueryable();
            if (!string.IsNullOrEmpty(billingEmail))
                query = query.ToArray().Where(o =>
                {
                    var address = _addressService.GetAddressByIdAsync(o.BillingAddressId).Result;
                    return address != null && !string.IsNullOrEmpty(address.Email) &&
                           address.Email.Contains(billingEmail);
                }).AsQueryable();
            if (!string.IsNullOrEmpty(billingLastName))
                query = query.ToArray().Where(o =>
                {
                    var address = _addressService.GetAddressByIdAsync(o.BillingAddressId).Result;
                    return address != null && !string.IsNullOrEmpty(address.LastName) &&
                           address.LastName.Contains(billingLastName);
                }).AsQueryable();
            if (!string.IsNullOrEmpty(orderNotes))
                query = query.ToArray().Where(o =>
                {
                    return _orderService.GetOrderNotesByOrderIdAsync(o.Id).Result.Any(on => on.Note.Contains(orderNotes));
                }).AsQueryable();
            query = query.Where(o => !o.Deleted);

            query = query.OrderByDescending(o => o.CreatedOnUtc);

            if (orderBy == OrderSortingEnum.CreatedOnAsc)
                query = query.OrderBy(o => o.CreatedOnUtc);
            if (orderBy == OrderSortingEnum.CreatedOnDesc)
                query = query.OrderByDescending(o => o.CreatedOnUtc);
            if (!string.IsNullOrWhiteSpace(fusionOrderId))
            {
                query = query.ToArray().Where(o =>
                {
                    return o.GetFusionOrderNumberAsync().Result == fusionOrderId;
                }).AsQueryable();
            }
            //database layer paging
            return new PagedList<Order>(query.ToList(), pageIndex, pageSize);
        }

        public async Task<Order> GetOrderByAuthorizationTransactionIdAsync(string authorizationTransactionId)
        {
            var orders = await _orderRepository.GetAllAsync(o => o.Where(s => s.AuthorizationTransactionId == authorizationTransactionId));

            return orders.FirstOrDefault();
        }

        public async Task<Order> GetOrderByAuthorizationTransactionIdAndPaymentMethod(string authorizationTransactionId, string paymentMethodSystemName)
        {
            var orders = await _orderRepository.GetAllAsync(o => o.Where(s => s.AuthorizationTransactionId == authorizationTransactionId && s.PaymentMethodSystemName == paymentMethodSystemName));
            var sortedOrders = orders.OrderByDescending(o=> o.CreatedOnUtc);
            var order = sortedOrders.FirstOrDefault();
            return order;
        }

        public async Task<IPagedList<Order>> SearchOrdersAsync(int storeId = 0, int vendorId = 0, int customerId = 0, int productId = 0, int affiliateId = 0, int warehouseId = 0, int billingCountryId = 0, string paymentMethodSystemName = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null, string billingPhone = null, string billingEmail = null, string billingLastName = "", string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, string containOrderNumber = null)
        {
            var query = _orderRepository.Table;

            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);

            if (vendorId > 0)
            {
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        where p.VendorId == vendorId
                        select o;

                query = query.Distinct();
            }

            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (productId > 0)
                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        where oi.ProductId == productId
                        select o;

            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;

                query = from o in query
                        join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                        join p in _productRepository.Table on oi.ProductId equals p.Id
                        join pwi in _productWarehouseInventoryRepository.Table on p.Id equals pwi.ProductId into ps
                        from pwi in ps.DefaultIfEmpty()
                        where
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (p.ManageInventoryMethodId == manageStockInventoryMethodId && p.UseMultipleWarehouses && pwi.WarehouseId == warehouseId) ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((p.ManageInventoryMethodId != manageStockInventoryMethodId || !p.UseMultipleWarehouses) && p.WarehouseId == warehouseId)
                        select o;
            }

            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);

            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);

            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));

            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));

            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => _orderNoteRepository.Table.Any(oNote => oNote.OrderId == o.Id && oNote.Note.Contains(orderNotes)));

            query = from o in query
                    join oba in _addressRepository.Table on o.BillingAddressId equals oba.Id
                    where
                        (billingCountryId <= 0 || (oba.CountryId == billingCountryId)) &&
                        (string.IsNullOrEmpty(billingPhone) || (!string.IsNullOrEmpty(oba.PhoneNumber) && oba.PhoneNumber.Contains(billingPhone))) &&
                        (string.IsNullOrEmpty(billingEmail) || (!string.IsNullOrEmpty(oba.Email) && oba.Email.Contains(billingEmail))) &&
                        (string.IsNullOrEmpty(billingLastName) || (!string.IsNullOrEmpty(oba.LastName) && oba.LastName.Contains(billingLastName)))
                    select o;

            if (!string.IsNullOrEmpty(containOrderNumber))
            {
                query = query.Where(o => o.CustomOrderNumber.Contains(containOrderNumber));
            }

            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        #endregion
    }
}
