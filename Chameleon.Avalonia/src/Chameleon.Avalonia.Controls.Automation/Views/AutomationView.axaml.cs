using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Automation.ViewModels;
using Chameleon.Interfaces.App.Automation.Views;
using System.Diagnostics;

namespace Chameleon.Avalonia.Controls.Automation.Views;
public partial class AutomationView
    : UserControl
    , IAutomationView
{
    public AutomationView()
    {
        try
        {
            DataContext = ContainerServiceHelper.Resolve<IAutomationViewModel>()
                ?? throw new InvalidOperationException("Not resolve moodel");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error: {ex.Message}");
            throw;
        }

        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is IHaveInitialize sp)
        {
            sp.InvokeInitializeAsyncCommand(e);
        }
    }
}
