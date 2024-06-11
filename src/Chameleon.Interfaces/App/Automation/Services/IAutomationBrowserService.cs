using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface IAutomationBrowserService
    : ISingletonDependency
{
    void RunScript(IAutomationScriptDescription script, SystemBrowserType browserType, IList<IUserProfile> userProfiles);
}
