using System.Linq;
using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Localization;
using System.Threading.Tasks;
using Filuet.Onlineordering.Shipping.Delivery.Services;

namespace Filuet.Onlineordering.Shipping.Delivery.Extensions
{
    public static class DeliveryCostRepositoryExtensions
    {
        #region Properties

        private static object syncDcl = new object();

        private static object syncDol = new object();

        private static object syncScl = new object();

        private static object syncDtl = new object();

        private static object syncApol = new object();

        #endregion

        #region Methods

        public async static Task<IRepository<DeliveryCity>> InitiateLanguages(this IRepository<DeliveryCity> dcRepository)
        {
            lock (syncDcl)
            {
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var deliveryCityService = EngineContext.Current.Resolve<IDeliveryCityService>();
                var allLanguages = languageService.GetAllLanguagesAsync().Result;
                var dclRepository = EngineContext.Current.Resolve<IRepository<DeliveryCityLanguage>>();
                var deliveryCityLanguagesGroups = dcRepository.Table.Join(dclRepository.Table, dc => dc.Id, dcl => dcl.DeliveryCityId, (dc, dcl) => dcl).AsEnumerable().GroupBy(dcl => dcl.DeliveryCityId).Where(x => x.Count() < allLanguages.Count).ToArray();

                foreach (var deliveryCityLanguages in deliveryCityLanguagesGroups)
                {
                    var languageIds = allLanguages.Select(lng => lng.Id)
                        .Except(deliveryCityLanguages.Select(dc => dc.LanguageId));
                    deliveryCityService.InsertDeliveryCityLanguageAsync(languageIds.Select(lId => new DeliveryCityLanguage
                    { DeliveryCityId = deliveryCityLanguages.Key, CityName = "<undefined>", LanguageId = lId }).ToArray());
                }
                return dcRepository;
            }
        }

        public static IRepository<SalesCenter> InitiateLanguages(this IRepository<SalesCenter> scRepository)
        {
            lock (syncScl)
            {
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var salesCenterService = EngineContext.Current.Resolve<ISalesCenterService>();
                var allLanguages = languageService.GetAllLanguagesAsync().Result;
                foreach (var salesCenter in scRepository.Table)
                {
                    if (salesCenterService.GetSalesCenterLanguagesBySalesCenterIdAsync(salesCenter.Id).Result.Select(scl => scl.LanguageId).Distinct().Count() < allLanguages.Count)
                    {
                        var languageIds = allLanguages.Select(lng => lng.Id)
                            .Except(salesCenterService.GetSalesCenterLanguagesBySalesCenterIdAsync(salesCenter.Id).Result.Select(lng => lng.LanguageId));
                        salesCenterService.InsertSalesCenterLanguageAsync(languageIds.Select(lId => new SalesCenterLanguage
                        { WorkTime = "<undefined>", City = "<undefined>", Name = "<undefined>", Address = "<undefined>", SalesCenterId = salesCenter.Id, LanguageId = lId }).ToArray());
                    }
                }
                return scRepository;
            }
        }

        public static IRepository<AutoPostOffice> InitiateLanguages(this IRepository<AutoPostOffice> apoRepository)
        {
            lock (syncApol)
            {
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var autoPostOfficeService = EngineContext.Current.Resolve<IAutoPostOfficeService>();
                var allLanguages = languageService.GetAllLanguagesAsync().Result;
                var apolRepository = EngineContext.Current.Resolve<IRepository<AutoPostOfficeLanguage>>();
                var apolsGroups = apoRepository.Table.Join(apolRepository.Table, apo => apo.Id, apol => apol.AutoPostOfficeId, (apo, apol) => apol).AsEnumerable().GroupBy(apol => apol.AutoPostOfficeId).Where(x => x.Count() < allLanguages.Count).ToArray();
                foreach (var apols in apolsGroups)
                {
                    var languageIds = allLanguages.Select(lng => lng.Id)
                        .Except(apols.Select(apoLanguage => apoLanguage.LanguageId));
                    autoPostOfficeService.InsertAutoPostOfficeLanguageAsync(languageIds.Select(lId => new AutoPostOfficeLanguage
                    { Address = "<undefined>", Comment = "<undefined>", AutoPostOfficeId = apols.Key, LanguageId = lId }).ToArray());
                }
                return apoRepository;
            }
        }

        public async static Task<IRepository<DeliveryType>> InitiateLanguagesAsync(this IRepository<DeliveryType> dtRepository)
        {
            lock (syncDtl)
            {
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var deliveryTypeService = EngineContext.Current.Resolve<IDeliveryTypeService>();
                var allLanguages = languageService.GetAllLanguagesAsync().Result;
                foreach (var deliveryType in dtRepository.Table)
                {
                    if (deliveryTypeService.GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(deliveryType.Id).Result.Select(dtl => dtl.LanguageId).Distinct().Count() < allLanguages.Count)
                    {
                        var languageIds = allLanguages.Select(lng => lng.Id)
                            .Except(deliveryTypeService.GetDeliveryTypeLanguagesByDeliveryTypeIdAsync(deliveryType.Id).Result.Select(dtl => dtl.LanguageId)).ToArray();

                        deliveryTypeService.InsertDeliveryTypeLanguageAsync(languageIds.Select(lId => new DeliveryTypeLanguage { DeliveryTypeId = deliveryType.Id, TypeName = "<undefined>", LanguageId = lId }).ToArray());
                    }
                }
                return dtRepository;
            }
        }

        public static IRepository<DeliveryOperator> InitiateLanguages(this IRepository<DeliveryOperator> doRepository)
        {
            lock (syncDol)
            {
                var languageService = EngineContext.Current.Resolve<ILanguageService>();
                var deliveryOperatorService = EngineContext.Current.Resolve<IDeliveryOperatorService>();
                var allLanguages = languageService.GetAllLanguagesAsync().Result;
                foreach (var deliveryOperator in doRepository.Table)
                {
                    if (deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(deliveryOperator.Id).Result.Select(dtl => dtl.LanguageId).Distinct().Count() < allLanguages.Count)
                    {
                        var languageIds = allLanguages.Select(lng => lng.Id)
                            .Except(deliveryOperatorService.GetDeliveryOperatorLanguagesByDeliveryOperatorIdAsync(deliveryOperator.Id).Result.Select(dtl => dtl.LanguageId)).ToArray();

                        deliveryOperatorService.InsertDeliveryOperatorLanguage(languageIds.Select(lId => new DeliveryOperatorLanguage { DeliveryOperatorId = deliveryOperator.Id, OperatorName = "<undefined>", LanguageId = lId }).ToArray());
                    }
                }
                return doRepository;
            }
        }
        #endregion
    }
}