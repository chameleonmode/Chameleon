using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Interfaces.UserProfileFolders;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class UserProfileFoldersView : AutoViewModelLocatorControl
    , IUserProfileFoldersView
{
    public UserProfileFoldersView()
    {
        InitializeComponent();
    }
}