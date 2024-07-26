using Avalonia.Controls;
using Chameleon.Avalonia.Common.Helpers;
using Avpplication = Avalonia.Application;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class UserProfileView : UserControl
{
    public UserProfileView()
    {
        InitializeComponent();
        //TooltipManager.Attach(Avpplication.Current, this);
    }
}