using System;

namespace Chameleon.Interfaces.Ioc
{
    public static class TypeExtensions
    {
        public static string? GetDependencyName(this Type self)
        {
            ArgumentNullException.ThrowIfNull(self);
            return self?.FullName;
        }
    }
}