using System.Reflection;

namespace Chameleon.SystemBrowser;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
