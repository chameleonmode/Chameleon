using System;
using System.Linq;
using System.Text;

namespace Chameleon.App.ValueObjects
{
    public class LicenseKey
    {
        public static string ExampleFormat;
        private static string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static char Separator = '-';
        private static int CharCount = 16;
        private static int SeparatorCount = 3;
        private static int Length = CharCount + SeparatorCount;

        static LicenseKey()
        {
            ExampleFormat = GetExampleFormat();
        }

        private static string GetExampleFormat()
        {
            var example = Generate()
                .Select(c => c != Separator ? 'X' : Separator)
                ;

            return string.Join(string.Empty, example);
        }

        public string Value { get; }
        protected LicenseKey(string value)
        {
            Value = value;
        }

        public static bool IsValid(string key)
        {
            if (key == null)
            {
                return false;
            }

            if (key.Length != Length)
            {
                return false;
            }

            if (key.All(IsValidChar))
            {
                return true;
            }
            return false;
        }

        private static bool IsValidChar(char keyChar)
        {
            if (keyChar == Separator)
            {
                return true;
            }

            if (ValidChars.Contains(keyChar))
            {
                return true;
            }
            return false;
        }

        public static LicenseKey Create(string value)
        {
            value = value?.Trim().ToUpperInvariant();
            if (IsValid(value))
            {
                return new LicenseKey(value);
            }
            throw new FormatException();
        }

        public static LicenseKey GetOrCreate(string value)
        {
            value = value?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return Create();
            }
            return Create(value);
        }

        public static LicenseKey Create()
        {
            return new LicenseKey(Generate());
        }

        public static string Generate()
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var sb = new StringBuilder(Length);
            for (var i = 0; i < CharCount; ++i)
            {
                var index = random.Next(0, ValidChars.Length);
                sb.Append(ValidChars[index]);
                if (i == 3 || i == 7 || i == 11)
                {
                    sb.Append(Separator);
                }
            }
            return sb.ToString();
        }

        public static implicit operator string(LicenseKey licenseKey)
        {
            return licenseKey?.Value;
        }

        public static explicit operator LicenseKey(string licenseKey)
        {
            return Create(licenseKey);
        }
    }
}
