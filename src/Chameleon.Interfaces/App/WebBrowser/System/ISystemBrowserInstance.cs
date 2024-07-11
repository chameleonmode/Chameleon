using Chameleon.Interfaces.UserProfiles;
using System.Diagnostics;

namespace Chameleon.Interfaces.WebBrowser;

public interface ISystemBrowserInstance
{
    public event Action<ISystemBrowserLaunchOptions> OnProcessClosed;
    TaskCompletionSource<bool> OPtcs { get; }
    UserProfileSystemBrowserProcessEventArgs GetArgs(Process process);
    Task Open();
    Task MakeForeground();
    int Port { get; }
    Process? Brocess { get; set; }
    void Cleanup();
}
