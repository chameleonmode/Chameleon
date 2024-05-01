namespace Chameleon.Interfaces.WebBrowser;

public interface ISystemBrowserInstance
{
    public event Action<ISystemBrowserLaunchOptions> OnProcessClosed;
    Task Open();
}
