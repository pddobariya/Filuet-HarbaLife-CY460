using Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Services;
using Filuet.Onlineordering.Shipping.Delivery.Constants;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Extensions;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using LinqToDB;
using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public class DeliveryPriceService : IDeliveryPriceService
    {
        #region Fields

        private readonly IRepository<SalesCenter> _salesCentersRepository;
        private readonly IRepository<DeliveryType> _deliveryTypeRepository;
        private readonly IRepository<DeliveryTypeLanguage> _deliveryTypeLanguageRepository;
        private readonly IRepository<AutoPostOffice> _autoPostOfficeRepository;
        private readonly IRepository<AutoPostOfficeLanguage> _autoPostOfficeLanguageRepository;
        private readonly IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;
        private readonly IRepository<DeliveryCityLanguage> _deliveryCityLanguageRepository;
        private readonly IWorkContext _workContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly ISalesCenterService _salesCenterService;
        private readonly IRepository<SalesCenterLanguage> _salesCenterLanguageRepository;
        private readonly IDeliveryTypeService _deliveryTypeService;
        private readonly IDeliveryOperatorService _deliveryOperatorService;
        private readonly IDeliveryCityService _deliveryCityService;
        private readonly IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService;
        private readonly IAutoPostOfficeService _autoPostOfficeService;
        private readonly IRepository<DeliveryOperator> _deliveryOperatorRepository;
        private readonly IRepository<DeliveryOperatorLanguage> _deliveryOperatorLanguageRepository;
        private readonly IRepository<DeliveryCity> _deliveryCityRepository;
        private readonly IRepository<Price> _priceRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;
        private readonly ISalesCentersService _salesCentersService;

        #endregion

        #region Ctor
        public DeliveryPriceService(
                 IRepository<DeliveryOperatorLanguage> deliveryOperatorLanguageRepository,
                 IRepository<DeliveryTypeLanguage> deliveryTypeLanguageRepository,
                 IRepository<SalesCenter> salesCentersRepository = null,
                 IRepository<DeliveryType> deliveryTypeRepository = null,
                 IRepository<AutoPostOffice> autoPostOfficeRepository = null,
                 IRepository<AutoPostOfficeLanguage> autoPostOfficeLanguageRepository = null,
                 IRepository<DeliveryOperator_DeliveryType_DeliveryCity_Dependency> deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository = null,
                 IRepository<DeliveryCityLanguage> deliveryCityLanguageRepository = null,
                 IWorkContext workContext = null,
                 IStaticCacheManager staticCacheManager = null,
                 ISalesCenterService salesCenterService = null,
                 IRepository<SalesCenterLanguage> salesCenterLanguageRepository = null,
                 IDeliveryTypeService deliveryTypeService = null,
                 IDeliveryOperatorService deliveryOperatorService = null,
                 IDeliveryCityService deliveryCityService = null,
                 IDeliveryOperator_DeliveryType_DeliveryCity_DependenciesService deliveryOperatorDeliveryTypeDeliveryCityDependenciesService = null,
                 IAutoPostOfficeService autoPostOfficeService = null,
                 IRepository<DeliveryOperator> deliveryOperatorRepository = null,
                 IRepository<DeliveryCity> deliveryCityRepository = null,
                 IRepository<Price> priceRepository = null,
                 IShoppingCartService shoppingCartService = null,
                 IProductService productService = null,
                 ISalesCentersService salesCentersService = null)
        {
            _salesCentersRepository = salesCentersRepository;
            _deliveryTypeRepository = deliveryTypeRepository;
            _autoPostOfficeRepository = autoPostOfficeRepository;
            _autoPostOfficeLanguageRepository = autoPostOfficeLanguageRepository;
            _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository = deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository;
            _deliveryCityLanguageRepository = deliveryCityLanguageRepository;
            _workContext = workContext;
            _staticCacheManager = staticCacheManager;
            _salesCenterService = salesCenterService;
            _salesCenterLanguageRepository = salesCenterLanguageRepository;
            _deliveryTypeService = deliveryTypeService;
            _deliveryOperatorService = deliveryOperatorService;
            _deliveryCityService = deliveryCityService;
            _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService = deliveryOperatorDeliveryTypeDeliveryCityDependenciesService;
            _autoPostOfficeService = autoPostOfficeService;
            _deliveryOperatorRepository = deliveryOperatorRepository;
            _deliveryCityRepository = deliveryCityRepository;
            _priceRepository = priceRepository;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
            _deliveryTypeLanguageRepository = deliveryTypeLanguageRepository;
            _deliveryOperatorLanguageRepository = deliveryOperatorLanguageRepository;
            _salesCentersService = salesCentersService;
        }

        #endregion

        #region Methods

        #region SalesCenterRegion

        private IQueryable<AutoPostOffice> GetAutoPostOfficesTable() => _autoPostOfficeRepository.InitiateLanguages().Table.Where(x => !x.Blocked);

        public async Task<SalesCenterDto[]> GetSalesCentersAsync(int languageId)
        {
            var key = string.Format(NopFiluetCommonDefaults.DeliverySalesCentersByLanguageCacheKey, languageId);
            return await _staticCacheManager.GetAsync(new CacheKey(key),
                 async () => await _salesCentersRepository.InitiateLanguages().Table.AsEnumerable().SelectAwait(async sc =>
                {
                    var scl = (await _salesCenterService.GetSalesCenterLanguagesBySalesCenterIdAsync(sc.Id)).FirstOrDefault(scl => scl.LanguageId == languageId);
                    return new SalesCenterDto
                    {
                        Id = sc.Id,
                        FreightCode = sc.FreightCode,
                        Price = sc.Price,
                        WarehouseCode = sc.WarehouseCode,
                        VolumePoints = sc.VolumePoints,
                        WorkTime = scl.WorkTime,
                        Name = scl.Name,
                        City = scl.City,
                        Address = scl.Address
                    };
                }).ToArrayAsync()
            );
        }

        public async Task<SalesCenterDto> GetSalesCenterByIdAsync(int id, int languageId)
        {
            return (await GetSalesCentersAsync(languageId)).FirstOrDefault(o => o.Id == id);
        }

        public async Task UpdateSalesCenterAsync(SalesCenterDtoModel model, int languageId)
        {
            var salesCenter = await _salesCentersRepository.Table.FirstOrDefaultAsync(sc => sc.Id == model.Id);
            var centerLanguage = (await _salesCenterService.GetSalesCenterLanguagesBySalesCenterIdAsync(salesCenter.Id)).FirstOrDefault(scl => scl.LanguageId == languageId);
            centerLanguage.City = model.City;
            centerLanguage.WorkTime = model.WorkTime;
            centerLanguage.Address = model.Address;
            centerLanguage.Name = model.Name;
            salesCenter.WarehouseCode = model.WarehouseCode;
            salesCenter.FreightCode = model.FreightCode;
            salesCenter.Price = model.Price;
            salesCenter.VolumePoints = model.VolumePoints;
            await _salesCenterService.UpdateSalesCenterAsync(salesCenter);
            await _salesCenterService.UpdateSalesCenterLanguageAsync(centerLanguage);
            var key = string.Format(NopFiluetCommonDefaults.DeliverySalesCentersByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task DeleteSalesCenterAsync(SalesCenterDtoModel model, int languageId)
        {
            var salesCenter = await _salesCentersRepository.GetByIdAsync(model.Id);
            await _salesCentersRepository.DeleteAsync(salesCenter);
            var key = string.Format(NopFiluetCommonDefaults.DeliverySalesCentersByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task CreateSalesCenterAsync(SalesCenterDtoModel model, int languageId)
        {
            var sales = new SalesCenter()
            {
                Id = model.Id,
                WarehouseCode = model.WarehouseCode,
                FreightCode = model.FreightCode,
                VolumePoints = model.VolumePoints,
                Price = model.Price
            };
            await _salesCentersRepository.InsertAsync(sales);

            await _salesCenterLanguageRepository.InsertAsync(new SalesCenterLanguage
            {
                LanguageId = languageId,
                Name = model.Name,
                WorkTime = model.WorkTime,
                Address = model.Address,
                City = model.City,
                SalesCenterId = sales.Id
            });

            var key = string.Format(NopFiluetCommonDefaults.DeliverySalesCentersByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        #endregion

        #region DeliveryTypeRegion

        public async Task<DeliveryTypeDto[]> GetDeliveryTypesAsync(int languageId)
        {
            return await (await _deliveryTypeRepository.InitiateLanguagesAsync()).Table.SelectAwait(async dt => new DeliveryTypeDto { Id = dt.Id, IsActive = dt.IsActive, SystemType = dt.SystemType, TypeName = (await _deliveryTypeService.GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(dt.Id)).FirstOrDefault(dtl => dtl.LanguageId == languageId).TypeName }).ToArrayAsync();
            //var key = string.Format(NopFiluetCommonDefaults.DeliveryTypesByLanguageCacheKey, languageId);
            //return await (await _staticCacheManager.GetAsync(new CacheKey(key), async () => (await _deliveryTypeRepository.InitiateLanguagesAsync()).Table.SelectAwait(async dt => new DeliveryTypeDto { Id = dt.Id, IsActive = dt.IsActive, SystemType = dt.SystemType, TypeName = (await _deliveryTypeService.GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(dt.Id)).FirstOrDefault(dtl => dtl.LanguageId == languageId).TypeName }).ToArrayAsync()));
        }

        public async Task DeliveryTypeUpdateAsync(DeliveryTypeDto model, int languageId)
        {
            var deliveryType = await _deliveryTypeRepository.GetByIdAsync(model.Id);
            var deliveryTypeLanguage = (await _deliveryTypeService.GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(deliveryType.Id)).FirstOrDefault(dtl => dtl.LanguageId == languageId);
            deliveryTypeLanguage.TypeName = model.TypeName;
            deliveryType.IsActive = model.IsActive;
            await _deliveryTypeService.UpdateDeliveryTypeAsync(deliveryType);
            await _deliveryTypeService.UpdateDeliveryTypeLanguageAsync(deliveryTypeLanguage);
            var key = string.Format(NopFiluetCommonDefaults.DeliveryTypesByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        #endregion

        #region DeliveryOperatorRegion

        //public async Task<DeliveryOperator[]> GetDeliveryOperatorsAsync()
        //{
        //    return _deliveryOperatorRepository.InitiateLanguages().Table.ToArray();
        //}

        public async Task<DeliveryOperatorDto[]> GetDeliveryOperatorDtosAsync(int languageId)
        {
            var excludedWarehouses = new[] { "NONELV", "NONELT" };
            var key = string.Format(NopFiluetCommonDefaults.DeliveryOperatorsByLanguageCacheKey, languageId);
            return await _staticCacheManager.GetAsync(new CacheKey(key), async () =>
            {
                return await _deliveryOperatorRepository.InitiateLanguages().Table
                    .Where(dop =>
                        !excludedWarehouses.Contains(dop.WarehouseCode, StringComparer.InvariantCultureIgnoreCase))
                    .AsEnumerable()
                    .SelectAwait(async dop =>
                    {
                        var deliveryOperatorLanguagesByDeliveryOperatorId = (await _deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(dop.Id));
                        return new DeliveryOperatorDto
                        {
                            Id = dop.Id,
                            FreightCode = dop.FreightCode,
                            OperatorName = deliveryOperatorLanguagesByDeliveryOperatorId
                                .FirstOrDefault(dcl => dcl.LanguageId == languageId).OperatorName,
                            WarehouseCode = dop.WarehouseCode
                        };
                    }).ToArrayAsync();
            });
        }

        public async Task UpdateDeliveryOperatorAsync(DeliveryOperatorDtoModel model, int languageId)
        {
            var deliveryOperator = await _deliveryOperatorRepository.Table.FirstOrDefaultAsync(dop => dop.Id == model.Id);
            var deliveryOperatorLanguage = (await _deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(deliveryOperator.Id)).FirstOrDefault(dcl => dcl.LanguageId == languageId);
            deliveryOperator.FreightCode = model.FreightCode;
            deliveryOperatorLanguage.OperatorName = model.OperatorName;
            deliveryOperator.WarehouseCode = model.WarehouseCode;
            await _deliveryOperatorService.UpdateDeliveryOperator(deliveryOperator);
            await _deliveryOperatorService.UpdateDeliveryOperatorLanguage(deliveryOperatorLanguage);
            var key = string.Format(NopFiluetCommonDefaults.DeliveryOperatorsByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task DeleteDeliveryOperatorAsync(DeliveryOperatorDtoModel model, int languageId)
        {
            var deliveryOperator = _deliveryOperatorRepository.Table.FirstOrDefault(dop => dop.Id == model.Id);
            await _deliveryOperatorRepository.DeleteAsync(deliveryOperator);
            var key = string.Format(NopFiluetCommonDefaults.DeliveryOperatorsByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task CreateDeliveryOperatorAsync(DeliveryOperatorDtoModel model, int languageId)
        {
            var deliveryOperator = new DeliveryOperator
            {
                FreightCode = model.FreightCode,
                WarehouseCode = model.WarehouseCode
            };
            await _deliveryOperatorService.InsertDeliveryOperatorAsync(deliveryOperator);

            await _deliveryOperatorService.InsertDeliveryOperatorLanguage(new DeliveryOperatorLanguage
            {
                LanguageId = languageId,
                OperatorName = model.OperatorName,
                DeliveryOperatorId = deliveryOperator.Id
            });
            var key = string.Format(NopFiluetCommonDefaults.DeliveryOperatorsByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        #endregion

        #region DeliveryCityRegion

        public async Task<DeliveryOperatorsCityDto[]> GetDeliveryOperatorsCitiesDtoAsync(int languageId, string[] wareHouses = null, CultureInfo cultureInfo = null)
        {
            var query =(await _deliveryCityRepository.InitiateLanguages()).Table
                .Join(_deliveryCityLanguageRepository.Table, dc => dc.Id, dcl => dcl.DeliveryCityId,
                    (dc, dcl) => new { dc, dcl })
                .Join(_deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table, dcdcl => dcdcl.dc.Id,
                    dodtdcd => dodtdcd.DeliveryCityId, (dcdcl, dodtdcd) => new { dcdcl.dc, dcdcl.dcl, dodtdcd })
                .Where(x => x.dcl.LanguageId == languageId);
            if (wareHouses != null && wareHouses.Any())
            {
                query = query
                    .Join(_deliveryOperatorRepository.Table, arg => arg.dodtdcd.DeliveryOperatorId, @do => @do.Id,
                        (arg, @do) => new { arg, @do }).Where(x => wareHouses.Contains(x.@do.WarehouseCode, StringComparer.InvariantCultureIgnoreCase))
                    .Select(x => x.arg);
            }

            var localComparer = new LocalComparer(cultureInfo);
            return query.AsEnumerable()
            .GroupBy(x => new { x.dc.Id, x.dcl.CityName }, arg => arg.dodtdcd).Select(x =>
                new DeliveryOperatorsCityDto
                {
                    Id = x.Key.Id,
                    CityName = x.Key.CityName,
                    DeliveryOperator_DeliveryType_DeliveryCity_Dependencies = x.ToArray()
                }).OrderBy(x => x.CityName, localComparer).ToArray();
        }

        public async Task<DeliveryCity[]> GetCitiesAsync()
        {
            return  (await _deliveryCityRepository.InitiateLanguages()).Table.ToArray();
        }

        public async Task<CityViewModel[]> GetDeliveryCitiesAsync(ShipingMethodEnum shipingMethod, string[] wareHouses = null)
        {
            return await(await _deliveryCityRepository.InitiateLanguages()).Table.Join(_deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table,
                dc => dc.Id, dodtdcd => dodtdcd.DeliveryCityId, (dc, dodtdcd) => new 
                {
                    Id = dc.Id, 
                    dodtdcd.DeliveryTypeId, 
                    dodtdcd.DeliveryOperatorId 
                }).Join(_deliveryTypeRepository.Table, 
                dcdodtdDc => dcdodtdDc.DeliveryTypeId, dt => dt.Id, (dcdodtdDc, dt) => new 
                { 
                    Id = dcdodtdDc.Id,
                    dt.SystemType, 
                    dcdodtdDc.DeliveryOperatorId 
                })/*.Where(c => c.DeliveryOperator_DeliveryType_DeliveryCity_Dependencies.Any(dodtdcd => dodtdcd.DeliveryType.SystemType == shipingMethod.ToString()))*/.Join(_deliveryOperatorRepository.Table, dcdodtdDcDt => dcdodtdDcDt.DeliveryOperatorId, dop => dop.Id, (dcdodtdDcDt, dop) => new 
                { 
                    Id = dcdodtdDcDt.Id, 
                    dop.WarehouseCode, 
                    dcdodtdDcDt.SystemType 
                }).Join(_deliveryCityLanguageRepository.Table, dc => dc.Id, lang => lang.DeliveryCityId, (dc, lang) => new 
                { 
                    Id = dc.Id, 
                    dc.WarehouseCode, 
                    CityName = lang.CityName, 
                    langId = lang.LanguageId, 
                    dt = dc.SystemType }).WhereAwait(async dcl => dcl.langId ==(await _workContext.GetWorkingLanguageAsync()).Id && dcl.dt == shipingMethod.ToString() && (wareHouses == null || wareHouses.Contains(dcl.WarehouseCode))).Select(c => new CityViewModel { Id = c.Id, CityName = c.CityName }).OrderBy(cvms => cvms.CityName).Distinct().ToArrayAsync();
        }

        public async Task UpdateDeliveryCityAsync(DeliveryOperatorsCityModel model, int languageId)
        {
            var deliveryCity = await _deliveryCityRepository.GetByIdAsync(model.Id);
            var deliveryCityLanguage = (await _deliveryCityService.GetDeliveryCityLanguagesByDeliveryCityIdAsync(deliveryCity.Id)).FirstOrDefault(dcl => dcl.LanguageId == languageId);
            deliveryCityLanguage.CityName = model.CityName;
            await _deliveryCityService.UpdateDeliveryCityLanguage(deliveryCityLanguage);
            var key = string.Format(NopFiluetCommonDefaults.DeliveryOperatorsCitiesByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task DeleteDeliveryCityAsync(DeliveryOperatorsCityModel model, int languageId)
        {
            var deliveryCity = await _deliveryCityRepository.GetByIdAsync(model.Id);
            await _deliveryCityRepository.DeleteAsync(deliveryCity);
            var key = string.Format(NopFiluetCommonDefaults.DeliveryOperatorsCitiesByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task CreateDeliveryCityAsync(DeliveryOperatorsCityModel model)
        {
            var deliveryCity = new DeliveryCity()
            {
                Id = model.Id
            };
            await _deliveryCityRepository.InsertAsync(deliveryCity);

            await _deliveryCityLanguageRepository.InsertAsync(new DeliveryCityLanguage
            {
                LanguageId = model.LanguageId,
                CityName = model.CityName,
                DeliveryCityId = deliveryCity.Id
            });
        }

        #endregion

        #region PriceRegion

        public async Task<IEnumerable<PriceDto>> GetPricesAsync()
        {
            return await _priceRepository.Table.AsEnumerable().SelectAwait(async p =>
            {
                var deliveryOperatorDeliveryTypeDeliveryCityDependencyById =(await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                    p.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId));

                return new PriceDto
                {
                    Id = p.Id,
                    DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                    DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                    DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                    DeliveryPrise = p.DeliveryPrise,
                    MaxCriterionValue = p.MaxCriterionValue(),
                    MinCriterionValue = p.MinCriterionValue()
                };
            }).ToArrayAsync();

        }
       
        public async Task<PriceDto[]> GetOperatorPriceAsync(int dodtdcd)
        {
            var prices = _priceRepository.Table.AsEnumerable().Where(p => p.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId == dodtdcd);

            var result = new List<PriceDto>();

            foreach (var price in prices)
            {
                var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService.GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);

                result.Add(new PriceDto
                {
                    Id = price.Id,
                    DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                    DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                    DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                    DeliveryPrise = price.DeliveryPrise,
                    MaxCriterionValue = price.MaxCriterionValue(),
                    MinCriterionValue = price.MinCriterionValue()
                });
            }

            return result.ToArray();
        }


        public async Task<PriceDto> GetPriceDtoByIdAsync(int id)
        {
            var price = await _priceRepository.GetByIdAsync(id);
            var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                    price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
            return new PriceDto
            {
                Id = price.Id,
                DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                DeliveryPrise = price.DeliveryPrise,
                MaxCriterionValue = price.MaxCriterionValue(),
                MinCriterionValue = price.MinCriterionValue()
            };
        }
        public async Task<PriceDto> GetPriceDtoByIdDropdownAsync(int id, int languageId)
        {
            var price = await _priceRepository.GetByIdAsync(id);
            var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                    price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
            var model = new PriceDto
            {
                Id = price.Id,
                DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                DeliveryPrise = price.DeliveryPrise,
                MaxCriterionValue = price.MaxCriterionValue(),
                MinCriterionValue = price.MinCriterionValue()
            };
            return model;
        }

        public async Task<DeliveryObject> GetDeliveryObjectByPriceIdAsync(int priceId)
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            return await _priceRepository.Table.AsEnumerable().Where(p => p.Id == priceId).Select(async p =>
            {
                var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService.GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                        p.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
                var deliveryCityLanguagesByDeliveryCityId = await _deliveryCityService.GetDeliveryCityLanguagesByDeliveryCityIdAsync(
                    deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId);
                var deliveryOperatorById = await _deliveryOperatorService.GetDeliveryObjectByPriceIdAsync(deliveryOperatorDeliveryTypeDeliveryCityDependencyById
                    .DeliveryOperatorId);
                var deliveryOperatorLanguagesByDeliveryOperatorId = await _deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(deliveryOperatorById.Id);
                return new DeliveryObject
                {
                    DeliveryPrise = p.DeliveryPrise,
                    DeliveryCity = deliveryCityLanguagesByDeliveryCityId.FirstOrDefault(lang => lang.LanguageId == language.Id).CityName,
                    WarehouseCode = deliveryOperatorById.WarehouseCode,
                    FreightCode = deliveryOperatorById.FreightCode,
                    OperatorName = deliveryOperatorLanguagesByDeliveryOperatorId.FirstOrDefault(dol => dol.LanguageId == language.Id)
                        .OperatorName
                };
            }).FirstOrDefault();
        }

        public async Task UpdatePriceAsync(PriceDtoAddModel model)
        {
            var price = await _priceRepository.GetByIdAsync(model.Id);
            var oldPriceDeliveryOperatorDeliveryTypeDeliveryCityDependency = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService.GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
            var deliveryOperatorDeliveryTypeDeliveryCityDependency = await _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table.FirstOrDefaultAsync(dodtdc =>
                dodtdc.DeliveryOperatorId == model.DeliveryOperatorId && dodtdc.DeliveryTypeId == model.DeliveryTypeId && dodtdc.DeliveryCityId == model.DeliveryCityId
            ) ?? new DeliveryOperator_DeliveryType_DeliveryCity_Dependency
            {
                DeliveryOperatorId = model.DeliveryOperatorId,
                DeliveryTypeId = model.DeliveryTypeId,
                DeliveryCityId = model.DeliveryCityId
            };
            price.DeliveryPrise = model.DeliveryPrise;
            price.CriterionValues = $"{model.MinCriterionValue};{model.MaxCriterionValue}";
            price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId = deliveryOperatorDeliveryTypeDeliveryCityDependency.Id;
            await _priceRepository.UpdateAsync(price);
            await _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.UpdateAsync(
                deliveryOperatorDeliveryTypeDeliveryCityDependency);
            await DeleteDependencyIfFreeAsync(oldPriceDeliveryOperatorDeliveryTypeDeliveryCityDependency);
        }

        public async Task DeletePriceAsync(PriceDtoAddModel model)
        {
            var price = await _priceRepository.GetByIdAsync(model.Id);
            await _priceRepository.DeleteAsync(price);
            await DeleteDependencyIfFreeAsync(await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService.GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId));
        }

        public async Task CreatePriceAsync(PriceDtoAddModel model)
        {
            var deliveryOperatorDeliveryTypeDeliveryCityDependency = await _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table.FirstOrDefaultAsync
                (dodtdc => dodtdc.DeliveryOperatorId == model.DeliveryOperatorId
                && dodtdc.DeliveryTypeId == model.DeliveryTypeId
                && dodtdc.DeliveryCityId == model.DeliveryCityId
                );
            if (deliveryOperatorDeliveryTypeDeliveryCityDependency == null)
            {
                deliveryOperatorDeliveryTypeDeliveryCityDependency = new DeliveryOperator_DeliveryType_DeliveryCity_Dependency
                {
                    DeliveryOperatorId = model.DeliveryOperatorId,
                    DeliveryTypeId = model.DeliveryTypeId,
                    DeliveryCityId = model.DeliveryCityId
                };
                await _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.InsertAsync(
                    deliveryOperatorDeliveryTypeDeliveryCityDependency);
            }

            var price = new Price
            {
                DeliveryOperator_DeliveryType_DeliveryCity_DependencyId = deliveryOperatorDeliveryTypeDeliveryCityDependency.Id,
                DeliveryPrise = model.DeliveryPrise,
                CriterionValues = $"{model.MinCriterionValue};{model.MaxCriterionValue}"
            };
            await _priceRepository.InsertAsync(price);
        }

        #endregion

        #region AutoPostOfficeRegion

        public async Task<IEnumerable<AutoPostOfficeDto>> GetAutoPostOfficesDtoAsync(int languageId)
        {
            try
            {
                int dataid = 0;
                return await GetAutoPostOfficesTable().AsEnumerable().SelectAwait(async apo =>
                {
                    var deliveryOperatorDeliveryTypeDeliveryCityDependencyById =(await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                            .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                                apo.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId));
                    var autoPostOfficeLanguagesById =(await _autoPostOfficeService.GetAutoPostOfficeLanguagesByIdAsync(apo.Id));
                    dataid = apo.Id;

                    return new AutoPostOfficeDto
                    {
                        Id = apo.Id,
                        DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                        DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                        DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                        Blocked = apo.Blocked,
                        Address = autoPostOfficeLanguagesById.FirstOrDefault(apol => apol.LanguageId == languageId)?.Address,
                        PointId = apo.PointId,
                        Comment = autoPostOfficeLanguagesById.FirstOrDefault(apol => apol.LanguageId == languageId)?.Comment,
                        PriceIsAbsent = !GetPriceByDeliveryOperator_DeliveryType_DeliveryCity_DependencyId(apo.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId).Any()
                    };
                }).ToArrayAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<IEnumerable<AutoPostOffice>> GetAutoPostOfficesAsync()
        //{
        //    return GetAutoPostOfficesTable().ToArray();
        //}

        public async Task<AutoPostOfficeDto> GetAutoPostOfficesDtoById(int Id)
        {
            var apo = await _salesCentersService.GetAutoPostOfficeByIdAsync(Id);

            var deliveryOperatorDeliveryTypeDeliveryCityDependencyById =(await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                   .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                       apo.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId));
            var model = new AutoPostOfficeDto
            {
                Id = apo.Id,
                Blocked = apo.Blocked,
                DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                PointId = apo.PointId,
                PriceIsAbsent = !GetPriceByDeliveryOperator_DeliveryType_DeliveryCity_DependencyId(apo.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId).Any()
            };
            return model;
        }

        public async Task<AutoPostOfficeDto[]> GetAutoPostOfficeDtosAsync(int languageId, CultureInfo cultureInfo = null)
        {
            var key = string.Format(NopFiluetCommonDefaults.AutoPostOfficesByLanguageCacheKey, languageId);
            return await _staticCacheManager.GetAsync(new CacheKey(key), () =>
            {
                var v = GetAutoPostOfficesTable()
                    .Join(_autoPostOfficeLanguageRepository.Table, apo => apo.Id, apol => apol.AutoPostOfficeId,
                        (apo, apol) => new { apo, apol })
                    .Join(_deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table,
                        arg => arg.apo.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId,
                        dodtdcd => dodtdcd.Id, (arg, dodtdcd) => new { arg.apo, arg.apol, dodtdcd })
                    .GroupJoin(_priceRepository.Table, arg => arg.dodtdcd.Id,
                        price => price.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId,
                        (arg, p) => new { arg.apo, arg.apol, arg.dodtdcd, p })
                    .Where(x => x.apol.LanguageId == languageId);

                var localComparer = new LocalComparer(cultureInfo);
                return v.Select(x => new AutoPostOfficeDto
                {
                    Id = x.apo.Id,
                    Blocked = x.apo.Blocked,
                    DeliveryCityId = x.dodtdcd.DeliveryCityId,
                    DeliveryOperatorId = x.dodtdcd.DeliveryOperatorId,
                    DeliveryTypeId = x.dodtdcd.DeliveryTypeId,
                    PointId = x.apo.PointId,
                    Address = x.apol.Address,
                    Comment = x.apol.Comment,
                    PriceIsAbsent = x.p.Any(),
                    DeliveryOperator_DeliveryType_DeliveryCity_DependencyId =
                            x.dodtdcd.Id
                }).OrderBy(x => x.Address, localComparer).ToArray();
            });
        }

        public async Task<AutoPostOfficeDto> GetAutoPostOfficeDtoByIdAsync(int id, int languageId)
        {
            var autoPostOffice = GetAutoPostOfficesTable().FirstOrDefault(apo => apo.Id == id);
            if (autoPostOffice == null)
            {
                return null;
            }
            var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(
                    autoPostOffice.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
            var autoPostOfficeLanguagesById = await _autoPostOfficeService.GetAutoPostOfficeLanguagesByIdAsync(autoPostOffice.Id);
            return new AutoPostOfficeDto
            {
                Id = autoPostOffice.Id,
                Blocked = autoPostOffice.Blocked,
                DeliveryCityId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId,
                DeliveryOperatorId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId,
                DeliveryTypeId = deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryTypeId,
                PointId = autoPostOffice.PointId,
                Address = autoPostOfficeLanguagesById.FirstOrDefault(apol => apol.LanguageId == languageId).Address,
                Comment = autoPostOfficeLanguagesById.FirstOrDefault(apol => apol.LanguageId == languageId).Comment,
                PriceIsAbsent = !GetPriceByDeliveryOperator_DeliveryType_DeliveryCity_DependencyId(autoPostOffice
                    .DeliveryOperator_DeliveryType_DeliveryCity_DependencyId).Any()
            };
        }

        public async Task<DeliveryObject> GetDeliveryObjectByAutoPostOfficeIdAsync(int autoPostOfficeId, int priceId)
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            return await _priceRepository.Table.AsEnumerable().Where(p => p.Id == priceId).SelectAwait(async p =>
            {
                var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                    .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(p.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
                var deliveryCityLanguagesByDeliveryCityId = await _deliveryCityService.GetDeliveryCityLanguagesByDeliveryCityIdAsync(
                    deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryCityId);
                var deliveryOperatorById = await _deliveryOperatorService.GetDeliveryObjectByPriceIdAsync(deliveryOperatorDeliveryTypeDeliveryCityDependencyById.DeliveryOperatorId);
                var deliveryOperatorLanguagesByDeliveryOperatorId = await _deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(deliveryOperatorById.Id);
                var autoPostOfficeById = await _autoPostOfficeService.GetAutoPostOfficeByIdAsync(autoPostOfficeId);
                var autoPostOfficeLanguagesById = await _autoPostOfficeService.GetAutoPostOfficeLanguagesByIdAsync(autoPostOfficeId);
                return new DeliveryObject
                {
                    DeliveryPrise = p.DeliveryPrise,
                    DeliveryCity = deliveryCityLanguagesByDeliveryCityId.FirstOrDefault(lang => lang.LanguageId == language.Id).CityName,
                    WarehouseCode = deliveryOperatorById.WarehouseCode,
                    FreightCode = deliveryOperatorById.FreightCode,
                    OperatorName = deliveryOperatorLanguagesByDeliveryOperatorId.FirstOrDefault(dol => dol.LanguageId == language.Id).OperatorName,
                    PointId = autoPostOfficeById.PointId,
                    Address = autoPostOfficeLanguagesById.FirstOrDefault(lang => lang.LanguageId == language.Id).Address,
                    Comment = autoPostOfficeLanguagesById.ToDictionary(x => x.LanguageId, x => x.Comment)
                };
            }).FirstOrDefaultAsync();
        }

        public async Task UpdateAutoPostOfficeAsync(AutoPostOfficeDtoModel model, int languageId)
        {
            var autoPostOffice = await _autoPostOfficeRepository.GetByIdAsync(model.Id);
            var autoPostOfficeLanguagesById = await _autoPostOfficeService.GetAutoPostOfficeLanguagesByIdAsync(autoPostOffice.Id);
            var oldDeliveryOperatorDeliveryTypeDeliveryCityDependency =(await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService.GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(autoPostOffice.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId));
            var deliveryOperatorDeliveryTypeDeliveryCityDependency = _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table.FirstOrDefault(dodtdc =>
                dodtdc.DeliveryOperatorId == model.DeliveryOperatorId && dodtdc.DeliveryTypeId == model.DeliveryTypeId && dodtdc.DeliveryCityId == model.DeliveryCityId
            ) ?? new DeliveryOperator_DeliveryType_DeliveryCity_Dependency
            {
                DeliveryOperatorId = model.DeliveryOperatorId,
                DeliveryTypeId = model.DeliveryTypeId,
                DeliveryCityId = model.DeliveryCityId
            };
            var autoPostOfficeLanguage = autoPostOfficeLanguagesById.First(apol => apol.LanguageId == languageId);
            autoPostOffice.Blocked = model.Blocked;
            autoPostOffice.PointId = model.PointId;
            autoPostOffice.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId =
                deliveryOperatorDeliveryTypeDeliveryCityDependency.Id;
            autoPostOfficeLanguage.Address = model.Address;
            autoPostOfficeLanguage.Comment = model.Comment;
            await _autoPostOfficeRepository.UpdateAsync(autoPostOffice);
            await _autoPostOfficeLanguageRepository.UpdateAsync(autoPostOfficeLanguage);
            await DeleteDependencyIfFreeAsync(oldDeliveryOperatorDeliveryTypeDeliveryCityDependency);
            var key = string.Format(NopFiluetCommonDefaults.AutoPostOfficesByLanguageCacheKey, languageId);
            await _staticCacheManager.RemoveAsync(new CacheKey(key));
        }

        public async Task DeleteAutoPostOfficeAsync(AutoPostOfficeDtoModel model, int languageId)
        {
            var autoPostOffice = await _autoPostOfficeRepository.GetByIdAsync(model.Id);
            var deliveryOperatorDeliveryTypeDeliveryCityDependencyById = await _deliveryOperatorDeliveryTypeDeliveryCityDependenciesService
                .GetDeliveryOperator_DeliveryType_DeliveryCity_DependencyByIdAsync(autoPostOffice
                    .DeliveryOperator_DeliveryType_DeliveryCity_DependencyId);
            await _autoPostOfficeRepository.DeleteAsync(autoPostOffice);
            await DeleteDependencyIfFreeAsync(deliveryOperatorDeliveryTypeDeliveryCityDependencyById);
        }

        public async Task<DeliveryCityLanguage> GetDeliveryCityNameByIdAsync(int deliveryCityId)
        {
            var query = from DeliveryCity in _deliveryCityLanguageRepository.Table
                        join DeliveryOperator in _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table on DeliveryCity.DeliveryCityId equals DeliveryOperator.DeliveryCityId
                        where DeliveryCity.DeliveryCityId == deliveryCityId
                        select DeliveryCity;
            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<DeliveryTypeLanguage> GetDeliveryTypeNameByIdAsync(int deliveryTypeId)
        {
            var query = from DeliveryType in _deliveryTypeLanguageRepository.Table
                        join DeliveryOperator in _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table on DeliveryType.DeliveryTypeId equals DeliveryOperator.DeliveryTypeId
                        where DeliveryType.DeliveryTypeId == deliveryTypeId
                        select DeliveryType;
            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<DeliveryType> GetDeliverySystemTypeNameByIdAsync(int deliveryTypeId)
        {
            var query = from DeliveryType in _deliveryTypeRepository.Table
                        join SystemType in _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table
                        on DeliveryType.Id equals SystemType.DeliveryTypeId
                        where DeliveryType.Id == deliveryTypeId
                        select DeliveryType;
            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<DeliveryOperatorLanguage> GetDeliveryOperatorNameByIdAsync(int deliveryOperatorTypeId)
        {
            var query = from DeliveryOperatorName in _deliveryOperatorLanguageRepository.Table
                        join DeliveryOperator in _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.Table on DeliveryOperatorName.DeliveryOperatorId equals DeliveryOperator.DeliveryOperatorId
                        where DeliveryOperatorName.DeliveryOperatorId == deliveryOperatorTypeId
                        select DeliveryOperatorName;
            return await Task.FromResult(query.FirstOrDefault());
        }
        #endregion

        #region Methods
        public virtual async Task<bool> IsUzfFreightCode()
        {
            var countryCode = await (await _workContext.GetCurrentCustomerAsync())?.GetShippingCountryCodeAsync();

            var shoppingCartItems = await _shoppingCartService.GetShoppingCartAsync(await _workContext.GetCurrentCustomerAsync());
            var product = await _productService.GetProductByIdAsync(shoppingCartItems.FirstOrDefault().ProductId);

            if (countryCode == "UZ"
                && shoppingCartItems.Count() == 1
                && (product.Sku.ToLower() == "219n" || product.Sku.ToLower() == "220n"))
            {
                return true;
            }

            return false;
        }

        private async Task DeleteDependencyIfFreeAsync(DeliveryOperator_DeliveryType_DeliveryCity_Dependency oldDeliveryOperatorDeliveryTypeDeliveryCityDependency)
        {

            if (!(GetPriceByDeliveryOperator_DeliveryType_DeliveryCity_DependencyId(oldDeliveryOperatorDeliveryTypeDeliveryCityDependency.Id)).Any() &&
                !(await _autoPostOfficeService.GetAutoPostOfficesByDeliveryOperator_DeliveryType_DeliveryCity_DependencyIdAsync(oldDeliveryOperatorDeliveryTypeDeliveryCityDependency.Id)).Any())
            {
                await _deliveryOperatorDeliveryTypeDeliveryCityDependencyRepository.DeleteAsync(oldDeliveryOperatorDeliveryTypeDeliveryCityDependency);
            }
        }

        private Price[] GetPriceByDeliveryOperator_DeliveryType_DeliveryCity_DependencyId(int id)
        {
            return _priceRepository.Table.Where(p =>
                p.DeliveryOperator_DeliveryType_DeliveryCity_DependencyId == id).ToArray();
        }
        #endregion

        #endregion

    }

    #region LocalComparer Method

    class LocalComparer : IComparer<string>
    {
        private readonly CultureInfo _cultureInfo;

        public LocalComparer(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo ?? CultureInfo.InvariantCulture;
        }
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, _cultureInfo, CompareOptions.None);
        }
    }

    #endregion
}
