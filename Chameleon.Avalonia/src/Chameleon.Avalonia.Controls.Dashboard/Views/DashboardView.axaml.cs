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
        DataContext = ContainerServiceHelper.Resolve<IDashboardViewModel>();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Initialize the WindowNotificationManager with the "TopLevel". Previously (v0.10), MainWindow
        var notifyService = ContainerServiceHelper.Resolve<IToastNotificationService>();
        notifyService.SetHostWindow(TopLevel.GetTopLevel(this));
    }
}