using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Domain.Entities;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.RssFeed;

public partial class RssFeedView : UserControl
        , IRssFeedView
{
    public RssFeedView()
    {
        InitializeComponent();
    }

    public static readonly DirectProperty<RssFeedView, IUserProfile> UserProfileProperty =
        AvaloniaProperty.RegisterDirect<RssFeedView, IUserProfile>(
            nameof(UserProfile),
            o => o.UserProfile,
            (o, v) => o.UserProfile = v);


    private IUserProfile userProfile;
    public IUserProfile UserProfile
    {
        get => userProfile; //(IUserProfile)GetValue(UserProfileProperty);
        set => userProfile = value;//SetValue(UserProfileProperty, value);
    }
}