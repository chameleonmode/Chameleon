using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.Dashboard;

namespace Chameleon.Avalonia.Controls.Dashboard;

public partial class DashboardView : UserControl
        , IDashboardView
{
    public DashboardView()
    {
        InitializeComponent();
    }
}