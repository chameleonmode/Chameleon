using System.Reflection;

namespace Chameleon.Av.Fluent.Dialogs;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
