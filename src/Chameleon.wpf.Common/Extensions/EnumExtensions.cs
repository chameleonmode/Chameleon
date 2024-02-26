using Chameleon.Common.Attributes;
using System;
using System.ComponentModel;
using DisplayNameAttribute = Chameleon.Common.Attributes.DisplayNameAttribute;

namespace Chameleon.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum self)
        {
            var fieldInfo = self.GetType().GetField(self.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return self.ToString();
        }

        public static string GetValue(this Enum self)
        {
            var fieldInfo = self.GetType().GetField(self.ToString());
            var attributes = (DescriptionWithValueAttribute[])fieldInfo
                .GetCustomAttributes(typeof(DescriptionWithValueAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Value;
            }
            return self.ToString();
        }

        public static string GetPermissionAttribute(this Enum self)
        {
            var fieldInfo = self.GetType().GetField(self.ToString());
            var attributes = (PermissionAttribute[])fieldInfo
                .GetCustomAttributes(typeof(PermissionAttribute), false);

            return attributes[0].Name;
        }

        public static string GetDisplayNameAttribute(this Enum self)
        {
            var fieldInfo = self.GetType().GetField(self.ToString());
            var attributes = (DisplayNameAttribute[])fieldInfo
                .GetCustomAttributes(typeof(DisplayNameAttribute), false);

            return attributes[0].Name;
        }
    }
}
