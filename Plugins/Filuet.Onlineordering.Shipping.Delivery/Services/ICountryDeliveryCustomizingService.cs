using System.Threading.Tasks;

namespace Filuet.Onlineordering.Shipping.Delivery.Services
{
    public interface ICountryDeliveryCustomizingService
    {
        #region Methods

        Task<decimal> GetDeliveryPriceCriterionValue();
        Task<bool> IsOnlySelfPickup();

        #endregion
    }
}
