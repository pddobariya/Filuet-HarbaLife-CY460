using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Factories
{
    public class SalesCenterModelFactory : ISalesCenterModelFactory
    {
        #region Fileds

        private readonly IDeliveryPriceService _deliveryPriceService;

        #endregion

        #region Ctor

        public SalesCenterModelFactory(IDeliveryPriceService deliveryPriceService)
        {
            _deliveryPriceService = deliveryPriceService;
        }

        #endregion

        #region Methods

        public virtual async Task<SalesCentersListModel> PrepareSalesCentersListModelAsync(SalesCenterSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            // Get salescenter
            var salescenter = (await _deliveryPriceService.GetSalesCentersAsync(searchModel.LanguageId)).ToPagedList(searchModel);

            // Prepare list model
            var model = new SalesCentersListModel().PrepareToGrid(searchModel, salescenter, () =>
            {
                // Fill in model values from the entity
                return salescenter.Select(salescenters =>
                {
                    var salesCentermodel = new SalesCenterDtoModel();
                    salesCentermodel.Id = salescenters.Id;
                    salesCentermodel.Name = salescenters.Name;
                    salesCentermodel.Address = salescenters.Address;
                    salesCentermodel.City = salescenters.City;
                    salesCentermodel.Price = salescenters.Price;
                    salesCentermodel.WarehouseCode = salescenters.WarehouseCode;
                    salesCentermodel.FreightCode = salescenters.FreightCode;
                    salesCentermodel.VolumePoints = salescenters.VolumePoints;
                    salesCentermodel.WorkTime = salescenters.WorkTime;

                    return salesCentermodel;
                });
            });

            return model;
        }

        public async Task<SalesCenterDtoModel> PrepareSalesCenterDtoModelAsync(SalesCenterDtoModel model, SalesCenterDto salesCenterDto, bool excludeProperties = false)
        {
            model = new SalesCenterDtoModel
            {
                Id = model.Id,
                Name = model.Name,
                Address = model.Address,
                City = model.City,
                Price = model.Price,
                WarehouseCode = model.WarehouseCode,
                FreightCode = model.FreightCode,
                VolumePoints = model.VolumePoints,
                WorkTime = model.WorkTime,

            };
            return await Task.FromResult(model);
        }

        #endregion

    }
}
