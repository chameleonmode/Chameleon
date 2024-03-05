using Avalonia.Controls;
using Chameleon.Avalonia.Controls.Sidebar;
using Chameleon.Common.Regions;
using Chameleon.Interfaces.Services;
using Chameleon.Interfaces.Views;
using Chameleon.Interfaces.Windows;
using Prism.Regions;
using Prism.Ioc;
using Chameleon.Avalonia.Controls.Dashboard;

namespace Chameleon.Avalonia.PrismApp;
public partial class MainWindow : Window, IMainWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public void SetContent(object content, string title = "TEMP")
    {
        throw new System.NotImplementedException();
    }

    public void ShowWaitIndicator()
    {
        throw new System.NotImplementedException();
    }

    public void HideWaitIndicator()
    {
        throw new System.NotImplementedException();
    }

    public object GetContent()
    {
        throw new System.NotImplementedException();
    }

    public void SetContent(INavigationContent navigationContent)
    {
        throw new System.NotImplementedException();
    }

    public void SetContent(string content)
    {
        var regionManager = ContainerLocator.Current.Resolve<IRegionManager>();

        regionManager.RegisterViewWithRegion(RegionNames.ContentRegion, typeof(DashboardView));

    }
}