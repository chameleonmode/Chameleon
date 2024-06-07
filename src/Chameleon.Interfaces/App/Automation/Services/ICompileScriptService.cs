using Chameleon.Interfaces.Ioc;
using System.Reflection;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface ICompileScriptService
    : ISingletonDependency
{
    MethodInfo? CompileScript(string script);
}
