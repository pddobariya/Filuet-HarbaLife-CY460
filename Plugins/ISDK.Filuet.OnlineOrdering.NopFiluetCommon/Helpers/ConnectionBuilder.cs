using Filuet.Hrbl.Ordering.Abstractions;
using Filuet.Hrbl.Ordering.Adapter;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom;
using Nop.Core.Infrastructure;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Helpers
{
    public static class ConnectionBuilder
    {
        #region Methods

        public const string SERVICE_CONSUMER = "AAKIOSK";
        public static Binding GetBinding(string protocol)
        {
            var casedProtocol = protocol.ToLower();
            if (!casedProtocol.Equals("https") && 
                !casedProtocol.Equals("http"))
            {
                throw new ProtocolException("Unknown binding protocol");
            }

            var binding = new BasicHttpBinding()
            {
                MaxBufferSize = int.MaxValue,
                ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max,
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = true,
            };
            if (casedProtocol.Equals("https"))
            {
                binding.Security.Mode = BasicHttpSecurityMode.Transport;
            }

            return binding;
        }

        public static EndpointAddress GetEndpointAddress(string address)
        {
            return new EndpointAddress(address);
        }

        public static X509ServiceCertificateAuthentication GetSslCertificateAuthentication(bool validate)
        {
            if (validate)
            {
                return new X509ServiceCertificateAuthentication();
            }
            else
            {
                return new X509ServiceCertificateAuthentication()
                {
                    CertificateValidationMode = X509CertificateValidationMode.None,
                    RevocationMode = X509RevocationMode.NoCheck,
                };
            }
        }

        public static IHrblOrderingAdapter GetRestApiAdapter()
        {
            var herbalifeEnvironment =
                EngineContext.Current.Resolve<IHerbalifeEnvironment>()?.GetEnvironmentCode().Result ?? "PRS";

            var defaultSettings = new HrblOrderingAdapterSettingsBuilder();

            defaultSettings =
                defaultSettings.WithUri($"https://herbalife-oegdevws.hrbl.com/Order/HLOnlineOrdering/{herbalifeEnvironment.ToLower()}/");

            switch (herbalifeEnvironment)
            {
                case "TS3":
                    defaultSettings =
                        defaultSettings.WithUri("https://herbalife-oegdevws.hrbl.com/Order/HLOnlineOrdering/ts3/");
                    break;
                case "PRS":
                    defaultSettings =
                        defaultSettings.WithUri("https://herbalife-oegdevws.hrbl.com/Order/HLOnlineOrdering/prs/");
                    break;
                case "PROD":
                    defaultSettings =
                        defaultSettings.WithUri("https://herbalife-econnectslc.hrbl.com/Order/HLOnlineOrdering/prod/");
                    break;
            }

            switch (herbalifeEnvironment)
            {
                case "TS3":
                case "PRS":
                    defaultSettings = defaultSettings.WithServiceConsumer(SERVICE_CONSUMER)
                        .WithOrganizationId(73)
                        .WithCredentials("hlfnord", "welcome123");
                    break;
                case "PROD":
                    defaultSettings = defaultSettings.WithServiceConsumer(SERVICE_CONSUMER)
                        .WithOrganizationId(73)
                        .WithCredentials("hlfnord", "F1uT2H1n@0rd");
                    break;
                default:
                    defaultSettings = defaultSettings.WithServiceConsumer(SERVICE_CONSUMER)
                        .WithOrganizationId(73)
                        .WithCredentials("hlfnord", "welcome123");
                    break;
            }
            return new HrblOrderingAdapter(defaultSettings.Build());
        }

        #endregion
    }
}
