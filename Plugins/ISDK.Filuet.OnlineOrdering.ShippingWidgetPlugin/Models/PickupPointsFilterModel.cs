using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Dtos;
using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure.Attributes;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    [TypeScriptModel]
    public class PickupPointsFilterModel
    {
        #region Properties

        public int? CountryId { get; set; }
        public string City { get; set; }
        public string NameOrAddress { get; set; }
        internal PickupPointsFilterDto ToDto()
        {
            return new PickupPointsFilterDto(CountryId, City, NameOrAddress);
        }

        #endregion
    }
}