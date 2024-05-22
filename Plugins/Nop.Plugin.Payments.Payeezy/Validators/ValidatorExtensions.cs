using FluentValidation;
using FluentValidation.Validators;
using System;

namespace Nop.Plugin.Payments.Payeezy.Validators
{
    public static class ValidatorExtensions
    {
        #region Methods

        [Obsolete]
        public static IRuleBuilderOptions<T, string> IsEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new EmailValidator<T>());
        }

        #endregion
    }
}
