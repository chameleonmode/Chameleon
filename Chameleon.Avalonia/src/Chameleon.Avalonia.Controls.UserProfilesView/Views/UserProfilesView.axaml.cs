using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class UserProfilesView : AutoViewModelLocatorControl
        , IUserProfilesView
{
    public UserProfilesView()
    {
        InitializeComponent();
    }

    public Func<IUserProfile, bool> Filter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Refresh()
    {
        throw new NotImplementedException();
    }
}