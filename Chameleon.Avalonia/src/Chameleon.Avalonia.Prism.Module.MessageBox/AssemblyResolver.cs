using System.Reflection;

namespace Chameleon.Avalonia.Prism.Module.MessageBox;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
