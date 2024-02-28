using Avalonia.Controls;
using Chameleon.Interfaces.Views;
using Chameleon.Interfaces.Windows;

namespace Chameleon.Avalonia.PrismApp;
public partial class MainWindow : Window  , IMainWindow
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
}