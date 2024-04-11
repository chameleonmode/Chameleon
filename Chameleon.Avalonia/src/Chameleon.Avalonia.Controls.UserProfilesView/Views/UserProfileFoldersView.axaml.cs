using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.UserProfiles;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class UserProfileFoldersView : UserControl
    , IUserProfileFoldersView
{
    public UserProfileFoldersView()
    {
        InitializeComponent();
        DataContext = ContainerServiceHelper.Resolve<IUserProfileFoldersViewModel>();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IHaveInitialize sp)
            sp.InvokeInitializeAsyncCommand(e);
    }
}