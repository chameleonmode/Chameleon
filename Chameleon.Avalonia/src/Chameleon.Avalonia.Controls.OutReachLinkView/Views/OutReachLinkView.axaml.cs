using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.OutReachLinkView;

public partial class OutReachLinkView : UserControl, IOutReachLinkView
{
    public OutReachLinkView()
    {
        InitializeComponent();
    }
    public IUserProfile UserProfile
    {
        get;
        set;
    }
}