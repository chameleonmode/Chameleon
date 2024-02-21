using System;
using System.Text;

namespace Chameleon.App.ValueObjects
{
    public class AssistantLicenseKey : LicenseKey
    {
        private const string ASSISTANT_LICENSE_KEY = "KEY";
        public AssistantLicenseKey(string value)
            : base(value)
        {
            
        }

        public static new bool IsValid(string key)
        {
            if (!LicenseKey.IsValid(key))
            {
                return false;
            }

            if (key.StartsWith(ASSISTANT_LICENSE_KEY))
            {
                return true;
            }

            return false;
        }

        public static new AssistantLicenseKey Create(string value)
        {
            value = value?.Trim().ToUpperInvariant();
            if (IsValid(value))
            {
                return new AssistantLicenseKey(value);
            }
            throw new FormatException();
        }

        public static new string Generate()
        {
            var assistantLicenseKey = LicenseKey.Generate();
            var aStringBuilder = new StringBuilder(assistantLicenseKey);
            aStringBuilder.Remove(0, 3);
            aStringBuilder.Insert(0, "KEY");
            assistantLicenseKey = aStringBuilder.ToString();
            return assistantLicenseKey;
        }
    }
}
