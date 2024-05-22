using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using FluentValidation;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Validators
{
    public partial class DeliveryOperatorsCityValidator : BaseNopValidator<DeliveryOperatorsCityModel>
    {
        #region Methods

        public DeliveryOperatorsCityValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.CityName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.DeliveryOperatorsCityModel.Fields.CityName.Required"));
            RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
            {
                RuleFor(x => x.CityName).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.Name.Required"));
            });
            SetDatabaseValidationRules<DeliveryOperatorsCityDto>(mappingEntityAccessor);
        }

        #endregion
    }
}
