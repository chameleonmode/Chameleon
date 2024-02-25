using System.Reflection;

namespace Chameleon.Application;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
