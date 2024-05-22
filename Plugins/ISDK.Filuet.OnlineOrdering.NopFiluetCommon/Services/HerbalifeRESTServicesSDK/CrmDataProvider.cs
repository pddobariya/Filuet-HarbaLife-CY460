using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.Distributor;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos.HerbalifeRESTServicesSDK;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Extensions.HerbalifeRESTServicesSDK;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.HerbalifeRESTServicesSDK
{
    public partial class CrmDataProvider : ICrmDataProvider
    {
        #region Fields

        public const string SessionName = ".DistributorDetailedProfile.Session";
        public const string CookieName = ".DistributorDetailedProfile.24hours";
        private readonly HerbalifeRESTServicesClient _herbalifeRESTServicesClient;
        private readonly HttpContext _httpContext;

        #endregion

        #region Ctor

        public CrmDataProvider(
            string apiUrl,
            IHttpContextAccessor httpContextAccessor)
        {
            _herbalifeRESTServicesClient = new HerbalifeRESTServicesClient(apiUrl);
            _httpContext = httpContextAccessor.HttpContext;
        }
        #endregion

        #region Methods

        public Task<DistributorFOPPurchasingLimitsResponse> GetDistributorFOPPurchasingLimitsAsync(DistributorFOPPurchasingLimitsRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileSSOAsync(string accessToken)
        {
            string resultContent = await _herbalifeRESTServicesClient.GetAsync(accessToken, "distributor", new NameValueCollection() { { "type", "Detailed" } });
            return JsonConvert.DeserializeObject<DistributorDetailedProfileResponse>(resultContent);
        }

        public async Task<DistributorProfileResponse> GetDistributorProfileAsync(string accessToken)
        {
            string resultContent = await _herbalifeRESTServicesClient.GetAsync(accessToken, "distributorprofile", new NameValueCollection());
            return JsonConvert.DeserializeObject<DistributorProfileResponse>(resultContent);
        }

        public async Task<DistributorFullProfile> GetDistributorFullProfileAsync(string accessToken)
        {
            var distributorProfileResponse = await GetDistributorProfileAsync(accessToken);
            var distributorDetailedProfileResponse = await GetDistributorDetailedProfileAsync(accessToken);
            var distributorVolumeResponse = await GetDistributorVolumeAsync(accessToken);
            if (distributorVolumeResponse == null || distributorDetailedProfileResponse == null || distributorProfileResponse == null)
            {
                return null;
            }

            return new DistributorFullProfile
            {
                DistributorProfileResponse = distributorProfileResponse,
                DistributorDetailedProfileResponse =  distributorDetailedProfileResponse,
                DistributorVolumeResponse = distributorVolumeResponse
            };
        }

        public async Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(string accessToken)
        {

            string resultContent = _httpContext.Session.GetString(SessionName);
            var requestCookie = _httpContext.Request.Cookies[CookieName];
            if (string.IsNullOrWhiteSpace(resultContent) || requestCookie == null)
            {
                resultContent = await _herbalifeRESTServicesClient.GetAsync(accessToken, "distributor", new NameValueCollection() { { "type", "Detailed" } });
                _httpContext.Response.Cookies.Append(CookieName, "true", new CookieOptions { Expires = DateTimeOffset.Now.AddHours(24) });
                _httpContext.Session.SetString(SessionName, resultContent);
            }
            var result = DistributorDetailedProfileResponse.FromJson(resultContent);
            return result;
        }

        public async Task<DistributorVolumeResponse> GetDistributorVolumeAsync(string accessToken)
        {
            string volumeResultContent = await _herbalifeRESTServicesClient.GetAsync(accessToken, "volume", new NameValueCollection());
            var distributorVolumeResponse = JsonConvert.DeserializeObject<DistributorVolumeResponse[]>(volumeResultContent);

            return distributorVolumeResponse != null && distributorVolumeResponse.Any() ? distributorVolumeResponse.First() : new DistributorVolumeResponse();
        }

        public Task<DualMonthStatusResponse> GetDualMonthStatusAsync(DualMonthStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ShoppingCartTotalsResponse> GetShoppingCartTotalsAsync(ShoppingCartTotalsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SubmitOrderResponse> SubmitOrderAsync(SubmitOrderRequest request)
        {
            throw new NotImplementedException();
        }

        //public Task<DistributorDetailedProfileResponse> GetDistributorDetailedProfileAsync(string accessToken)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
