namespace Chameleon.Core.Extensions;

public static class TypeExtensions
{
    public static string GetDependencyName(this Type self)
    {
        ArgumentNullException.ThrowIfNull(self);
        ArgumentNullException.ThrowIfNull(self.FullName);
        return self.FullName;
    }
}
