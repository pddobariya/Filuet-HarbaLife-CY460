using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Services;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using Filuet.Onlineordering.Shipping.Delivery.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Factories
{

    public class DeliveryTypeModelFactory : IDeliveryTypeModelFactory
    {
        #region Fields

        private readonly IDeliveryPriceService _deliveryPriceService;
        private readonly ISalesCentersService _salesCenterService;

        #endregion

        #region Ctor

        public DeliveryTypeModelFactory(
            IDeliveryPriceService deliveryPriceService,
            ISalesCentersService salesCenterService)
        {
            _deliveryPriceService = deliveryPriceService;
            _salesCenterService = salesCenterService;
        }

        #endregion

        #region Methods

        public virtual async Task<DeliveryTypeDtoListModel> PrepareDeliveryTypesListModelAsync(DeliveryTypeDtoSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get deliveryTypes
            var deliveryTypes = (await _deliveryPriceService.GetDeliveryTypesAsync(searchModel.LanguageId)).ToPagedList(searchModel);

            //prepare list model
            var model =  new DeliveryTypeDtoListModel().PrepareToGrid(searchModel, deliveryTypes, () =>
            {
                //fill in model values from the entity
                return deliveryTypes.Select( deliveryType =>
                {
                    var DeliveryTypesModel = new DeliveryTypeDtoModel();
                    DeliveryTypesModel.Id = deliveryType.Id;
                    DeliveryTypesModel.SystemType = deliveryType.SystemType;
                    DeliveryTypesModel.TypeName = deliveryType.TypeName;
                    DeliveryTypesModel.IsActive = deliveryType.IsActive;

                    return DeliveryTypesModel;
                });
            });

            return model;
        }
        public virtual async Task<DeliveryOperatorDtoListModel> PrepareDeliveryOperatorListModelAsync(DeliveryOperatorDtoSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get deliveryTypes
            var deliveryOperators = (await _deliveryPriceService.GetDeliveryOperatorDtosAsync(searchModel.LanguageId)).ToPagedList(searchModel);

            //prepare list model
            var model =  new DeliveryOperatorDtoListModel().PrepareToGrid(searchModel, deliveryOperators, () =>
            {
                //fill in model values from the entity
                return deliveryOperators.Select(deliveryOperator =>
                {
                    var DeliveryOperatorsModel = new DeliveryOperatorDtoModel();
                    DeliveryOperatorsModel.Id = deliveryOperator.Id;
                    DeliveryOperatorsModel.OperatorName = deliveryOperator.OperatorName;
                    DeliveryOperatorsModel.FreightCode = deliveryOperator.FreightCode;
                    DeliveryOperatorsModel.WarehouseCode = deliveryOperator.WarehouseCode;
                    return DeliveryOperatorsModel;
                });
            });

            return model;

        }

        public virtual async Task<DeliveryOperatorsCityListModel> PrepareDeliveryCityRegionListModelAsync(DeliveryOperatorsCitySearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get deliveryCities
            var deliveryCities = (await _deliveryPriceService.GetDeliveryOperatorsCitiesDtoAsync(searchModel.LanguageId)).ToPagedList(searchModel);

            //prepare list model
            var model =  new DeliveryOperatorsCityListModel().PrepareToGrid(searchModel, deliveryCities, () =>
            {
                //fill in model values from the entity
                return deliveryCities.Select( deliveryCity =>
                {
                    var DeliveryCityModel = new DeliveryOperatorsCityModel();
                    DeliveryCityModel.Id = deliveryCity.Id;
                    DeliveryCityModel.CityName = deliveryCity.CityName;

                    return DeliveryCityModel;
                });
            });

            return model;
        }

        public virtual async Task<PriceDtoListModel> PreparePriceListModelAsync(PriceDtoSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get prices
            var prices = (await _deliveryPriceService.GetPricesAsync()).ToList().ToPagedList(searchModel);

            //prepare list model
            var model = await new PriceDtoListModel().PrepareToGridAsync(searchModel, prices, () =>
            {
                //fill in model values from the entity
                return prices.SelectAwait(async price =>
                {
                    var PriceDtoModel = new PriceDtoAddModel();
                    PriceDtoModel.Id = price.Id;
                    PriceDtoModel.DeliveryCityName = (await _deliveryPriceService.GetDeliveryCityNameByIdAsync(price.DeliveryCityId)).CityName;
                    PriceDtoModel.DeliveryOperatorName = (await _deliveryPriceService.GetDeliveryOperatorNameByIdAsync(price.DeliveryOperatorId)).OperatorName;
                    PriceDtoModel.TypeName = (await _deliveryPriceService.GetDeliverySystemTypeNameByIdAsync(price.DeliveryTypeId)).SystemType;
                    PriceDtoModel.DeliveryPrise = price.DeliveryPrise;
                    PriceDtoModel.MaxCriterionValue = price.MaxCriterionValue;
                    PriceDtoModel.MinCriterionValue = price.MinCriterionValue;

                    return PriceDtoModel;
                });
            });

            return model;
        }
        public async Task<PriceDtoAddModel> PreparePriceEditDtoModelAsync(PriceDto priceDto, int languageId, bool excludeProperties = false)
        {
            var model = new PriceDtoAddModel();
            {
                model.Id = priceDto.Id;
                model.DeliveryCityName = (await _deliveryPriceService.GetDeliveryCityNameByIdAsync(priceDto.DeliveryCityId))?.CityName;
                model.DeliveryOperatorName = (await _deliveryPriceService.GetDeliveryOperatorNameByIdAsync(priceDto.DeliveryOperatorId))?.OperatorName;
                model.TypeName = (await _deliveryPriceService.GetDeliverySystemTypeNameByIdAsync(priceDto.DeliveryTypeId))?.SystemType;
                model.DeliveryCityId = priceDto.DeliveryCityId;
                model.DeliveryOperatorId = priceDto.DeliveryOperatorId;
                model.DeliveryTypeId = priceDto.DeliveryTypeId;
                model.DeliveryPrise = priceDto.DeliveryPrise;
                model.MinCriterionValue = priceDto.MinCriterionValue;
                model.MaxCriterionValue = priceDto.MaxCriterionValue;
            };
            var cityname = await _salesCenterService.GetAllCityAsync();
            foreach (var store in cityname)
            {
                if (store.LanguageId == languageId)
                {
                    model.AvailableDeliveryCityId.Add(new SelectListItem() { Text = store.CityName, Value = store.DeliveryCityId.ToString(), Selected = (model.DeliveryCityId > 0 && store.DeliveryCityId == model.DeliveryCityId) });
                }
            }
            var Operatortype = await _salesCenterService.GetAllOperatorAsync();
            foreach (var operators in Operatortype)
            {
                if (operators.LanguageId == languageId)
                {
                    model.AvailableOperatorId.Add(new SelectListItem() { Text = operators.OperatorName, Value = operators.DeliveryOperatorId.ToString(), Selected = (model.DeliveryOperatorId > 0 && operators.DeliveryOperatorId == model.DeliveryOperatorId) });
                }
            }
            var deliveryTypeLanguage = await _salesCenterService.GetSystemTypesByLanguageIdAsync(languageId);
            foreach (var types in deliveryTypeLanguage)
            {
                var deliveryTypedId = await _salesCenterService.GetDeliveryTypeId(types);
                if (deliveryTypedId != null)
                {
                    model.AvailableTypeId.Add(new SelectListItem()
                    {
                        Text = deliveryTypedId.SystemType,
                        Value = deliveryTypedId.Id.ToString()
                    });
                }
            }
            return model;
        }

        public virtual async Task<AutoPostOfficeDtoListModel> PrepareAutoPostOfficeListModelAsync(AutoPostOfficeDtoSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get deliveryTypes
            var autoPostOffices = (await _deliveryPriceService.GetAutoPostOfficesDtoAsync(searchModel.LanguageId)).ToList().ToPagedList(searchModel);


            //prepare list model
            var model = await new AutoPostOfficeDtoListModel().PrepareToGridAsync(searchModel, autoPostOffices, () =>
            {
                //fill in model values from the entity
                return autoPostOffices.SelectAwait(async autopostoffice =>
                {
                    var AutoPostOfficesModel = new AutoPostOfficeDtoModel();
                    AutoPostOfficesModel.Id = autopostoffice.Id;
                    AutoPostOfficesModel.DeliveryCityName = (await _deliveryPriceService.GetDeliveryCityNameByIdAsync(autopostoffice.DeliveryCityId)).CityName;
                    AutoPostOfficesModel.DeliveryOperatorName = (await _deliveryPriceService.GetDeliveryOperatorNameByIdAsync(autopostoffice.DeliveryOperatorId)).OperatorName;
                    AutoPostOfficesModel.TypeName = (await _deliveryPriceService.GetDeliverySystemTypeNameByIdAsync(autopostoffice.DeliveryTypeId)).SystemType;
                    AutoPostOfficesModel.PointId = autopostoffice.PointId;
                    AutoPostOfficesModel.Blocked = autopostoffice.Blocked;
                    AutoPostOfficesModel.Address = autopostoffice.Address;
                    AutoPostOfficesModel.Comment = autopostoffice.Comment;
                    AutoPostOfficesModel.PriceIsAbsent = autopostoffice.PriceIsAbsent;

                    return AutoPostOfficesModel;
                });
            });

            return model;
        }
        public async Task<PriceDtoAddModel> PreparePriceDtoModelAsync(PriceDtoAddModel model, int languageId, bool excludeProperties = false)
        {
            model = new PriceDtoAddModel
            {
                Id = model.Id,
                DeliveryCityId = model.DeliveryCityId,
                DeliveryOperatorId = model.DeliveryOperatorId,
                DeliveryTypeId = model.DeliveryTypeId,
                DeliveryPrise = model.DeliveryPrise,
                MinCriterionValue = model.MinCriterionValue,
                MaxCriterionValue = model.MaxCriterionValue,
            };
            var cityname = await _salesCenterService.GetAllCityAsync();
            foreach (var store in cityname)
            {
                if (store.LanguageId == languageId)
                {
                    model.AvailableDeliveryCityId.Add(new SelectListItem() { Text = store.CityName, Value = store.DeliveryCityId.ToString() });
                }
            }
            var Operatortype = await _salesCenterService.GetAllOperatorAsync();
            foreach (var operators in Operatortype)
            {
                if (operators.LanguageId == languageId)
                {
                    model.AvailableOperatorId.Add(new SelectListItem() { Text = operators.OperatorName, Value = operators.DeliveryOperatorId.ToString() });
                }
            }
            var deliveryTypeLanguage = await _salesCenterService.GetSystemTypesByLanguageIdAsync(languageId);
            foreach (var types in deliveryTypeLanguage)
            {
                var deliveryTypedId = await _salesCenterService.GetDeliveryTypeId(types);
                if (deliveryTypedId != null)
                {
                    model.AvailableTypeId.Add(new SelectListItem()
                    {
                        Text = deliveryTypedId.SystemType,
                        Value = deliveryTypedId.Id.ToString()
                    });
                }
            }
            return model;
        }
        public async Task<AutoPostOfficeDtoModel> PrepareAutoPostOfficeDtoModelAsync(AutoPostOfficeDtoModel model, int languageId)
        {
            model = new AutoPostOfficeDtoModel
            {
                Id = model.Id,
                DeliveryCityId = model.DeliveryCityId,
                DeliveryOperatorId = model.DeliveryOperatorId,
                DeliveryTypeId = model.DeliveryTypeId,
                PointId = model.PointId,
                Blocked = model.Blocked,
                Address = model.Address,
                Comment = model.Comment,
                PriceIsAbsent = model.PriceIsAbsent,

            };
            var cityname = await _salesCenterService.GetAllCityAsync();
            foreach (var store in cityname)
            {
                if (store.LanguageId == languageId)
                {
                    model.AvailableDeliveryCityId.Add(new SelectListItem() { Text = store.CityName, Value = store.DeliveryCityId.ToString() });
                }
            }
            var Operatortype = await _salesCenterService.GetAllOperatorAsync();
            foreach (var operators in Operatortype)
            {
                if (operators.LanguageId == languageId)
                {
                    model.AvailableOperatorId.Add(new SelectListItem() { Text = operators.OperatorName, Value = operators.DeliveryOperatorId.ToString() });
                }
            }
            var deliveryTypeLanguage = await _salesCenterService.GetSystemTypesByLanguageIdAsync(languageId);
            foreach (var types in deliveryTypeLanguage)
            {
                var deliveryTypedId = await _salesCenterService.GetDeliveryTypeId(types);
                if (deliveryTypedId != null)
                {
                    model.AvailableTypeId.Add(new SelectListItem()
                    {
                        Text = deliveryTypedId.SystemType,
                        Value = deliveryTypedId.Id.ToString()
                    });
                }
            }
            return model;
        }

        public async Task<AutoPostOfficeDtoModel> PrepareAutoPostOfficeEditDtoModelAsync(AutoPostOfficeDto autopostOfficeDto, int languageId, bool excludeProperties = false)
        {
            var model = new AutoPostOfficeDtoModel();
            {
                model.Id = autopostOfficeDto.Id;
                model.DeliveryCityName = (await _deliveryPriceService.GetDeliveryCityNameByIdAsync(autopostOfficeDto.DeliveryCityId))?.CityName;
                model.DeliveryOperatorName = (await _deliveryPriceService.GetDeliveryOperatorNameByIdAsync(autopostOfficeDto.DeliveryOperatorId))?.OperatorName;
                model.TypeName = (await _deliveryPriceService.GetDeliveryTypeNameByIdAsync(autopostOfficeDto.DeliveryTypeId))?.TypeName;
                model.DeliveryCityId = autopostOfficeDto.DeliveryCityId;
                model.DeliveryOperatorId = autopostOfficeDto.DeliveryOperatorId;
                model.DeliveryTypeId = autopostOfficeDto.DeliveryTypeId;
                model.PointId = autopostOfficeDto.PointId;
                model.Blocked = autopostOfficeDto.Blocked;
                model.Address = autopostOfficeDto.Address;
                model.Comment = autopostOfficeDto.Comment;
            };
            var cityname = await _salesCenterService.GetAllCityAsync();
            foreach (var store in cityname)
            {
                if(store.LanguageId == languageId)
                {
                    model.AvailableDeliveryCityId.Add(new SelectListItem() { Text = store.CityName, Value = store.DeliveryCityId.ToString(), Selected = (model.DeliveryCityId > 0 && store.DeliveryCityId == model.DeliveryCityId) });
                }
            }
            var Operatortype = await _salesCenterService.GetAllOperatorAsync();
            foreach (var operators in Operatortype)
            {
                if (operators.LanguageId == languageId)
                {
                    model.AvailableOperatorId.Add(new SelectListItem() { Text = operators.OperatorName, Value = operators.DeliveryOperatorId.ToString(), Selected = (model.DeliveryOperatorId > 0 && operators.DeliveryOperatorId == model.DeliveryOperatorId) });
                }
            }
            var deliveryType = await _salesCenterService.GetSystemTypesByLanguageIdAsync(languageId);
            foreach (var types in deliveryType)
            {
                var deliveryTypedId = await _salesCenterService.GetDeliveryTypeId(types);
                if (deliveryTypedId != null)
                {
                    model.AvailableTypeId.Add(new SelectListItem()
                    {
                        Text = deliveryTypedId.SystemType,
                        Value = deliveryTypedId.Id.ToString()
                    });
                }
            }
            return model;
        }

        public async Task<DeliveryOperatorsCityModel> PrepareDeliveryOperatorsCityModelAsync(DeliveryOperatorsCityModel model, DeliveryOperatorsCityDto deliveryOperatorsCityDto, bool excludeProperties = false)
        {
            model = new DeliveryOperatorsCityModel
            {
                Id = model.Id,
                CityName = model.CityName
            };
            return await Task.FromResult(model);
        }

        public async Task<DeliveryOperatorDtoModel> PrepareDeliveryOperatorDtoModelAsync(DeliveryOperatorDtoModel model, DeliveryOperatorDto deliveryOperatorDto, bool excludeProperties = false)
        {
            model = new DeliveryOperatorDtoModel
            {
                Id = model.Id,
                FreightCode = model.FreightCode,
                WarehouseCode = model.WarehouseCode,
                OperatorName = model.OperatorName
            };
            return await Task.FromResult(model);
        }

        #endregion
    }
}
