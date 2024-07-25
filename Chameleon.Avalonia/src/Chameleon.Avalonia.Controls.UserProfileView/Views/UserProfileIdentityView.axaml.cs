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
        //Description = "Customize profile related data";
        //PreviewImage = ApplicationHelper.TryGetResource<IconSource>("ProfilePageIcon");
    }

    public override Visual? AnimateVisual { get => UPView; set => base.AnimateVisual = value; }
}