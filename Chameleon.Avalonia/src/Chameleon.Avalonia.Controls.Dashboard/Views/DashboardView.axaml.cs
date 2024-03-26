using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Chameleon.Av.Fluent.Common.Pages;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;

namespace Chameleon.Avalonia.Controls.Dashboard;

public partial class DashboardView : ChameleonNavigationPage
        , IDashboardView
{
    public DashboardView()
    {
        InitializeComponent();
    }
}