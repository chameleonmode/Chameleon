using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Chameleon.Interfaces.Dashboard;
using Chameleon.Interfaces.Dialogs;
using Prism.Ioc;

namespace Chameleon.Avalonia.Controls.Dashboard;

public partial class DashboardView : UserControl
        , IDashboardView
{
    public DashboardView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Initialize the WindowNotificationManager with the "TopLevel". Previously (v0.10), MainWindow
        var notifyService = ContainerLocator.Current.Resolve<IToastNotificationService>();
        notifyService.SetHostWindow(TopLevel.GetTopLevel(this));
    }
}