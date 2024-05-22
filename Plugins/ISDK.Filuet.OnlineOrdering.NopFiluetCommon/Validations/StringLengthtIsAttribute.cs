using System;
using System.ComponentModel.DataAnnotations;

namespace ISDK.Filuet.OnlineOrdering.NopFiluetCommon.Validations
{
    public class StringLengthIsAttribute : ValidationAttribute
    {
        #region Ctor

        public StringLengthIsAttribute(int count)
            : base("")
        {
            Count = count;
        }

        #endregion

        #region Methods

        public int Count { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Count <= 0)
            {
                return new ValidationResult(ErrorMessage);
            }

            if (value is null || ((String)value).Length != Count)
            {
                return new ValidationResult(ErrorMessage);
            }

            return null;
        }

        #endregion
    }
}
