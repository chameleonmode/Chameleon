using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Interfaces.App.Automation.Playwright;
public class PlaywrightBrowserLaunchOptions 
    : SystemBrowserLaunchOptions
    , IPlaywrightBrowserLaunchOptions
{
    public string Script { get; set; }  
    public IDictionary<string, string> Arguments { get; set; }
}
