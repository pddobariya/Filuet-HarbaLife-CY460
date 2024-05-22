using HBL.Cyprus.Onlineordering.Payments.Saferpay.Models.DTO;
using Nop.Core;
using Nop.Services.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HBL.Cyprus.Onlineordering.Payments.Saferpay.Services
{
    public class SaferpayService : ISaferpayService
    {
        #region Fields 

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public SaferpayService(
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;        
        }

        #endregion

        #region Methods

        public async Task<SaferpayInitializeResponse> InitializeAsync(decimal orderTotal, string orderNumber, string returnSuccessUrl, string returnFailUrl, string notifyUrl, Guid orderGuid)
        {
            var settings =await GetSettingsAsync();
            var request = new SaferpayInitializeRequest()
            {
                RequestHeader = await MakeSaferpayRequestHeaderAsync(settings, orderGuid),
                TerminalId = settings.TerminalId,
                Payment = new Payment()
                {
                    Amount = new Amount()
                    {
                        Value = Math.Truncate(orderTotal * 100).ToString(),
                        CurrencyCode = settings.CurrencyCode
                    },
                    OrderId = orderNumber,
                    Description = "Herbalife order"
                },
                Authentication = new Authentication()
                {
                    ThreeDsChallenge = "FORCE"
                },
                Notification = new Notification()
                {
                    NotifyUrl = notifyUrl
                },
                ReturnUrls = new ReturnUrls()
                {
                    Success = returnSuccessUrl,
                    Fail = returnFailUrl
                }
            };
            return await SendHttpRequest<SaferpayInitializeResponse>("Payment/v1/PaymentPage/Initialize", request, settings);
        }

        public async Task<SaferpayAssertResponse> AssertAsync(string token)
        {
            var settings = await GetSettingsAsync();
            var request = new SaferpayAssertRequest()
            {
                RequestHeader = await MakeSaferpayRequestHeaderAsync(settings, Guid.NewGuid()),
                Token = token
            };
            return await SendHttpRequest<SaferpayAssertResponse>("Payment/v1/PaymentPage/Assert", request, settings);
        }

        public async Task<SaferpayCaptureResponse> CaptureAsync(string transactionId)
        {
            var settings = await GetSettingsAsync();
            var request = new SaferpayCaptureRequest()
            {
                RequestHeader = await MakeSaferpayRequestHeaderAsync(settings, Guid.NewGuid()),
                TransactionReference = new TransactionReference()
                {
                    TransactionId = transactionId
                }
            };

            return await SendHttpRequest<SaferpayCaptureResponse>("Payment/v1/Transaction/Capture", request, settings);
        }

        public async Task<SaferpayCancelResponse> CancelAsync(string transactionId)
        {
            var settings = await GetSettingsAsync();
            var request = new SaferpayCaptureRequest()
            {
                RequestHeader = await MakeSaferpayRequestHeaderAsync(settings, Guid.NewGuid()),
                TransactionReference = new TransactionReference()
                {
                    TransactionId = transactionId
                }
            };
            return await SendHttpRequest<SaferpayCancelResponse>("Payment/v1/Transaction/Capture", request, settings);
        }

        private async Task<T> SendHttpRequest<T>(string methodUrl, object content, SaferpayPaymentSettings paymentSettings)
        {
            using (var client = new HttpClient())
            {
                var url = new Uri(new Uri(paymentSettings.APIUrl + (paymentSettings.APIUrl.EndsWith("/") ? "" : "/")), methodUrl).ToString();
                // Create HTTP transport objects
                var httpRequest = new HttpRequestMessage();
                httpRequest.Method = new HttpMethod("POST");
                httpRequest.RequestUri = new Uri(url);

                string authInfo = $"{paymentSettings.APIUsername}:{paymentSettings.APIPassword}";
                httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo)));
                httpRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string requestContent = JsonSerializer.Serialize(content);
                httpRequest.Content = new StringContent(requestContent, Encoding.UTF8);
                httpRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=utf-8");

                HttpResponseMessage httpResponse = await client.SendAsync(httpRequest);
                string responseContent = "";
                if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    httpRequest.Dispose();
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();
                    }
                    SaferpayErrorResponse error = null;
                    try
                    {
                         error = JsonSerializer.Deserialize<SaferpayErrorResponse>(responseContent);
                    }
                    catch { }
                    httpResponse.Dispose();

                    InvalidOperationException exception = new InvalidOperationException(string.Format("Operation returned an invalid status code '{0}'", httpResponse.StatusCode));
                    if (error != null)
                        exception.Data["Error"] = error;

                    throw exception;
                }

                responseContent = await httpResponse.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<T>(responseContent);
                }
                catch (JsonException ex)
                {
                    httpRequest.Dispose();
                    if (httpResponse != null)
                    {
                        httpResponse.Dispose();
                    }
                    throw new SerializationException($"Unable to deserialize the response. {responseContent}", ex);
                }
            }
        }

        private async Task<SaferpayPaymentSettings> GetSettingsAsync()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            return await _settingService.LoadSettingAsync<SaferpayPaymentSettings>(storeScope);
        }

        private async Task<SaferpayRequestHeader> MakeSaferpayRequestHeaderAsync(SaferpayPaymentSettings paymentSettings, Guid orderGuid)
        {
            return await Task.FromResult(new SaferpayRequestHeader()
            {
                CustomerId = paymentSettings.CustomerId,
                RequestId = orderGuid.ToString(),
                RetryIndicator = 0,
                SpecVersion = paymentSettings.APISpecVersion
            });
        }

        #endregion
    }
}
