using Avalonia.Controls;
using Avalonia.Interactivity;
using Chameleon.Common.Helpers;
using Chameleon.Interfaces;
using Chameleon.Interfaces.App.Settings;

namespace Chameleon.Av.Fluent.Common.Controls;

public class SubPageViewControl<T> : UserControl where T : ISubPageViewModel
{
    public SubPageViewControl()
    {
        if (!Design.IsDesignMode && DataContext?.GetType() != typeof(T))
        {
            var subPageViewModel = ContainerServiceHelper.Resolve<T>();
            subPageViewModel?.LoadAsync();
            DataContext = subPageViewModel;
        }
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
    }
}
