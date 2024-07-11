using Chameleon.Interfaces.App.Automation.Entities;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.Ioc;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.App.Automation.Services;
public interface IAutomationBrowserService
    : ISingletonDependency
{
    Task RunScript(
        IAutomationScriptDescription script, 
        SystemBrowserType browserType,
        IList<IUserProfileActionsViewModel> userProfiles,
        CancellationToken token,
        bool record);
}
