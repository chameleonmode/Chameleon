using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class AddUserProfilesPopupView : AutoViewModelLocatorControl, IAddUserProfilesPopupView
{
    public AddUserProfilesPopupView()
    {
        InitializeComponent();
    }
}