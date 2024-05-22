using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Constants.AttributeNames;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models.FiluetCommon;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure.Mapper;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Logging;
using NUglify.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Contracts
{
    public class DistributorService : IDistributorService
    {
        #region Fields

        private const string CACHE_KEY = "NOP_FILUET_COMMON_DISTRIBUTOR_SERVICE";
        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly ICrmDataProviderAdapter _crmDataProvider;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<CustomerLimits> _customerLimitsRepository;
        private readonly IAddressService _addressService;
        private readonly ISpRoleChecker _spRoleChecker;
        private readonly IDistributedCache _distributedCache;

        #endregion

        #region Ctor

        public DistributorService(ILogger logger, IWorkContext workContext,
            ICrmDataProviderAdapter crmDataProvider, IGenericAttributeService genericAttributeService,
            ICustomerService customerService,
            IRepository<CustomerLimits> customerLimitsRepository,
            IAddressService addressService,
            ISpRoleChecker spRoleChecker,
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _workContext = workContext;
            _crmDataProvider = crmDataProvider;
            _genericAttributeService = genericAttributeService;
            _customerService = customerService;
            _customerLimitsRepository = customerLimitsRepository;
            _addressService = addressService;
            _spRoleChecker = spRoleChecker;
            _distributedCache = distributedCache;
        }

        #endregion

        #region Methods

        [CanBeNull]
        public async Task<DistributorFullProfile> GetDistributorFullProfileAsync(Customer customer)
        {
            customer ??= await _workContext.GetCurrentCustomerAsync();
            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                var distributorFullProfile = await _crmDataProvider.GetDistributorFullProfileAsync(customer);
                stopWatch.Stop();

                await _logger.InsertLogAsync(LogLevel.Information,
                $"GetDistributorFullProfile: Username = {customer.Username}. Processing time: {stopWatch.ElapsedMilliseconds} ms.",
                JsonConvert.SerializeObject(distributorFullProfile));

                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DistributorProfile,
                    JsonConvert.SerializeObject(distributorFullProfile?.DistributorProfileResponse));
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DistributorDetailedProfile,
                    distributorFullProfile?.DistributorDetailedProfileResponse.ToJson());
                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DistributorVolume,
                    JsonConvert.SerializeObject(distributorFullProfile?.DistributorVolumeResponse));

                return distributorFullProfile;
            }
            catch (Exception exc)
            {
                await _logger.InsertLogAsync(LogLevel.Error, $"GetDistributorFullProfile: Username = {customer.Username}",
                    exc.ToString());
             
                var distributorDetailedProfileAttributeValue = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorDetailedProfile);
                var distributorProfileAttributeValue = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorProfile);
                var distributorVolumeAttributeValue = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorVolume);

                var result = customer is null ? new DistributorFullProfile() : new DistributorFullProfile
                {
                    DistributorProfileResponse = string.IsNullOrWhiteSpace(distributorDetailedProfileAttributeValue) ? new DistributorProfileResponse() :
                        JsonConvert.DeserializeObject<DistributorProfileResponse>(distributorDetailedProfileAttributeValue),
                    DistributorDetailedProfileResponse = string.IsNullOrWhiteSpace(distributorProfileAttributeValue) ? new DistributorDetailedProfileResponse() :
                        DistributorDetailedProfileResponse.FromJson(distributorProfileAttributeValue),
                    DistributorVolumeResponse = string.IsNullOrWhiteSpace(distributorVolumeAttributeValue) ? new DistributorVolumeResponse() :
                        JsonConvert.DeserializeObject<DistributorVolumeResponse>(distributorVolumeAttributeValue)
                };

                return result;
            }
        }

        public async Task<DistributorProfileResponse> GetDistributorProfileAsync(Customer customer)
        {
            try
            {
                var distributorProfile = await _crmDataProvider.GetDistributorProfile(customer);
                if (distributorProfile == null)
                    throw new Exception();

                await _logger.InsertLogAsync(LogLevel.Information, $"GetDistributorProfile: Username = {customer.Username}.",
                    JsonConvert.SerializeObject(distributorProfile));

                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DistributorProfile,
                    JsonConvert.SerializeObject(distributorProfile));

                return distributorProfile;
            }
            catch (Exception exc)
            {
               await _logger.InsertLogAsync(LogLevel.Error, $"GetDistributorProfile: Username = {customer.Username}",
                    exc.ToString());

                return JsonConvert.DeserializeObject<DistributorProfileResponse>(
                           await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorProfile)) ??
                       new DistributorProfileResponse();
            }
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(Customer customer)
        {
            try
            {
                DistributorDetailedProfileResponse distributorDetailedProfile = null;
                var distCache = await _distributedCache.GetStringAsync(customer.GetCustomerCacheKey(NopFiluetCommonDefaults.DistributorDetailedProfile));

                if (!string.IsNullOrWhiteSpace(distCache))
                {
                    distributorDetailedProfile = System.Text.Json.JsonSerializer.Deserialize<DistributorDetailedProfileResponse>(distCache);

                    await _logger.InsertLogAsync(LogLevel.Information,
                            $"GetDistributorDetailedProfile cached: Username = {customer.Username}.",
                            distributorDetailedProfile?.ToJson());

                    return distributorDetailedProfile;
                }
                else
                {
                    distributorDetailedProfile = await _crmDataProvider?.GetDistributorDetailedProfileAsync(customer);


                    if (_logger != null)
                        await _logger.InsertLogAsync(LogLevel.Information,
                            $"GetDistributorDetailedProfile: Username = {customer.Username}.",
                            distributorDetailedProfile?.ToJson());

                    if (distributorDetailedProfile != null)
                    {
                        await _distributedCache.SetStringAsync(customer.GetCustomerCacheKey(NopFiluetCommonDefaults.DistributorDetailedProfile), System.Text.Json.JsonSerializer.Serialize(distributorDetailedProfile),
                            new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });

                        await _genericAttributeService.SaveAttributeAsync(customer,
                            CustomerAttributeNames.DistributorDetailedProfile,
                            distributorDetailedProfile.ToJson());
                        return distributorDetailedProfile;
                    }
                    else
                    {
                        string json = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorDetailedProfile);
                        if (!string.IsNullOrEmpty(json))
                        {
                            return DistributorDetailedProfileResponse.FromJson(json);
                        }

                        return null;
                    }
                }


            }
            catch (Exception exc)
            {
                await _logger.InsertLogAsync(LogLevel.Error, $"GetDistributorDetailedProfile: Username = {customer.Username}",
                    exc.ToString());

                try
                {
                    string json = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorDetailedProfile);
                    return DistributorDetailedProfileResponse.FromJson(json);
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(string accessToken)
        {
            return await _crmDataProvider.GetDistributorDetailedProfile(accessToken);
        }

        public async Task<DistributorVolumeResponse> GetDistributorVolumeAsync(Customer customer)
        {
            try
            {
                var distributorVolume = await _crmDataProvider.GetDistributorVolume(customer);

                await _logger.InsertLogAsync(LogLevel.Information, $"GetDistributorVolume: Username = {customer.Username}.",
                    JsonConvert.SerializeObject(distributorVolume));

                await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DistributorVolume, JsonConvert.SerializeObject(distributorVolume));

                return distributorVolume;
            }
            catch (Exception exc)
            {
               await _logger.InsertLogAsync(LogLevel.Error, $"GetDistributorVolume: Username = {customer.Username}",
                    exc.ToString());

                var json = await _genericAttributeService.GetAttributeAsync<string>(customer,CustomerAttributeNames.DistributorVolume);
                return string.IsNullOrEmpty(json)
                    ? new DistributorVolumeResponse()
                    : JsonConvert.DeserializeObject<DistributorVolumeResponse>(json);
            }
        }

        public async Task<DistributorFopLimitsModel> GetDistributorVPLimitsSSOAsync(string memberId, string processingCountryCode)
        {
            try
            {
                DistributorFopLimitsModel vpLimits = await _crmDataProvider.GetDistributorVPLimitsSSO(memberId, processingCountryCode);

                return vpLimits;
            }
            catch (Exception exc)
            {

                await _logger.InsertLogAsync(LogLevel.Error, $"{nameof(GetDistributorVPLimitsSSOAsync)}: Error.", exc.ToString());
                return null;
            }
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileSSOAsync(string accessToken)
        {
            try
            {
                var distributorInfo =await _crmDataProvider.GetDistributorDetailedProfileSSO(accessToken);

                return distributorInfo;
            }
            catch (Exception exc)
            {
                await _logger.InsertLogAsync(LogLevel.Error, $"{nameof(GetDistributorDetailedProfileSSOAsync)}: Error.", exc.ToString());
                return null;
            }
        }

        public async Task SetDistributorRegionalRole(Customer customer, int? discount, string countryCode)
        {
            if (!discount.HasValue)
            {
                await _logger.InsertLogAsync(LogLevel.Error, "Discount profile is NULL");
                throw new NopException("Discount profile is NULL");
            }

            if (string.IsNullOrEmpty(countryCode))
            {
                await _logger.InsertLogAsync(LogLevel.Error, "CountryCode is NULL");
                throw new NopException("CountryCode is NULL");
            }

            var regionalRole = $"{countryCode}DIS{discount.Value}".ToUpper();

            var currentRegionalRole = await _customerService.GetCustomerRoleBySystemNameAsync(regionalRole);

            if (currentRegionalRole is not { Active: true })
            {
                regionalRole = $"DEFDIS{discount.Value}".ToUpper();
            }

            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);

            var disRoles = customerRoles.Where(cr => cr.SystemName.ToUpper().Contains("DIS")).ToList();
            if (disRoles.Any() && disRoles.Any(r => r.SystemName != regionalRole))
            {
                disRoles.ForEach(async r =>
                {
                    if (r.SystemName != regionalRole.ToString())
                        await _customerService.RemoveCustomerRoleMappingAsync(customer, r);
                });
            }

            var newDisCustomerRole = await _customerService.GetCustomerRoleBySystemNameAsync(regionalRole);
            if (newDisCustomerRole == null)
            {
                await _logger.InsertLogAsync(LogLevel.Error, $"{regionalRole} regional role could not be loaded");
                throw new NopException($"{regionalRole} regional role could not be loaded");
            }

            if (!customerRoles.Select(r => r.SystemName).ToList().Contains(newDisCustomerRole.SystemName))
            {
                await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                { CustomerRoleId = newDisCustomerRole.Id, CustomerId = customer.Id });
            }
        }

        public async Task SetDistributorRegionalRole(Customer customer, DistributorDetailedProfileResponse profile)
        {
            if (profile == null)
                throw new NopException($"{customer.Email} DistributorDetailedProfileResponse is NULL");

            if (profile.Discount > 0)
            {
                if (profile.ResidenceCountryCode == null)
                    throw new NopException($"{customer.Email} ResidenceCountryCode is NULL");

                var regionalRole = $"{profile.ResidenceCountryCode}DIS{profile.Discount}".ToUpper();

                var currentRegionalRole = await _customerService.GetCustomerRoleBySystemNameAsync(regionalRole);

                if (currentRegionalRole is not { Active: true })
                    regionalRole = $"DEFDIS{profile.Discount}".ToUpper();


                var customerRoles = await _customerService.GetCustomerRolesAsync(customer);

                var disRoles = customerRoles.Where(cr => cr.SystemName.ToUpper().Contains("DIS"));

                if (disRoles.Any() && disRoles.Any(r => r.SystemName != regionalRole))
                {
                    disRoles.ForEach(async r =>
                    {
                        if (r.SystemName != regionalRole.ToString())
                            await _customerService.RemoveCustomerRoleMappingAsync(customer, r);
                    });
                }

                var newDisCustomerRole = await _customerService.GetCustomerRoleBySystemNameAsync(regionalRole);

                if (newDisCustomerRole == null)
                    throw new NopException($"{customer.Email} {regionalRole} regional role could not be loaded");


                if (!customerRoles.Select(r => r.SystemName).ToList().Contains(newDisCustomerRole.SystemName))
                    await _customerService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping
                    { CustomerRoleId = newDisCustomerRole.Id, CustomerId = customer.Id });
            }
            else
            {
                throw new NopException($"{customer.Email} Discount profile is NULL");
            }
        }

        public async Task<CustomerLimits> PutDistributorVpLimits(Customer customer, string memberId, string processingCountryCode)
        {
            var distributorFopLimitsModel = await GetDistributorVPLimitsSSOAsync(memberId, processingCountryCode);

            if (distributorFopLimitsModel == null)
            {
                return null;
            }

            var customerLimits = _customerLimitsRepository.Table.FirstOrDefault(x => x.CustomerId == customer.Id);

            var res = AutoMapperConfiguration.Mapper.Map<CustomerLimits>(distributorFopLimitsModel);
            res.CustomerId = customer.Id;
            res.IsValidInfo = true;
            if (customerLimits == null)
            {
                await _customerLimitsRepository.InsertAsync(res);
            }
            else
            {
                res.Id = customerLimits.Id;
                await _customerLimitsRepository.UpdateAsync(res);
            }

            return res;
        }

        public async Task SaveCustomerDataAsync(Customer customer, DistributorDetailedProfileResponse profile)
        {
            var mainConditionsAccepted = await _genericAttributeService.GetAttributeAsync<bool>(customer,
                CoreGenericAttributes.MainConditionsAcceptedAttribute);

            if (mainConditionsAccepted)
                await _genericAttributeService.SaveAttributeAsync(customer,
                    CoreGenericAttributes.MainConditionsAcceptedAttribute, false);

            var apfMessageAccepted = await _genericAttributeService.GetAttributeAsync<bool>(customer,CoreGenericAttributes.ApfMessageAcceptedAttribute);

            if (apfMessageAccepted)
                await _genericAttributeService.SaveAttributeAsync(customer,
                    CoreGenericAttributes.ApfMessageAcceptedAttribute, false);

            var apfMessageDeclined = await _genericAttributeService.GetAttributeAsync<bool>(customer,CoreGenericAttributes.ApfMessageDeclinedAttribute);

            if (apfMessageDeclined)
                await _genericAttributeService.SaveAttributeAsync(customer,
                    CoreGenericAttributes.ApfMessageDeclinedAttribute, false);

            await _genericAttributeService.SaveAttributeAsync(customer,
                CustomerAttributeNames.IsShowShippingCountryPopup, true);

            await _genericAttributeService.SaveAttributeAsync(customer, CustomerAttributeNames.DistributorDetailedProfile,
                    System.Text.Json.JsonSerializer.Serialize(profile));

            if(profile.FirstName != null && profile.LastName != null)
            {
                customer.FirstName = profile.FirstName;
                customer.LastName = profile.LastName;
                await _customerService.UpdateCustomerAsync(customer);
            }


            await _genericAttributeService.SaveAttributeAsync(customer,
                CustomerAttributeNames.CountryOfProcessing,
                profile.ProcessingCountryCode);

            var distCache = await _distributedCache.GetStringAsync(customer.GetCustomerCacheKey(NopFiluetCommonDefaults.DistributorDetailedProfile));

            if (!string.IsNullOrWhiteSpace(distCache))
            {
                await _distributedCache.RemoveAsync(customer.GetCustomerCacheKey(NopFiluetCommonDefaults.DistributorDetailedProfile));
            }
        }

        public async Task PutAddressesFromOracleAsync(Customer customer, DistributorDetailedProfileResponse distributorInfo)
        {
            if (distributorInfo.Addresses?.BillingAddress != null)
            {
                Nop.Core.Domain.Common.Address addressBA = null;
                if (customer.BillingAddressId == null)
                {
                    if (customer.ShippingAddressId == null)
                    {
                        addressBA = new Nop.Core.Domain.Common.Address
                        {
                            Address1 = distributorInfo.Addresses
                                ?.BillingAddress
                                ?.FullAddress,
                            City = distributorInfo.Addresses
                                ?.BillingAddress.City,
                            Email = distributorInfo.Email,
                            FirstName = distributorInfo.FirstName,
                            LastName = distributorInfo.LastName,
                            PhoneNumber = distributorInfo.Phone
                        };
                        await _addressService.InsertAddressAsync(addressBA);
                        await _customerService.InsertCustomerAddressAsync(customer, addressBA);
                        customer.ShippingAddressId = addressBA.Id;
                    }

                    customer.BillingAddressId = customer.ShippingAddressId;
                    await _customerService.UpdateCustomerAsync(customer);
                }

                if (addressBA == null)
                {
                    addressBA = await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value);
                    await _customerService.InsertCustomerAddressAsync(customer, addressBA);
                }


                addressBA.Email ??=
                    customer.Email ?? distributorInfo.Email;
                await _addressService.UpdateAddressAsync(addressBA);
            }
        }

        public void InvalidateDistributorInfoAsync(Customer customer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}