using Chameleon.Interfaces.App.Automation.ExternalScript;
using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface ICompileScriptService
    : ISingletonDependency
{
    IExternalScript CompileScript(string script);
}
