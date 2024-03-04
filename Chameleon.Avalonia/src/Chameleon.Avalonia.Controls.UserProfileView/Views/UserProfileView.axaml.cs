using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.WebBrowser;

namespace Chameleon.Avalonia.Controls.UserProfileView;

public partial class UserProfileView : UserControl, IUserProfileView
{
    public UserProfileView()
    {
        InitializeComponent();
    }

    public UserProfileViewTab Tab => throw new NotImplementedException();

    public IWebBrowserView WebBrowserView => throw new NotImplementedException();

    public IUserProfile UserProfile => throw new NotImplementedException();

    public string Title => throw new NotImplementedException();

    public void OpenWebBrowserNewTab(string url)
    {
        throw new NotImplementedException();
    }

    public void SetUserProfile(IUserProfile userProfile, UserProfileViewTab tab, OutReachViewTab outReachTab = OutReachViewTab.Rss)
    {
        throw new NotImplementedException();
    }

    public object GetContext()
    {
        throw new NotImplementedException();
    }

    public void SetContext(object context)
    {
        throw new NotImplementedException();
    }
}