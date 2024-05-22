using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Text.RegularExpressions;

namespace Nop.Plugin.Payments.Payeezy.Validators
{
    public class EmailPropertyValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        #region Methods

        public static Regex EmailRegex = new Regex(@"^([a-zA-Z0-9_\+\-\.]+)@((\[[0-9]{1,3}" +
                      @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                      @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override string Name => "EmailPropertyValidator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            var email = value as string;
            if (String.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            return EmailRegex.IsMatch(email);
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "Email is not valid";
        
        #endregion
    }
}
