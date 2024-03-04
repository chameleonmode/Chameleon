using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.OutReachLinkView;

public partial class OutReachLinkView : UserControl, IOutReachLinkView
{
    private IUserProfile userProfile;

    public OutReachLinkView()
    {
        InitializeComponent();
    }

    public static readonly DirectProperty<OutReachLinkView, IUserProfile> UserProfileProperty =
        AvaloniaProperty.RegisterDirect<OutReachLinkView, IUserProfile>(
            nameof(UserProfile),
            o=> o.UserProfile,
            (o,v) => o.UserProfile = v);


    public IUserProfile UserProfile
    {
        get => userProfile; //(IUserProfile)GetValue(UserProfileProperty);
        set => userProfile = value;//SetValue(UserProfileProperty, value);
    }
}