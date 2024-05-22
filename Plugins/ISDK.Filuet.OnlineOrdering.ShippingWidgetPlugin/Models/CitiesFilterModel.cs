using ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Infrastructure.Attributes;

namespace ISDK.Filuet.OnlineOrdering.ShippingWidgetPlugin.Models
{
    [TypeScriptModel]
    public class CitiesFilterModel
    {
        #region Properties

        public int CountryId { get; set; }
        public string Name { get; set; }

        #endregion
    }
}