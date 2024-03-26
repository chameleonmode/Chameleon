using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Interfaces.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfileView;

public partial class UserProfileIdentityView : SubPageViewControl, IUserProfileIdentityView
{
    public UserProfileIdentityView()
    {
        InitializeComponent();
    }

    //public IUserProfile UserProfile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //IUserProfile IUserProfileSetter.UserProfile { set => throw new NotImplementedException(); }

    //IUserProfile IUserProfileGetter.UserProfile => throw new NotImplementedException();
}