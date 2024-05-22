using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Contracts;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Domain;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.DTO;
using ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Models;
using Nop.Plugin.Pickup.PickupInStore.Domain;
using Nop.Services.Shipping.Pickup;
using System.Threading.Tasks;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Factory
{
    public interface IShippingModelFactory
    {
        #region Methods

        Task<DictionaryModel> PreparePickupPointModelAsync(StorePickupPoint storePickupPoint);

        Task<ShippingComputationOptionModel> PrepareShippingComputationOptionModelAsync(
            FiluetFusionShippingComputationOption filuetFusionShippingComputationOption,
            bool isSelected);

        Task<ShippingMethodModel> PrepareShippingMethodModelAsync(IShippingInformationProvider shippingInformationProvider, bool isSelected);

        Task<ShippingMethodModel> PrepareShippingMethodModelAsync(IPickupPointProvider pickupPointProvider, bool isSelected);

        Task<FormFieldMetaModel> PrepareFormFieldMetaModelAsync(FormFieldMeta formFieldMeta);

        #endregion
    }
}