using Chameleon.app.Avalonia.ViewModels;
using Chameleon.Av.Fluent.Common.Pages;

namespace Chameleon.app.Avalonia.Views;

[Chameleon.lib.Common.Attributes.ViewModel(typeof(ProjectsViewModel))]
public partial class ProjectsView : ChameleonNavigationPage {
    public ProjectsView()
    {
        InitializeComponent();
    }
}