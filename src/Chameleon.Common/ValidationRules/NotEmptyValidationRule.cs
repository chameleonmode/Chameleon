using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Chameleon.Common.ValidationRules
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            // Get and convert the value
            string stringValue = GetBoundValue(value) as string;

            return string.IsNullOrWhiteSpace((stringValue ?? "").ToString())
                ? new ValidationResult(false, "Field is required.")
                : ValidationResult.ValidResult;
        }

        private object GetBoundValue(object value)
        {
            if (value is BindingExpression)
            {
                // ValidationStep was UpdatedValue or CommittedValue (Validate after setting)
                // Need to pull the value out of the BindingExpression.
                var binding = (BindingExpression)value;

                // Get the bound object and name of the property
                object dataItem = binding.DataItem;
                string propertyName = binding.ParentBinding.Path.Path;

                // Extract the value of the property.
                object propertyValue = dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);

                // This is what we want.
                return propertyValue;
            }
            else
            {
                // ValidationStep was RawProposedValue or ConvertedProposedValue
                // The argument is already what we want!
                return value;
            }
        }
    }
}
