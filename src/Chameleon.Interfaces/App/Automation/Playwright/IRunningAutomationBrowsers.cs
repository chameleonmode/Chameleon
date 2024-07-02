using Chameleon.Interfaces.Ioc;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public interface IRunningAutomationBrowsers : ITransientDependency
{
    void AddBrowser(IPlaywrightBrowserInstance browser);
    void RefreshBrowsers();
    void RemoveBrowser(IPlaywrightBrowserInstance browser);
    bool IsAllClosed { get; }
}
