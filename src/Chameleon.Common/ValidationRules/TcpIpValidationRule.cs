using Chameleon.Interfaces.Ioc;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace Chameleon.Common.ValidationRules
{
    public class TcpIpPortRule : 
        ValidationRule 
        , ITransientDependency
    {
        public const int Min = 0;
        public const int Max = 65353;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int port = 0;

            try
            {
                port = (int)value;
            }
            catch (Exception e)
            {
                return new ValidationResult(false, $"Illegal characters or {e.Message}");
            }

            if ((port < Min) || (port > Max))
            {
                return new ValidationResult(false,
                  $"Please enter a port in the range: {Min}-{Max}.");
            }
            return ValidationResult.ValidResult;
        }
    }
}
