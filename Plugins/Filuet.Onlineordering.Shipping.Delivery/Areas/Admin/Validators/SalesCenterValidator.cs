using Filuet.Onlineordering.Shipping.Delivery.Domain;
using Filuet.Onlineordering.Shipping.Delivery.Models;
using FluentValidation;
using Nop.Data.Mapping;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Filuet.Onlineordering.Shipping.Delivery.Areas.Admin.Validators
{
    public partial class SalesCenterValidator : BaseNopValidator<SalesCenterDtoModel>
    {
        #region Methods

        public SalesCenterValidator(ILocalizationService localizationService, IMappingEntityAccessor mappingEntityAccessor) 
        {
            RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.Name.Required"));
            RuleFor(x => x.FreightCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.FreightCode.Required"));
            RuleFor(x => x.WarehouseCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.WarehouseCode.Required"));

            RuleSet(NopValidationDefaults.ValidationRuleSet, () =>
            {
                RuleFor(x => x.Name).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.Name.Required"));
                RuleFor(x => x.FreightCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.FreightCode.Required"));
                RuleFor(x => x.WarehouseCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Shipping.Delivery.Models.Fields.WarehouseCode.Required"));
            });
            SetDatabaseValidationRules<SalesCenterDto>(mappingEntityAccessor);
        }

        #endregion
    }
}
