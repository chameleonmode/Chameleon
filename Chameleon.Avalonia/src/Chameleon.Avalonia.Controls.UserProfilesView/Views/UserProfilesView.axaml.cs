using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfileFolders;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class UserProfilesView : UserControl
        , IUserProfilesView
{
    public UserProfilesView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IUserProfilesViewModel>();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IHaveInitialize sp)
            sp.InvokeInitializeAsyncCommand(e);
    }
}