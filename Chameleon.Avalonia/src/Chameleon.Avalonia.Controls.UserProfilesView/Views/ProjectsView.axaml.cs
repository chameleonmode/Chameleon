using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Interfaces.App.UserProfiles;

namespace Chameleon.Avalonia.Controls.UserProfilesView;

public partial class ProjectsView : ChameleonNavigationPage,
    IProjectsView
{
    public ProjectsView()
    {
        InitializeComponent();
    }
}