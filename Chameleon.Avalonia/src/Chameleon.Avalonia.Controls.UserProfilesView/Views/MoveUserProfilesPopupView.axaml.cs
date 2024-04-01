using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Interfaces.App.UserProfiles.Views.List;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class MoveUserProfilesPopupView : AutoViewModelLocatorControl, IMoveUserProfilesPopupView
{
    public MoveUserProfilesPopupView()
    {
        InitializeComponent();
    }
}