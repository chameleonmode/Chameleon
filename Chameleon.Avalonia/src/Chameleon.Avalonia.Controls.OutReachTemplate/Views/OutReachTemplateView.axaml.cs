using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.App.OutReach.Views;
using Chameleon.Interfaces.OutReach;
using Chameleon.Interfaces.UserProfiles;
using Chameleon.Interfaces.Views;

namespace Chameleon.Avalonia.Controls.OutReachTemplate;

public partial class OutReachTemplateView : UserControl,
    IOutReachTemplateView
{
    public OutReachTemplateView()
    {
        InitializeComponent();
    }

    public void SetOutReachTemplate(IOutReachTemplate template, IUserProfile userProfile)
    {
        //TODO: refactor
        IOutReachTemplateViewModel ViewModel = (IOutReachTemplateViewModel)DataContext;
        ViewModel.Id = 0;
        ViewModel.UserProfile = userProfile;
        ViewModel.OutReachTemplate = template;
        ViewModel.UserProfileTitle = userProfile.Title;
    }
}