using System.Reflection;

namespace Chameleon.Domain;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
