using Chameleon.Interfaces.UserProfiles;
using System.Diagnostics;

namespace Chameleon.Interfaces.WebBrowser;

public interface ISystemBrowserInstance
{
    public event Action<ISystemBrowserLaunchOptions> OnProcessClosed;
    TaskCompletionSource<bool> OPtcs { get; }
    UserProfileSystemBrowserProcessEventArgs GetArgs(Process process);
    void Open();
}
