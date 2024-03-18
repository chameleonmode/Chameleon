using System.Reflection;

namespace Chameleon.Avalonia.Controls.Settings;

public static class AssemblyResolver
{
    public static Assembly GetAssembly()
    {
        return Assembly.GetExecutingAssembly();
    }
}
