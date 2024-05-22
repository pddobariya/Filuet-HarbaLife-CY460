using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using FluentValidation;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Validators
{
    public partial class AutoPostOfficeValidator : BaseNopValidator<AutoPostOfficeDtoModel>
    {
        #region Methods

        public AutoPostOfficeValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor)
        {
            RuleFor(x => x.Address).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Fields.Address.Required"));
           
            RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
            {
                RuleFor(x => x.Address).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Plugins.ShippingMethods.Delivery.Fields.Address.Required"));
               
            });
            SetDatabaseValidationRules<DeliveryOperatorDto>(mappingEntityAccessor);
        }

        #endregion
    }
}
