namespace Chameleon.Core.Extensions;

public static class TypeExtensions
{
    public static string? GetDependencyName(this Type self)
    {
        ArgumentNullException.ThrowIfNull(self);
        return self.FullName;
    }
}
