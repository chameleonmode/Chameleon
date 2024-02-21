using Chameleon.App.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Chameleon.App.Licences.Attributes
{
    public class LicenseKeyAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            return LicenseKey.IsValid(value?.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be in format {LicenseKey.ExampleFormat}";
        }
    }
}
