using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Controls;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Avalonia.Common.Helpers;
using Chameleon.Avalonia.Controls.UserProfileView.ViewModels;
using Chameleon.Interfaces.UserProfiles;
using FluentAvalonia.UI.Controls;

namespace Chameleon.Avalonia.Controls.UserProfileView;

public partial class UserProfileIdentityView : SubPageViewControl, IUserProfileIdentityView
{
    public UserProfileIdentityView()
    {
        InitializeComponent();
        //ControlName = (DataContext as UserProfileIdentityViewModel)?.UserProfileModel.Title ?? "UserProfileModel.Title";
        Description = "Customize profile-related data";
        PreviewImage = ApplicationHelper.TryGetResource<IconSource>("ProfilePageIcon");
    }

    //public IUserProfile UserProfile { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    //IUserProfile IUserProfileSetter.UserProfile { set => throw new NotImplementedException(); }

    //IUserProfile IUserProfileGetter.UserProfile => throw new NotImplementedException();
}