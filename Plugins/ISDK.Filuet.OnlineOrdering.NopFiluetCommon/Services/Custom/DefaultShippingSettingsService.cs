using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Shipping;
using System.Linq;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Services.Custom
{
    public class DefaultShippingSettingsService : IDefaultShippingSettingsService
    {
        #region Fields

        private readonly IShippingWidgetService _shippingWidgetService;

        #endregion

        #region Ctor

        public DefaultShippingSettingsService(IShippingWidgetService shippingWidgetService)
        {
            _shippingWidgetService = shippingWidgetService;
        }

        #endregion

        #region Methods

        public async Task<string> GetDefaultWareHouseAsync()
        {
            return (await _shippingWidgetService.GetShippingComputationOptionsAsync()).FirstOrDefault(sco => sco.IsSelected)
                ?.WarehouseCode.Split(';')[0];
        }

        #endregion
    }
}
