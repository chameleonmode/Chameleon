using System;

namespace Chameleon.Interfaces.Ioc
{
    public static class TypeExtensions
    {
        public static string GetDependencyName(this Type self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(Type));
            }
            return self.FullName;
        }
    }
}