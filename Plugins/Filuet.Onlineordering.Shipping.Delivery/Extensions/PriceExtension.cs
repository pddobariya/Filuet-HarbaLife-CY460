using Filuet.Onlineordering.Shipping.Delivery.Domain;

namespace Filuet.Onlineordering.Shipping.Delivery.Extensions
{
    public static class PriceExtension
    {
        #region Methods

        public static decimal MinCriterionValue(this Price p)
        {
            if (!string.IsNullOrWhiteSpace(p.CriterionValues) && decimal.TryParse(p.CriterionValues.Split(';')[0], out var value))
            {
                return value;
            }

            return default;
        }

        public static decimal MaxCriterionValue(this Price p)
        {
            if (!string.IsNullOrWhiteSpace(p.CriterionValues) && decimal.TryParse(p.CriterionValues.Split(';')[1], out var value))
            {
                return value;
            }

            return decimal.MaxValue;
        }

        #endregion
    }
}
