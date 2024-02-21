using System;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.App
{
    public class IdentityAttribute : RangeAttribute
    {
        public IdentityAttribute()
            : base(1, int.MaxValue)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null && IsPropertyNullable(validationContext))
            {
                return ValidationResult.Success;
            }
            return base.IsValid(value, validationContext);
        }

        private bool IsPropertyNullable(ValidationContext validationContext)
        {
            var instance = validationContext.ObjectInstance;
            var instanceType = instance.GetType();
            var memberProperty = instanceType.GetProperty(validationContext.MemberName);
            var memberType = memberProperty.PropertyType;
            if (Nullable.GetUnderlyingType(memberType) != null)
            {
                return true;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                return $"Please enter a valid {name}";
            }
            return ErrorMessage;
        }
    }
}
