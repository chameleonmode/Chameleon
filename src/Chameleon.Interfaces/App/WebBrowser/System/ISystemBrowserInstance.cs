using Chameleon.Interfaces.UserProfiles;
using System.Diagnostics;

namespace Chameleon.Interfaces.WebBrowser;

public interface ISystemBrowserInstance
{
    public event Action<ISystemBrowserLaunchOptions>? OnProcessClosed;
    public event Action<ISystemBrowserLaunchOptions>? OnProcessOpenError;
    TaskCompletionSource<bool> OPtcs { get; }
    UserProfileSystemBrowserProcessEventArgs GetArgs{get;}
    Task Open();
    void MakeForeground();
    int Port { get; }
    Process? Brocess { get; set; }
    void Cleanup();
}
