using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface IAutomationBrowserService
    : ISingletonDependency
{
    void RunScript(IAutomationScriptDescription script, IList<IUserProfile> userProfiles);
}
