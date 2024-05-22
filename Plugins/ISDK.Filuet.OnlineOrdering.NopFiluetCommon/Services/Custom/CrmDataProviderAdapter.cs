using Filuet.Hrbl.Ordering.Abstractions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Exceptions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.FusionProxyClientSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class CrmDataProviderAdapter : ICrmDataProviderAdapter
    {
        #region Fields

        private static ConcurrentDictionary<int, object> syncDictionary =
           new();
        private readonly ICrmDataProvider _dataProvider;
        private readonly IRepository<CustomerGenericAttribute> _customerGenericAttributeRepository;
        private readonly IRestApiClient _restApiClient;

        #endregion

        #region Ctor

        public CrmDataProviderAdapter(ICrmDataProvider dataProvider, IRepository<CustomerGenericAttribute> customerGenericAttributeRepository, IRestApiClient restApiClient)
        {
            _dataProvider = dataProvider;
            _customerGenericAttributeRepository = customerGenericAttributeRepository;
            _restApiClient = restApiClient;
        }

        #endregion

        #region Methods

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileSSO(string accessToken)
        {
            return await _dataProvider.GetDistributorDetailedProfileSSOAsync(accessToken);
        }

        public async Task<DistributorProfileResponse> GetDistributorProfile(Customer customer)
        {
            return (await GetDistributorFullProfileAsync(customer))?.DistributorProfileResponse;
           
        }

        public async Task<DistributorFullProfile> GetDistributorFullProfileAsync(Customer customer)
        {
            object syncObj = null;
            if (!syncDictionary.TryGetValue(customer.Id, out syncObj))
            {
                syncObj = new();
                syncDictionary.TryAdd(customer.Id, syncObj);
            }

            DistributorFullProfile distributorFullProfile;

            lock (syncObj)
            {
                var customerGenericAttribute = _customerGenericAttributeRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);
                if (customerGenericAttribute != null && !customerGenericAttribute.IsValidInfo || customerGenericAttribute == null)
                {
                    distributorFullProfile =  _dataProvider.GetDistributorFullProfileAsync((customer.GetAccessTokenAsync()?.Result)?.ToString())?.Result;
                    if (distributorFullProfile == null)
                        return null;
                    
                    distributorFullProfile.Discount = Convert.ToInt32(distributorFullProfile.DistributorDetailedProfileResponse.Discount);
                    var res = AutoMapperConfiguration.Mapper.Map<CustomerGenericAttribute>(distributorFullProfile);
                    res.CustomerId = customer.Id;
                    if (customerGenericAttribute == null)
                    {
                        _customerGenericAttributeRepository.InsertAsync(res).Wait();
                    }
                    else
                    {
                        res.Id = customerGenericAttribute.Id;
                        _customerGenericAttributeRepository.UpdateAsync(res).Wait();
                    }
                }
                else
                {
                    distributorFullProfile =
                        AutoMapperConfiguration.Mapper.Map<DistributorFullProfile>(customerGenericAttribute);
                }
            }

            return  await Task.FromResult(distributorFullProfile);
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(Customer customer)
        {
            return (await GetDistributorFullProfileAsync(customer))?.DistributorDetailedProfileResponse;
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfile(string accessToken)
        {
            try
            {
                return await _dataProvider.GetDistributorDetailedProfileAsync(accessToken);
            }
            catch (Newtonsoft.Json.JsonReaderException exc)
            {
                throw new DistributorDetailedException(exc.Message);
            }
        }

        public async Task<DistributorVolumeResponse> GetDistributorVolume(Customer customer)
        {
            return (await GetDistributorFullProfileAsync(customer))?.DistributorVolumeResponse;
        }

        public void InvalidateDistributorInfo(Customer customer)
        {
            object syncObj = null;
            if (!syncDictionary.TryGetValue(customer.Id, out syncObj))
            {
                syncObj = new();
                syncDictionary.TryAdd(customer.Id, syncObj);
            }

            lock (syncObj)
            {
                var customerGenericAttribute = _customerGenericAttributeRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);
                if (customerGenericAttribute != null && customerGenericAttribute.IsValidInfo)
                {
                    customerGenericAttribute.IsValidInfo = false;
                    _customerGenericAttributeRepository.UpdateAsync(customerGenericAttribute).Wait();
                }

                var customerLimitsRepository = EngineContext.Current.Resolve<IRepository<CustomerLimits>>();
                var customerLimits = customerLimitsRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);
                if (customerLimits != null && customerLimits.IsValidInfo)
                {
                    customerLimits.IsValidInfo = false;
                    customerLimitsRepository.UpdateAsync(customerLimits).Wait();
                }
            }
        }

        public async Task<DistributorFopLimitsModel> GetDistributorVPLimitsSSO(string memberId, string processingCountryCode)
        {
            FOPPurchasingLimitsResult limits =await _restApiClient.GetDSFOPPurchasingLimits(memberId, processingCountryCode);


            DSPurchasingLimit dsPurchasingLimit = null;
            DSFOPLimit fopLimit = null;
            try
            {
                dsPurchasingLimit = limits.DSPurchasingLimits?.FirstOrDefault(pl => pl.PPVOrderMonth == $"{DateTime.Now:yyyy}/{DateTime.Now:MM}");
            }
            catch
            {
            }
            try
            {
                fopLimit = limits.FopLimit;
            }
            catch
            {
            }
            var distributorLimits = new DistributorFopLimitsModel()
            {
                InFopPeriod = limits.FopLimit?.FOPFirstOrderDate?.AddDays(limits.FopLimit.FOPThresholdPeriod ?? 0) > DateTime.Now,
                FopLimit = (double)(fopLimit?.AvailableFOPLimit ?? 0),
                PcLimit = (double)(dsPurchasingLimit?.AvailablePCLimit ?? 0)
            };
            return distributorLimits;
        }

        #endregion
    }
}
