using Nop.Core.Domain.Configuration;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Configuration
{
    public class FiluetConfig
    {
        #region Properties

        public string FusionProxyConnectionProtocol { get; set; } = "https";

        public string FusionProxyConnectionAddress { get; set; } = "https://onlineorderproxy-poc.hrbl.com:8057/ProxyService.svc/FusionServiceProxy";

        public string LandingConnectionAddress { get; set; } = "https://uz-consult.filuet.ru";

        public string SettingTableName { get; set; } = nameof(Setting);

        #endregion
    }
}
