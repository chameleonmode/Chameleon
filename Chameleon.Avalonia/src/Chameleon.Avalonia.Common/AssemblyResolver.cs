using System.Reflection;

namespace Chameleon.Avalonia.Common;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}