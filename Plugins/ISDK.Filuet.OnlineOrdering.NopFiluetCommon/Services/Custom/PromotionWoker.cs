using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;
using ProxyServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    class PromotionWoker : IPromotionWoker
    {
        #region Fields
        IGenericAttributeService _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        ICustomerService _customerService = EngineContext.Current.Resolve<ICustomerService>();
        ISettingsLoader _settingsLoader = EngineContext.Current.Resolve<ISettingsLoader>();
        //private readonly IGenericAttributeService _genericAttributeService;
        private List<Promotion> _promotions = new List<Promotion>();
        //private readonly ICustomerService _customerService;
        //private readonly ISettingsLoader _settingsLoader;

        #endregion

        #region Methods

        public PromotionWoker(IOrderService orderService, IProductService productService, IGenericAttributeService genericAttributeService, ISettingsLoader settingsLoader = null)
        {
            _genericAttributeService = genericAttributeService;

            _promotions.Add(new Promotion
            {
                Name = "CozyHome",
                StartDateTime = new DateTime(2020, 11, 16),
                EndDateTime = new DateTime(2020, 11, 30),
                PromotionAction = async order =>
                {
                    var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

                    if (/*product == null || */ await _genericAttributeService.GetAttributeAsync<bool>(customer, "CozyHome") || await _genericAttributeService.GetAttributeAsync<double>(customer, OrderAttributeNames.VolumePoints) < 250
                    || await _genericAttributeService.GetAttributeAsync<DistributorType>(customer, CustomerAttributeNames.DistributorType) == DistributorType.Supervisor
                    || await _genericAttributeService.GetAttributeAsync<double>(customer, OrderAttributeNames.DiscountPercent) >= 50)

                        return false;

                    await orderService.InsertOrderNoteAsync(new OrderNote
                    {
                        Note = "Promotion \"Уютный дом\" accomplished",
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });

                    await orderService.UpdateOrderAsync(order);
                    return true;
                }
            });
            _settingsLoader = settingsLoader;
        }


        public string[] PromotionNames => _promotions.Select(p => p.Name).ToArray();

        public async Task RunAsync(Order order)
        {
            var hoursShift =  await _settingsLoader.GetHoursShift();
            foreach (var promotion in _promotions)
            {
                if (promotion.StartDateTime <= DateTime.UtcNow.AddHours(hoursShift).Date && promotion.EndDateTime >= DateTime.UtcNow.AddHours(hoursShift).Date)
                {
                    if (await promotion.PromotionAction(order))
                    {
                        await _genericAttributeService.SaveAttributeAsync((await _customerService.GetCustomerByIdAsync(order.CustomerId)), promotion.Name, true);
                        await _genericAttributeService.SaveAttributeAsync(order, promotion.Name, true);
                    }
                }
                else
                {
                  await _genericAttributeService.SaveAttributeAsync<bool?>(await _customerService.GetCustomerByIdAsync(order.CustomerId), promotion.Name, true);
                }
            }
        }

        #endregion
    }
}
