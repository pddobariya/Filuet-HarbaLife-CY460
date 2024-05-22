using ISDK.Filuet.OnlineOrdering.CorePlugin.Models;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.CorePlugin.Services
{
    public class _1SServiceWrapper
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public _1SServiceWrapper(
            ISettingService settingService,
            ILogger logger)
        {
            _settingService = settingService;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<bool?> CheckPartner(string distributorId)
        {
            var client =await CreateClient();
            bool? flag = null;
            try
            {
                flag = bool.Parse((await client.CheckPartnerAsync(distributorId, String.Empty)).Body.result);
            }
            catch (Exception e)
            {
                await _logger.ErrorAsync("Ошибка проверки статуса партнера (ФизЛиц, ЮрЛиц)", e);
            }

            return flag;
        }

        public async Task<OrderStatusDto[]> GetOrderStatuses(string fusionOrderNumbers)
        {
            var client =await CreateClient();
            var getOrderStatusesResponseBody =(await client.GetOrderStatusesAsync(fusionOrderNumbers)).Body.@return;
            return JsonConvert.DeserializeObject<IEnumerable<OrderStatusDto>>(getOrderStatusesResponseBody, new IsoDateTimeConverter { DateTimeFormat = "dd.MM.yyyy H:mm:ss" }).ToArray();
        }

        public async Task<StockBalanceItemDto[]> GetStocks()
        {
            var client =await CreateClient();
            var stocksAsync =(await client.GetStocksAsync()).Body.result;
            return JsonConvert.DeserializeObject<IEnumerable<StockBalanceItemDto>>(stocksAsync).ToArray();
        }

        public async Task<bool> SendOrder(string orderJson)
        {
            var client =await CreateClient();
            var result =(await client.SendOrderAsync(orderJson)).Body.result;
            return bool.Parse(result);
        }

        private async Task<HLOrdersPortTypeClient> CreateClient()
        {
            var client = new HLOrdersPortTypeClient(HLOrdersPortTypeClient.EndpointConfiguration.HLOrdersSoap,(await _settingService.LoadSettingAsync<FiluetCorePluginSettings>()).CheckPartnerUrlAddress);
            HttpBindingBase myBinding;
            if (client.Endpoint.Address.Uri.Scheme == "https")
            {
                myBinding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
                ((BasicHttpsBinding)myBinding).Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = new X509ServiceCertificateAuthentication
                { CertificateValidationMode = X509CertificateValidationMode.None, RevocationMode = X509RevocationMode.NoCheck };
            }
            else
            {
                myBinding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                ((BasicHttpBinding)myBinding).Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            }
            client.Endpoint.Binding = myBinding;
            client.ClientCredentials.UserName.UserName = "webuser";
            client.ClientCredentials.UserName.Password = "kfVOpu";
            return client;
        }

        #endregion
    }
}
