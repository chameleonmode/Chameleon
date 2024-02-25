using System.Reflection;

namespace Chameleon.Infrastructure;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
