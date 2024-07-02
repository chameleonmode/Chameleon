using Chameleon.Interfaces.App.Automation.Playwright;

namespace Chameleon.Infrastructure.App.Automation;
public class RunningAutomationBrowsers : IRunningAutomationBrowsers
{
    private List<IPlaywrightBrowserInstance> _browserInstances;

    public RunningAutomationBrowsers()
    {
        _browserInstances = new List<IPlaywrightBrowserInstance>();
    }

    public void RefreshBrowsers()
    {
        _browserInstances.Clear();
    }

    public void AddBrowser(IPlaywrightBrowserInstance browser)
    {
        _browserInstances.Add(browser);
    }

    public void RemoveBrowser(IPlaywrightBrowserInstance browser)
    {
        _browserInstances.Remove(browser);
    }

    public bool IsAllClosed => _browserInstances.Count == 0;
}
